using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight;
using Midnight.Logs;
using Midnight.Tasks;

namespace VirtualHole.Scraper
{
	using DB.Contents.Videos;
	using DB.Contents.Creators;
	using Newtonsoft.Json;

	public class VirtualHoleScraperRunner
	{
		private List<Creator> _creators = new List<Creator>();
		private List<Video> _videos = new List<Video>();

		private VirtualHoleScraperClient _client = null;
		private CancellationTokenSource _cts = null;

		public async Task RunAsync(CancellationToken cancellationToken = default)
		{
			MLog.Log("Starting to scrape creator details..");
			while(true) {
				_videos = new List<Video>();
				_videos.AddRange(await _client.videos.ScrapeAsync(_creators, false, cancellationToken));

				Console.Clear();

				MLog.Log($"Finished: [Creators: {_creators.Count}] | [Videos: {_videos.Count}]");
				await Task.Delay(TimeSpan.FromSeconds(10));
			}
		}

		public void Run()
		{
			string proxyListPath = Path.Combine(PathUtilities.GetApplicationPath(), "config/proxy-list.txt");
			string proxyList = File.ReadAllText(proxyListPath);

			string creatorListPath = Path.Combine(PathUtilities.GetApplicationPath(), "config/creators.json");
			string creatorList = File.ReadAllText(creatorListPath);
			_creators = JsonConvert.DeserializeObject<List<Creator>>(creatorList);
			_creators = _creators.Take(20).ToList();

			VirtualHoleScraperClientSettings settings = new VirtualHoleScraperClientSettings() {
				connectionString = "mongodb+srv://<username>:<password>@us-east-1-free.41hlb.mongodb.net/test",
				password = "holoverse-editor",
				userName = "RBqYN3ugVTb2stqD",
				proxyPool = new ProxyPool(proxyList)
			};

			_client = new VirtualHoleScraperClient(settings);

			CancellationTokenSourceExt.CancelAndCreate(ref _cts);
			Task.Run(() => RunAsync(_cts.Token));
		}
	}

	internal class Program
	{
		private static void Main(string[] args)
		{
			MLog.Log("Running scraper...");

			VirtualHoleScraperRunner runner = new VirtualHoleScraperRunner();
			runner.Run();

			Console.ReadLine();
		}
	}
}
