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
		private static void Main(string[] args)
		{
			// We do this as we're dealing with camel case convention JSON files
			// which is not aligned with C#'s naming convention
			JsonConvert.DefaultSettings = () => JsonUtilities.SerializerSettings.DefaultCamelCase;

			MLog.Log("Running scraper...");

			string proxyListPath = Path.Combine(PathUtilities.GetApplicationPath(), "data/proxy-list.txt");
			string proxyList = File.ReadAllText(proxyListPath);

			string settingsPath = Path.Combine(PathUtilities.GetApplicationPath(), "data/settings.json");
			string settingsTxt = File.ReadAllText(settingsPath);

			VirtualHoleScraperSettings scraperSettings = JsonConvert.DeserializeObject<VirtualHoleScraperSettings>(settingsTxt);
			scraperSettings.ProxyPool = new ProxyPool(proxyList);

			VirtualHoleScraperClient runner = new VirtualHoleScraperClient(scraperSettings);
			Task.Run(() => runner.RunAsync());

			Console.ReadLine();
		}
	}
}
