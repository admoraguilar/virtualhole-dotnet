using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Midnight;
using Midnight.Logs;
using Midnight.Tasks;

namespace VirtualHole.Scraper
{
	using DB.Contents.Videos;
	using DB.Contents.Creators;

	public class VirtualHoleScraperRunner
	{
		private List<Creator> creators = new List<Creator>();
		private List<Video> videos = new List<Video>();

		private ContentClient content = null;

		private Queue<Action> actionQueue = new Queue<Action>();
		private DateTime lastFullRun = DateTime.MinValue;
		private DateTime lastRun = DateTime.MinValue;

		private bool isRunning = false;
		private CancellationTokenSource cts = null;

		public async Task RunAsync(bool isIncremental, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if(isRunning) { return; }
			isRunning = true;

			lastFullRun = DateTime.Now;

			using(StopwatchScope stopwatch = new StopwatchScope(
				nameof(ContentClient),
				"Start run..",
				"Success! Taking a break before next iteration.")) {
				await TaskExt.RetryAsync(
					() => content.WriteToVideosDBUsingCreatorsDBAsync(
						isIncremental, cancellationToken),
					TimeSpan.FromSeconds(3), 3, cancellationToken
				);
			}

			if(DateTime.Now.Subtract(lastFullRun).Days > 0) {
				lastFullRun = DateTime.Now;

				actionQueue.Enqueue(() => {
					Task.Run(() => RunAsync(false, cancellationToken));
				});
			} else {
				actionQueue.Enqueue(() => {
					Task.Run(() => RunAsync(true, cancellationToken));
				});
			}

			lastRun = DateTime.Now;
			isRunning = false;
		}

		public void Run()
		{
			string startupPath = Path.Combine(PathUtilities.GetApplicationPath(), "data/startup.json");
			string startupTxt = File.ReadAllText(startupPath);
			VirtualHoleScraperSettings scraperSettings = JsonConvert.DeserializeObject<VirtualHoleScraperSettings>(startupTxt);

			string proxyListPath = Path.Combine(PathUtilities.GetApplicationPath(), "data/proxy-list.txt");
			string proxyList = File.ReadAllText(proxyListPath);

			string creatorListPath = Path.Combine(PathUtilities.GetApplicationPath(), "data/creators.json");
			string creatorList = File.ReadAllText(creatorListPath);
			creators = JsonConvert.DeserializeObject<List<Creator>>(creatorList);
			creators = creators.Take(20).ToList();

			ContentClientSettings settings = new ContentClientSettings() {
				ConnectionString = scraperSettings.ConnectionString,
				UserName = scraperSettings.UserName,
				Password = scraperSettings.Password,
				ProxyPool = new ProxyPool(proxyList)
			};

			content = new ContentClient(settings);

			CancellationTokenSourceExt.CancelAndCreate(ref cts);
			Task.Run(() => RunAsync(false, cts.Token));
		}
	}
}
