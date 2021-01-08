using System.Collections.Generic;
using System.Diagnostics;
using VirtualHole.DB;
using VirtualHole.DB.Contents;
using VirtualHole.DB.Creators;

namespace VirtualHole.Scraper
{
	public class ContentScraperContext
	{
		public ContentScraperContext(ScraperClient scraper, VirtualHoleDBClient db)
		{
			Debug.Assert(scraper != null);
			Debug.Assert(db != null);

			InScraper = scraper;
			InDB = db;
		}

		public ScraperClient InScraper { get; private set; }
		public VirtualHoleDBClient InDB { get; private set; }

		public List<Creator> InCreators { get; set; } = new List<Creator>();
		public List<Content> OutResults { get; set; } = new List<Content>();

		public void Reset()
		{
			InCreators = new List<Creator>();
			OutResults = new List<Content>();
		}
	}
}
