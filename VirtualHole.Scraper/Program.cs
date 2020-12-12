using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Midnight;
using Midnight.Logs;

namespace VirtualHole.Scraper
{
	internal class Program
	{
		private static VirtualHoleScraperSettings GetSettings()
		{
			string settingsPath = Path.Combine(PathUtilities.GetApplicationPath(), "data/settings.json");
			string settingsTxt = File.ReadAllText(settingsPath);

			string proxyListPath = Path.Combine(PathUtilities.GetApplicationPath(), "data/proxy-list.txt");
			string proxyList = File.ReadAllText(proxyListPath);

			VirtualHoleScraperSettings settings = JsonConvert.DeserializeObject<VirtualHoleScraperSettings>(settingsTxt);
			settings.ProxyPool = new ProxyPool(proxyList);

			return settings;
		}

		private static void Main(string[] args)
		{
			JsonConfig.Initialize();

			MLog.Log("Running scraper...");

			VirtualHoleScraperClient runner = new VirtualHoleScraperClient(GetSettings());
			Task.Run(() => runner.RunAsync());

			Console.ReadLine();
		}
	}
}
