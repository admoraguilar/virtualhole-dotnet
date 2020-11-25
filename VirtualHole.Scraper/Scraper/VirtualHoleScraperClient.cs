using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Logs;
using Midnight.Tasks;
using VirtualHole.DB;

namespace VirtualHole.Scraper
{
	public class VirtualHoleScraperClient
	{
		private ContentClient contentClient = null;

		private Queue<Action> actionQueue = new Queue<Action>();
		private DateTime lastFullRun = DateTime.MinValue;
		private DateTime lastRun = DateTime.MinValue;

		private int iterationGapAmount = 300;
		private bool isStartIncremental = false;

		private bool isRunning = false;
		private int runCounter = 0;

		public VirtualHoleScraperClient(VirtualHoleScraperSettings settings)
		{
			ScraperClient scraperClient = new ScraperClient(settings.ProxyPool);
			VirtualHoleDBClient dbClient = new VirtualHoleDBClient(
				settings.ConnectionString, settings.UserName, 
				settings.Password);

			contentClient = new ContentClient(scraperClient, dbClient);

			iterationGapAmount = settings.IterationGapAmount;
			isStartIncremental = settings.IsStartIncremental;
		}

		public async Task RunAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if(isRunning) { return; }
			isRunning = true;

			MLog.Clear();

			lastFullRun = DateTime.Now;

			using(StopwatchScope stopwatch = new StopwatchScope(
				nameof(ContentClient),
				"Start run..",
				$"Success! Taking a break before next iteration. Break for {iterationGapAmount}s")) {
				await TaskExt.RetryAsync(
					() => contentClient.WriteToVideosDBUsingCreatorsDBAsync(
						isStartIncremental, cancellationToken),
						TimeSpan.FromSeconds(3), 3, cancellationToken
				);
			}

			if(DateTime.Now.Subtract(lastFullRun).Days > 0) {
				lastFullRun = DateTime.Now;

				actionQueue.Enqueue(() => {
					isStartIncremental = false;
					Task.Run(() => RunAsync(cancellationToken));
				});
			} else {
				actionQueue.Enqueue(() => {
					Task.Run(() => RunAsync(cancellationToken));
				});
			}

			runCounter++;
			lastRun = DateTime.Now;
			isRunning = false;

			MLog.Log(
				nameof(VirtualHoleScraperClient), 
				$"Run details: {Environment.NewLine}" +
				$"Run count: {runCounter} | Last Run: {lastRun} {Environment.NewLine}" +
				$"Last Full Run: {lastFullRun}");

			await Task.Delay(TimeSpan.FromSeconds(iterationGapAmount));

			Action nextTask = actionQueue.Dequeue();
			nextTask();
		}
	}
}
