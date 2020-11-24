
namespace VirtualHole.Scraper
{
	public class VirtualHoleScraperSettings
	{
		public int IterationGapAmount = 3600;
		public bool UseProxies = true;
		public bool IsStartIncremental = true;

		public string ConnectionString = string.Empty;
		public string UserName = string.Empty;
		public string Password = string.Empty;
	}
}
