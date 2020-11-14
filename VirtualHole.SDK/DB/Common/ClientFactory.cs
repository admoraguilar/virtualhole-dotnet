using MongoDB.Driver;

namespace VirtualHole.DB.Common
{
	internal static class ClientFactory
	{
		private static IMongoClient client = null;

		public static IMongoClient GetMongoClient(string connectionString)
		{
			if(client != null) { return client; }
			BsonConfig.SetConvention();
			return client = new MongoClient(connectionString);
		}
	}
}