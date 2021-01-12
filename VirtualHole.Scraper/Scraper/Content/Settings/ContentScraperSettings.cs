using VirtualHole.DB;

namespace VirtualHole.Scraper
{
	public class ContentScraperSettings
	{
		public bool IsDevMode { get; set; } = false;

		public int IterationGapAmount { get; set; } = 3600;
		public bool UseProxies { get; set; } = true;

		public string ConnectionString { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;

		public ProxyPool ProxyPool { get; set; } = null;

		public string GetRootDatabaseName()
		{
			return IsDevMode ? VirtualHoleDBClient.devRootDatabaseName : 
				VirtualHoleDBClient.prodRootDatabaseName;
		}
	}
}
