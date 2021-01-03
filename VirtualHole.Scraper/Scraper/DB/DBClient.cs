using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Midnight;
using Midnight.Logs;
using VirtualHole.DB;
using VirtualHole.DB.Creators;
using VirtualHole.DB.Contents;

using ScraperContentClient = VirtualHole.Scraper.Contents.ContentClient;
using ScraperCreatorClient = VirtualHole.Scraper.Creators.CreatorClient;

namespace VirtualHole.Scraper
{
	public class DBClient
	{
		public ScraperContentClient Contents { get; private set; } = null;
		public ScraperCreatorClient Creators { get; private set; } = null;

		public DBClient(ScraperClient scraperClient, VirtualHoleDBClient dbClient)
		{
			Contents = new ScraperContentClient(scraperClient, dbClient);
			Creators = new ScraperCreatorClient(scraperClient, dbClient);
		}

		public async Task WriteToVideosDBUsingCreatorsDBAsync(
			bool incremental = false, CancellationToken cancellationToken = default)
		{
			IEnumerable<Creator> creators = await Creators.GetAllFromDBAsync(cancellationToken);

			MLog.Log(nameof(DBClient), $"Found {creators.Count()} creators on the database...");

			List<Content> contents = new List<Content>();
			contents.AddRange(await Contents.ScrapeAsync(creators, incremental, cancellationToken));
			MLog.Log(nameof(DBClient), $"Scraped a total of {contents.Count} contents.");

			string contentsPath = Path.Combine(PathUtilities.GetApplicationPath(), "data/results/contents.json");
			string contentsDirectory = Path.GetDirectoryName(contentsPath);
			Directory.CreateDirectory(contentsDirectory);
			File.WriteAllText(contentsPath, JsonConvert.SerializeObject(contents));
			MLog.Log(nameof(DBClient), $"Saved contents.json to {contentsPath}.");

			if(contents.Count > 0) { await Contents.WriteToDBAsync(contents, incremental, cancellationToken); }
			MLog.Log(nameof(DBClient), $"Wrote a total of {contents.Count} content to database, during this iteration!");
		}
	}
}
