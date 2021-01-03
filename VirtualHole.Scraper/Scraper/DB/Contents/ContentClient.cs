using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Logs;
using Midnight.Tasks;
using VirtualHole.DB;
using VirtualHole.DB.Creators;
using VirtualHole.DB.Contents;

namespace VirtualHole.Scraper.Contents
{
	public class ContentClient
	{
		private ScraperClient scraperClient = null;
		private VirtualHoleDBClient dbClient = null;

		public ContentClient(ScraperClient scraperClient, VirtualHoleDBClient dbClient)
		{
			this.scraperClient = scraperClient;
			this.dbClient = dbClient;
		}

		public async Task<List<Content>> ScrapeAsync(
			IEnumerable<Creator> creators, bool incremental = false,
			CancellationToken cancellationToken = default)
		{
			List<Content> contents = new List<Content>();
			await Concurrent.ForEachAsync(creators.ToList(), ProcessCreator, 5, cancellationToken);
			return contents;

			async Task ProcessCreator(Creator creator)
			{
				if(creator.IsGroup) { return; }

				YoutubeScraper.GetChannelVideoSettings channelVideoSettings = null;
				if(incremental) {
					channelVideoSettings = new YoutubeScraper.GetChannelVideoSettings {
						AnchorDate = DateTimeOffset.UtcNow.Date,
						IsForward = true
					};
				}

				// YouTube
				foreach(CreatorSocial youtube in creator.Socials.Where(s => s.SocialType == SocialTypes.YouTube)) {
					await Task.WhenAll(
						Task.Run(async () => {
							contents.AddRange(await ProcessSocialVideo(
							   youtube, "Videos",
							   (CreatorSocial yt) => scraperClient.Youtube.Get().GetChannelVideosAsync(creator, yt, channelVideoSettings)));
						}),
						Task.Run(async () => {
							contents.AddRange(await ProcessSocialVideo(
							   youtube, "Scheduled",
							   (CreatorSocial yt) => scraperClient.Youtube.Get().GetChannelUpcomingBroadcastsAsync(creator, yt)));
						}),
						Task.Run(async () => {
							contents.AddRange(await ProcessSocialVideo(
								youtube, "Live",
								(CreatorSocial yt) => scraperClient.Youtube.Get().GetChannelLiveBroadcastsAsync(creator, yt)));
						})
					);
				}

				async Task<List<T>> ProcessSocialVideo<T>(CreatorSocial social, string socialPageName, Func<CreatorSocial, Task<List<T>>> task)
				{
					using(StopwatchScope s = new StopwatchScope(
						nameof(ContentClient),
						$"Processing [{social.SocialType} - {social.Name} - {socialPageName}]...",
						$"Finished processing [{social.SocialType} - {social.Name} - {socialPageName}]!")) {
						return await TaskExt.RetryAsync(() => task(social), TimeSpan.FromSeconds(1), int.MaxValue, cancellationToken);
					}
				}
			}
		}

		public async Task WriteToDBAsync(
			IEnumerable<Content> contents, bool isIncremental = false,
			CancellationToken cancellationToken = default)
		{
			CancellationTokenSource writeCts = null;
			CancellationTokenSourceExt.CancelAndCreate(ref writeCts);

			using(StopwatchScope s = new StopwatchScope(
				nameof(ContentClient),
				"Writing to content collection...",
				"Finished writing to content collection!")) {
				Task timeout = Task.Delay(TimeSpan.FromMinutes(30), cancellationToken);
				Task write = TaskExt.RetryAsync(
					() => WriteAsync(),
					TimeSpan.FromSeconds(5), int.MaxValue,
					writeCts.Token
				);

				try {
					Task done = await Task.WhenAny(write, timeout);
					if(done == timeout) {
						MLog.Log(MLogLevel.Warning, nameof(ContentClient), "Write operation timed out!");
						CancellationTokenSourceExt.Cancel(ref writeCts);
					}
				} catch (OperationCanceledException c) {
					CancellationTokenSourceExt.Cancel(ref writeCts);
				}
			}

			Task WriteAsync()
			{
				if(isIncremental) { return dbClient.Contents.UpsertManyContentsAsync(contents, writeCts.Token); } 
				else { return dbClient.Contents.UpsertManyContentsAndDeleteDanglingAsync(contents, writeCts.Token); }
			}
		}
	}
}
