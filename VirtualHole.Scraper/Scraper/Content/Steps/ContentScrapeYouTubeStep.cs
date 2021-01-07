using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Logs;
using Midnight.Tasks;
using VirtualHole.DB;
using VirtualHole.DB.Creators;

namespace VirtualHole.Scraper
{
	public class ContentScrapeYouTubeStep : PipelineStep<ContentScraperContext>
	{
		public override async Task ExecuteAsync()
		{
			await Concurrent.ForEachAsync(Context.InCreators, ProcessCreator, 99);
			MLog.Log(nameof(ContentScrapeYouTubeStep), $"Scraped a total of {Context.OutResults.Count} contents.");

			async Task ProcessCreator(Creator creator)
			{
				if(creator.IsGroup) { return; }

				YoutubeScraper.GetChannelVideoSettings channelVideoSettings = null;
				if(Context.InIsIncremental) {
					channelVideoSettings = new YoutubeScraper.GetChannelVideoSettings {
						AnchorDate = DateTimeOffset.UtcNow.Date,
						IsForward = true
					};
				}

				// YouTube
				foreach(CreatorSocial youtube in creator.Socials.Where(s => s.SocialType == SocialTypes.YouTube)) {
					await Task.WhenAll(
						Task.Run(async () => {
							Context.OutResults.AddRange(await ProcessSocialVideo(
							   youtube, "Videos",
							   (CreatorSocial yt) => Context.InScraper.Youtube.Get().GetChannelVideosAsync(creator, yt, channelVideoSettings)));
						}),
						Task.Run(async () => {
							Context.OutResults.AddRange(await ProcessSocialVideo(
							   youtube, "Scheduled",
							   (CreatorSocial yt) => Context.InScraper.Youtube.Get().GetChannelUpcomingBroadcastsAsync(creator, yt)));
						}),
						Task.Run(async () => {
							Context.OutResults.AddRange(await ProcessSocialVideo(
								youtube, "Live",
								(CreatorSocial yt) => Context.InScraper.Youtube.Get().GetChannelLiveBroadcastsAsync(creator, yt)));
						})
					);
				}

				async Task<List<T>> ProcessSocialVideo<T>(CreatorSocial social, string socialPageName, Func<CreatorSocial, Task<List<T>>> task)
				{
					using(StopwatchScope s = new StopwatchScope(
						nameof(ContentScrapeYouTubeStep),
						$"Processing [{social.SocialType} - {social.Name} - {socialPageName}]...",
						$"Finished processing [{social.SocialType} - {social.Name} - {socialPageName}]!")) {
						return await TaskExt.RetryAsync(() => task(social), TimeSpan.FromSeconds(1), int.MaxValue);
					}
				}
			}
		}
	}
}
