using MongoDB.Driver;

namespace VirtualHole.DB
{
	internal static class ClientFactory
	{
		private static IMongoClient client = null;

		public static IMongoClient GetMongoClient(string connectionString)
		{
			if(client != null) { return client; }

			BsonConfig.Initialize();
			return client = new MongoClient(connectionString);
		}
	}
}