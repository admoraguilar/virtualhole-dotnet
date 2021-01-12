using System.IO;
using Newtonsoft.Json;
using Midnight;

namespace VirtualHole.Scraper
{
	public class LocalContentScraperSettingsProvider : ContentScraperSettingsProvider
	{
		public bool IsDevMode { get; set; } = false;

		public override ContentScraperSettings Get()
		{
			Directory.CreateDirectory(Path.Combine(PathUtilities.GetApplicationPath(), "data/"));

			string settingsPath = Path.Combine(PathUtilities.GetApplicationPath(), "data/settings.json");
			string settingsTxt = File.ReadAllText(settingsPath);

			string proxyListPath = Path.Combine(PathUtilities.GetApplicationPath(), "data/proxy-list.txt");
			string proxyList = File.ReadAllText(proxyListPath);

			ContentScraperSettings settings = JsonConvert.DeserializeObject<ContentScraperSettings>(settingsTxt);
			settings.ProxyPool = new ProxyPool(proxyList);

			settings.IsDevMode = IsDevMode;

			return settings;
		}
	}
}
