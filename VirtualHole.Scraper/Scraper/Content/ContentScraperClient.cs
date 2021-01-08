using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Midnight.Logs;
using Midnight.Tasks;
using Midnight.Pipeline;
using VirtualHole.DB;

namespace VirtualHole.Scraper
{
	public class ContentScraperClient
	{
		private ScraperClient scraperClient = null;
		private VirtualHoleDBClient dbClient = null;

		public ContentScraperClient(ContentScraperSettings settings)
		{
			Debug.Assert(settings != null);

			this.settings = settings;

			scraperClient = new ScraperClient(settings.ProxyPool);
			dbClient = new VirtualHoleDBClient(
				settings.ConnectionString, settings.UserName, 
				settings.Password);

			firstPipeline = new Pipeline<ContentScraperContext>(new ContentScraperContext(scraperClient, dbClient));
			firstPipeline.Add(new ContentGetAllFromDBAndStoreStep());
			firstPipeline.Add(new CreatorGetFromDBStep());
			firstPipeline.Add(new ContentScrapeYouTubeStep());
			firstPipeline.Add(new ContentWriteToDBStep());

			secondPipeline = new Pipeline<ContentScraperContext>(new ContentScraperContext(scraperClient, dbClient));
			secondPipeline.Add(new CreatorGetFromDBStep());
			secondPipeline.Add(new ContentScrapeYouTubeStep());
			secondPipeline.Add(new ContentWriteToDBStep());
		}

		private ContentScraperSettings settings = null;
		private Pipeline<ContentScraperContext> firstPipeline = null;
		private Pipeline<ContentScraperContext> secondPipeline = null;

		public bool IsRunning { get; private set; } = false;
		public int RunCount { get; private set; } = 0;
		public DateTime LastFullRun { get; private set; } = DateTime.MinValue;
		public DateTime LastRun { get; private set; } = DateTime.MinValue;

		public async Task RunAsync(CancellationToken cancellationToken = default)
		{
			if(IsRunning) { return; }
			IsRunning = true;

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
					$"Success! Taking a break before next iteration.")) {
					await TaskExt.RetryAsync(
						() => {
							if(RunCount <= 0) {
								firstPipeline.Context.Reset();
								return firstPipeline.ExecuteAsync();
							} else {
								secondPipeline.Context.Reset();
								return secondPipeline.ExecuteAsync();
							}
						},
						TimeSpan.FromSeconds(5), int.MaxValue,
						cancellationToken);
				}

				RunCount++;
				LastRun = DateTime.Now;

				MLog.Log(
					nameof(ContentScraperClient),
					$"Run details: {Environment.NewLine}" +
					$"Run count: {RunCount} | Last Run: {LastRun} {Environment.NewLine}" +
					$"Last full run: {LastFullRun} | Next run: {DateTime.Now.AddSeconds(settings.IterationGapAmount)}");

				await Task.Delay(TimeSpan.FromSeconds(settings.IterationGapAmount), cancellationToken);

				if(DateTime.Now.Subtract(LastFullRun).Days > 0) {
					LastFullRun = DateTime.Now;
					settings.IsStartIncremental = false;
				}
			}
		}
	}
}
