
namespace VirtualHole.Scraper
{
	public class ScraperClient
	{
		public YoutubeScraperFactory Youtube { get; set; } = null;

		public ScraperClient(ProxyPool proxyPool)
		{
			Youtube = new YoutubeScraperFactory(proxyPool);
		}
	}
}
