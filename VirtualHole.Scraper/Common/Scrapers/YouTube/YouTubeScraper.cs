using System;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Channels;
using VirtualHole.DB.Creators;
using VirtualHole.DB.Contents;

namespace VirtualHole.Scraper
{
	public class YoutubeScraper
	{
		public class GetChannelVideoSettings
		{
			public DateTimeOffset AnchorDate = DateTimeOffset.MinValue;
			public bool IsForward = true;
		}

		private YoutubeClient client = null;

		public YoutubeScraper()
		{
			client = new YoutubeClient();
		}

		public YoutubeScraper(HttpClient httpClient)
		{
			client = new YoutubeClient(httpClient);
		}

		public async Task<YouTubeSocial> GetChannelInfoAsync(string channelUrl)
		{
			Channel channel = await client.Channels.GetAsync(channelUrl);
			return new YouTubeSocial {
				Name = channel.Title,
				Id = channel.Id,
				Url = channel.Url,
				AvatarUrl = channel.LogoUrl
			};
		}

		public async Task<List<YouTubeVideo>> GetChannelVideosAsync(
			Creator creator, CreatorSocial creatorSocial,
			GetChannelVideoSettings settings = null)
		{
			List<YouTubeVideo> results = new List<YouTubeVideo>();

			IReadOnlyList<Video> videos = await client.Channels.GetUploadsAsync(creatorSocial.Url);
			DateTimeOffset uploadDateAnchor = default;
			foreach(Video video in videos) {
				// We process the video date because sometimes
				// the dates are messed up, so we run a correction to
				// fix it
				if(uploadDateAnchor != default && video.UploadDate.Subtract(uploadDateAnchor).TotalDays > 60) {
					//MLog.Log(
					//	nameof(YouTubeScraper),
					//	$"Wrong date detected from [L: {uploadDateAnchor} | C:{video.UploadDate}]! " +
					//	$"Fixing {video.Title}..."
					//);
					
					// Disabled: We don't do a full checking of a the video anymore for
					// full accuracy because it's too slow of a process especially if there's
					// too many discrepancies in dates
					//processedVideo = await _client.Videos.GetAsync(processedVideo.Url);

					// Hack: as the videos we're scraping are always descending
					// we just put a date that's a bit behind the upload date anchor
					// this is so if we put things the videos in order they'd still be
					// in order albeit now with accurate dates
					uploadDateAnchor = uploadDateAnchor.AddDays(-1);
				} else {
					uploadDateAnchor = video.UploadDate;
				}

				if(settings != null) {
					if(settings.IsForward && settings.AnchorDate > uploadDateAnchor) { continue; }
					if(!settings.IsForward && settings.AnchorDate < uploadDateAnchor) { continue; }
				}

				results.Add(new YouTubeVideo {
					Title = video.Title,
					Id = video.Id,
					Url = video.Url,
					Creator = new CreatorSimple(creator, creatorSocial),
					CreationDate = uploadDateAnchor,
					Tags = video.Keywords.ToList(),
					ThumbnailUrl = video.Thumbnails.MediumResUrl,
					Description = video.Description,
					Duration = video.Duration,
					ViewsCount = video.Engagement.ViewCount,
					LikesCount = video.Engagement.LikeCount,
					DislikesCount = video.Engagement.DislikeCount
				});
			}

			return results;
		}

		public async Task<List<YouTubeBroadcast>> GetChannelLiveBroadcastsAsync(Creator creator, CreatorSocial creatorSocial)
		{
			return await GetChannelBroadcastsAsync(creator, creatorSocial, BroadcastType.Now);
		}

		public async Task<List<YouTubeBroadcast>> GetChannelUpcomingBroadcastsAsync(Creator creator, CreatorSocial creatorSocial)
		{
			return await GetChannelBroadcastsAsync(creator, creatorSocial, BroadcastType.Upcoming);
		}

		private async Task<List<YouTubeBroadcast>> GetChannelBroadcastsAsync(
			Creator creator, CreatorSocial creatorSocial,
			BroadcastType type)
		{
			List<YouTubeBroadcast> results = new List<YouTubeBroadcast>();

			IReadOnlyList<Video> broadcasts = await client.Channels.GetBroadcastsAsync(creatorSocial.Url, type);
			foreach(Broadcast broadcast in broadcasts.Select(v => v as Broadcast)) {
				results.Add(new YouTubeBroadcast() {
					Title = broadcast.Title,
					Id = broadcast.Id,
					Url = broadcast.Url,
					Creator = new CreatorSimple(creator, creatorSocial),
					CreationDate = broadcast.UploadDate,
					Tags = broadcast.Keywords.ToList(),
					ThumbnailUrl = broadcast.Thumbnails.MediumResUrl,
					Description = broadcast.Description,
					Duration = broadcast.Duration,
					ViewsCount = broadcast.Engagement.ViewCount,
					IsLive = broadcast.IsLive,
					ViewerCount = broadcast.ViewerCount,
					ScheduleDate = broadcast.Schedule,
					LikesCount = broadcast.Engagement.LikeCount,
					DislikesCount = broadcast.Engagement.DislikeCount
				});
			}

			return results;
		}
	}
}
