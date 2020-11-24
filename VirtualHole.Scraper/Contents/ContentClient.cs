using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Logs;

namespace VirtualHole.Scraper
{
	using DB;
	using DB.Contents.Videos;
	using DB.Contents.Creators;

	using DBVideoClient = DB.Contents.Videos.VideoClient;
	using DBCreatorClient = DB.Contents.Creators.CreatorClient;

	public class ContentClientSettings
	{
		public string ConnectionString { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public ProxyPool ProxyPool { get; set; } = null;
	}

	public class ContentClient
	{
		public Contents.Videos.VideoClient Videos { get; private set; } = null;
		public CreatorClient Creators { get; private set; } = null;

		private ScraperClient scraperClient = null;
		private VirtualHoleDBClient dbClient = null;

		public ContentClient(ContentClientSettings settings)
		{
			scraperClient = new ScraperClient(settings.ProxyPool);
			dbClient = new VirtualHoleDBClient(settings.ConnectionString, settings.UserName, settings.Password);

			Videos = new ScraperVideoClient(scraperClient, dbClient);
			Creators = new ScraperCreatorClient(scraperClient, dbClient);
		}

		public async Task WriteToCreatorsDBAsync(
			IEnumerable<Creator> creators, CancellationToken cancellationToken = default)
		{
			await Creators.WriteToDBAsync(creators, cancellationToken);
		}

		public async Task WriteToVideosDBUsingCreatorsDBAsync(
			bool incremental = false, CancellationToken cancellationToken = default)
		{
			IEnumerable<Creator> creators = await Creators.GetAllFromDBAsync(cancellationToken);

			MLog.Log($"Found creators: {creators.Count()}");
			List<Video> videos = new List<Video>();
			videos.AddRange(await Videos.ScrapeAsync(creators, incremental, cancellationToken));

			if(videos.Count > 0) {
				await Videos.WriteToDBAsync(videos, incremental, cancellationToken);
			}
		}
	}
}
