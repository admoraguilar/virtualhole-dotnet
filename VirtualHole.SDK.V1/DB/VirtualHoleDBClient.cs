using MongoDB.Driver;
using VirtualHole.DB.Contents;
using VirtualHole.DB.Creators;

namespace VirtualHole.DB
{
	public class VirtualHoleDBClient
	{
		internal const string RootDatabaseName = "virtualhole-prod";

		public ContentClient Contents { get; private set; } = null;
		public CreatorClient Creators { get; private set; } = null;

		private IMongoClient _client = null;
		private IMongoDatabase _rootDatabase = null;

		public VirtualHoleDBClient(
			string connectionString, string userName,
			string password)
		{
			string connection = connectionString
				.Replace("<username>", userName)
				.Replace("<password>", password);

			_client = ClientFactory.GetMongoClient(connection);
			_rootDatabase = _client.GetDatabase(RootDatabaseName);

			Contents = new ContentClient(_rootDatabase);
			Creators = new CreatorClient(_rootDatabase);
		}
	}
}
