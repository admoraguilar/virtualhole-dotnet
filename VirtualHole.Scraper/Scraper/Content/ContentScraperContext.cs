using System.Collections.Generic;
using VirtualHole.DB;
using VirtualHole.DB.Contents;
using VirtualHole.DB.Creators;

namespace VirtualHole.Scraper
{
	public class ContentScraperContext
	{
		public ContentScraperContext(
			ContentScraperSettings settings, ScraperClient scraper,
			VirtualHoleDBClient db)
		{
			InSettings = settings;
			InScraper = scraper;
			InDB = db;

			InIsIncremental = InSettings.IsStartIncremental;
		}

		public ContentScraperSettings InSettings { get; private set; }
		public ScraperClient InScraper { get; private set; }
		public VirtualHoleDBClient InDB { get; private set; }

		public List<Creator> InCreators { get; set; } = new List<Creator>();
		public bool InIsIncremental { get; set; } = false;

		public List<Content> OutResults { get; set; } = new List<Content>();
	}
}
