using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Midnight.Logs;
using Midnight.Tasks;
using Midnight.Pipeline;
using VirtualHole.DB;
using VirtualHole.DB.Creators;
using VirtualHole.DB.Contents;

namespace VirtualHole.Scraper
{
	public class ContentScrapeYouTubeStep : PipelineStep<ContentScraperContext>
	{
		public override async Task ExecuteAsync()
		{
			ConcurrentBag<Content> contentBag = new ConcurrentBag<Content>();
			await Concurrent.ForEachAsync(Context.InCreators, ProcessCreator, 100);
			Context.OutResults.AddRange(contentBag);
			MLog.Log(nameof(ContentScrapeYouTubeStep), $"Scraped a total of {Context.OutResults.Count} contents.");

			async Task ProcessCreator(Creator creator)
			{
				foreach(CreatorSocial youtube in creator.Socials.Where(s => s.SocialType == SocialTypes.YouTube)) {
					await Task.WhenAll(
						Task.Run(async () => {
							await ProcessYouTubeContents(
							   youtube, "Videos",
							   (CreatorSocial yt) => Context.InScraper.Youtube.Get().GetChannelVideosAsync(creator, yt));
						}),
						Task.Run(async () => {
							await ProcessYouTubeContents(
							   youtube, "Scheduled",
							   (CreatorSocial yt) => Context.InScraper.Youtube.Get().GetChannelUpcomingBroadcastsAsync(creator, yt));
						}),
						Task.Run(async () => {
							await ProcessYouTubeContents(
								youtube, "Live",
								(CreatorSocial yt) => Context.InScraper.Youtube.Get().GetChannelLiveBroadcastsAsync(creator, yt));
						})
					);
				}

				async Task ProcessYouTubeContents<T>(
					CreatorSocial social, string socialPageName, 
					Func<CreatorSocial, Task<List<T>>> task) where T : Content
				{
					List<T> contents = null;

					using(StopwatchScope s = new StopwatchScope(
						nameof(ContentScrapeYouTubeStep),
						$"Processing [{social.SocialType} - {social.Name} - {socialPageName}]...",
						$"Finished processing [{social.SocialType} - {social.Name} - {socialPageName}]!")) {
						contents = await TaskExt.RetryAsync(() => task(social), TimeSpan.FromSeconds(1), int.MaxValue);
					}

					contents.ForEach((T content) => contentBag.Add(content));
				}
			}
		}
	}
}
