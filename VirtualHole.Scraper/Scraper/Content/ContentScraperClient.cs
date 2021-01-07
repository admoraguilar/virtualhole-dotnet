using Midnight.Logs;
using Midnight.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualHole.DB;

namespace VirtualHole.Scraper
{
	public class ContentScraperClient
	{
		public ContentScraperClient(ContentScraperSettings settings)
		{
			this.settings = settings;
			
			pipeline = new Pipeline<ContentScraperContext>(CreateScraperContext(settings));
			pipeline.Add(new CreatorsGetFromDBStep());
			pipeline.Add(new ContentScrapeYouTubeStep());
			pipeline.Add(new ContentSaveAsJsonToDiskStep());
			pipeline.Add(new ContentWriteToDBStep());
		}

		private ContentScraperSettings settings = null;
		private Pipeline<ContentScraperContext> pipeline = null;

		public bool IsRunning { get; private set; } = false;
		public int RunCount { get; private set; } = 0;
		public DateTime LastFullRun { get; private set; } = DateTime.MinValue;
		public DateTime LastRun { get; private set; } = DateTime.MinValue;

		public async Task RunAsync(CancellationToken cancellationToken = default)
		{
			if(IsRunning) { return; }
			IsRunning = true;

			// 1st scrape
			// 1. Scrape contents
			// 2. Store scraped contents in json

			// 2nd scrape
			// 1. Scrape contents
			// 2. Compare scraped contents with previous
			// 3. Only write changed ones:
			//   * Contents existing on new but not on old are "NEW"
			//   * Contents existing on old but not on new are "DELETED, PRIVATED"
			// This makes the load on MongoDB much lighter
			while(true) {
				if(cancellationToken.IsCancellationRequested) {
					IsRunning = false;
				}
				cancellationToken.ThrowIfCancellationRequested();

				MLog.Clear();
				LastFullRun = DateTime.Now;

				using(StopwatchScope stopwatch = new StopwatchScope(
					nameof(ContentScraperClient),
					"Start run..",
					$"Success! Taking a break before next iteration. Next run {DateTime.Now.AddSeconds(settings.IterationGapAmount)}")) {
					await TaskExt.RetryAsync(
						() => pipeline.ExecuteAsync(),
						TimeSpan.FromSeconds(5), int.MaxValue,
						cancellationToken);
				}

				RunCount++;
				LastRun = DateTime.Now;

				MLog.Log(
					nameof(ContentScraperClient),
					$"Run details: {Environment.NewLine}" +
					$"Run count: {RunCount} | Last Run: {LastRun} {Environment.NewLine}" +
					$"Last Full Run: {LastFullRun}");

				await Task.Delay(TimeSpan.FromSeconds(settings.IterationGapAmount), cancellationToken);

				if(DateTime.Now.Subtract(LastFullRun).Days > 0) {
					LastFullRun = DateTime.Now;
					settings.IsStartIncremental = false;
					pipeline.SetContext(CreateScraperContext(settings));
				}
			}
		}

		private ContentScraperContext CreateScraperContext(ContentScraperSettings settings)
		{
			return new ContentScraperContext(
				new ScraperClient(settings.ProxyPool),
				new VirtualHoleDBClient(
					settings.ConnectionString, settings.UserName,
					settings.Password)) {
				InIsIncremental = settings.IsStartIncremental
			};
		}
	}
}
