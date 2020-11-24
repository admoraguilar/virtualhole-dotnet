
namespace VirtualHole.Scraper
{
	public class YoutubeScraperFactory : ScraperFactory<YoutubeScraper>
	{
		public YoutubeScraperFactory(ProxyPool proxyPool) : base(proxyPool)
		{ }

		protected override YoutubeScraper InternalGet_Impl(Proxy proxy)
		{
			return new YoutubeScraper(HttpClientFactory.Get(proxy));
		}

		protected override YoutubeScraper InternalGet_Impl()
		{
			return new YoutubeScraper();
		}
	}
}
