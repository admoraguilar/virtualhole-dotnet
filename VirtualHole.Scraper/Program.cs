using System;
using System.Threading.Tasks;
using Midnight.Logs;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using VirtualHole.DB.Contents;
using Midnight;

namespace VirtualHole.Scraper
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			JsonConfig.Initialize();

			MLog.Log("Running scraper...");
			ContentScraperSettingsProvider settingsProvider = new LocalContentScraperSettingsProvider();
			ContentScraperClient client = new ContentScraperClient(settingsProvider.Get());
			Task.Run(() => client.RunAsync());

			// TESTS
			//string contentsPath = Path.Combine(PathUtilities.GetApplicationPath(), "data/results/contents.json");
			//string prevContentsJson = File.ReadAllText(contentsPath);
			//List<Content> prevContents = JsonConvert.DeserializeObject<List<Content>>(prevContentsJson);
			//MLog.Log(prevContents.Count);

			//List<Content> prevContents2 = JsonConvert.DeserializeObject<List<Content>>(prevContentsJson);
			//List<Content> newContents = prevContents.Except(prevContents2).ToList();
			//MLog.Log(newContents.Count);

			Console.ReadLine();
		}
	}
}
