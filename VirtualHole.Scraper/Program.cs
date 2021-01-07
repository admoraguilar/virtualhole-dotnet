using System;
using System.Threading.Tasks;
using Midnight.Logs;

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

			Console.ReadLine();
		}
	}
}
