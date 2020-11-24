using System;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using YoutubeExplode;
using YoutubeExplode.Videos;
using VirtualHole.DB.Contents;
using VirtualHole.DB.Contents.Creators;

using ExplodeVideo = YoutubeExplode.Videos.Video;
using ExplodeBroadcast = YoutubeExplode.Videos.Broadcast;
using ExplodeChannel = YoutubeExplode.Channels.Channel;
using DBVideo = VirtualHole.DB.Contents.Videos.Video;
using DBBroadcast = VirtualHole.DB.Contents.Videos.Broadcast;

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

		public async Task<Social> GetChannelInfoAsync(string channelUrl)
		{
			ExplodeChannel channel = await client.Channels.GetAsync(channelUrl);
			return new Social {
				Name = channel.Title,
				Platform = Platform.YouTube,
				Id = channel.Id,
				Url = channel.Url,
				AvatarUrl = channel.LogoUrl
			};
		}

		public async Task<List<DBVideo>> GetChannelVideosAsync(
			Creator creator, string channelUrl,
			GetChannelVideoSettings settings = null)
		{
			List<DBVideo> results = new List<DBVideo>();

			IReadOnlyList<ExplodeVideo> videos = await client.Channels.GetUploadsAsync(channelUrl);
			DateTimeOffset uploadDateAnchor = default;
			foreach(ExplodeVideo video in videos) {
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

				results.Add(new DBVideo() {
					Title = video.Title,
					Platform = Platform.YouTube,
					Id = video.Id,
					Url = video.Url,

					Creator = video.Author,
					CreatorId = video.ChannelId,
					CreatorUniversal = creator.UniversalName,
					CreatorIdUniversal = creator.UniversalId,

					CreationDate = uploadDateAnchor,
					Tags = video.Keywords.ToArray(),

					ThumbnailUrl = video.Thumbnails.HighRes720Url,
					Description = video.Description,
					Duration = video.Duration,
					ViewCount = video.Engagement.ViewCount
				});
			}

			return results;
		}

		public async Task<List<DBBroadcast>> GetChannelLiveBroadcastsAsync(Creator creator, string channelUrl)
		{
			return await GetChannelBroadcastsAsync(creator, channelUrl, BroadcastType.Now);
		}

		public async Task<List<DBBroadcast>> GetChannelUpcomingBroadcastsAsync(Creator creator, string channelUrl)
		{
			return await GetChannelBroadcastsAsync(creator, channelUrl, BroadcastType.Upcoming);
		}

		private async Task<List<DBBroadcast>> GetChannelBroadcastsAsync(
			Creator creator, string channelUrl,
			BroadcastType type)
		{
			List<DBBroadcast> results = new List<DBBroadcast>();

			IReadOnlyList<ExplodeVideo> broadcasts = await client.Channels.GetBroadcastsAsync(channelUrl, type);
			foreach(ExplodeBroadcast broadcast in broadcasts.Select(v => v as ExplodeBroadcast)) {
				results.Add(new DBBroadcast() {
					Title = broadcast.Title,
					Platform = Platform.YouTube,
					Id = broadcast.Id,
					Url = broadcast.Url,

					Creator = broadcast.Author,
					CreatorId = broadcast.ChannelId,
					CreatorUniversal = creator.UniversalName,
					CreatorIdUniversal = creator.UniversalId,

					CreationDate = broadcast.UploadDate,
					Tags = broadcast.Keywords.ToArray(),

					ThumbnailUrl = broadcast.Thumbnails.HighRes720Url,
					Description = broadcast.Description,
					Duration = broadcast.Duration,
					ViewCount = broadcast.Engagement.ViewCount,

					IsLive = broadcast.IsLive,
					ViewerCount = broadcast.ViewerCount,
					ScheduleDate = broadcast.Schedule
				});
			}

			return results;
		}
	}
}
