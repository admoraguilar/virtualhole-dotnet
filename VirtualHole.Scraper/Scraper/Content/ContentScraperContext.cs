using System.Collections.Generic;
using VirtualHole.DB;
using VirtualHole.DB.Contents;
using VirtualHole.DB.Creators;

namespace VirtualHole.Scraper
{
	public class ContentScraperContext
	{
		public ContentScraperContext(ScraperClient scraper, VirtualHoleDBClient db)
		{
			InScraper = scraper;
			InDB = db;
		}

		public ScraperClient InScraper { get; private set; }
		public VirtualHoleDBClient InDB { get; private set; }

		public List<Creator> InCreators { get; set; } = new List<Creator>();
		public bool InIsIncremental { get; set; } = false;

		public List<Content> OutAllResults { get; set; } = new List<Content>();
		public List<Content> OutNewResults { get; set; } = new List<Content>();
		public List<Content> OutDeletedResults { get; set; } = new List<Content>();
	}
}
