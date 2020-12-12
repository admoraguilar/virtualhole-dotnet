using System;
using System.Threading;
using System.Threading.Tasks;
using Midnight.Logs;
using Midnight.Tasks;
using VirtualHole.DB;

namespace VirtualHole.Scraper
{
	public class VirtualHoleScraperClient
	{
		public bool IsStartIncremental { get; private set; } = false;
		public int IterationGapAmount { get; private set; } = 300;

		public bool IsRunning { get; private set; } = false;
		public int RunCount { get; private set; } = 0;
		public DateTime LastFullRun { get; private set; } = DateTime.MinValue;
		public DateTime LastRun { get; private set; } = DateTime.MinValue;

		private DBClient contentClient = null;

		public VirtualHoleScraperClient(VirtualHoleScraperSettings settings)
		{
			IsStartIncremental = settings.IsStartIncremental;
			IterationGapAmount = settings.IterationGapAmount;
			
			contentClient = new DBClient(
				new ScraperClient(settings.ProxyPool),
				new VirtualHoleDBClient(
					settings.ConnectionString, settings.UserName,
					settings.Password));
		}

		public async Task RunAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if(IsRunning) { return; }
			IsRunning = true;

			MLog.Clear();
			LastFullRun = DateTime.Now;

			using(StopwatchScope stopwatch = new StopwatchScope(
				nameof(DBClient),
				"Start run..",
				$"Success! Taking a break before next iteration. Next run {DateTime.Now.AddSeconds(IterationGapAmount)}")) {
				await TaskExt.RetryAsync(
					() => contentClient.WriteToVideosDBUsingCreatorsDBAsync(
						IsStartIncremental, cancellationToken),
						TimeSpan.FromSeconds(3), int.MaxValue, cancellationToken
				);
			}

			RunCount++;
			LastRun = DateTime.Now;
			IsRunning = false;

			MLog.Log(
				nameof(VirtualHoleScraperClient), 
				$"Run details: {Environment.NewLine}" +
				$"Run count: {RunCount} | Last Run: {LastRun} {Environment.NewLine}" +
				$"Last Full Run: {LastFullRun}");

			await Task.Delay(TimeSpan.FromSeconds(IterationGapAmount));

			if(DateTime.Now.Subtract(LastFullRun).Days > 0) {
				LastFullRun = DateTime.Now;
				IsStartIncremental = false;
				_ = Task.Run(() => RunAsync(cancellationToken));
			} else {
				_ = Task.Run(() => RunAsync(cancellationToken));
			}
		}
	}
}
