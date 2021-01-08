using System.Diagnostics;
using MongoDB.Driver;

namespace VirtualHole.DB
{
	internal static class MongoDBClientFactory
	{
		private static IMongoClient client = null;

		public static IMongoClient GetMongoClient(string connectionString)
		{
			Debug.Assert(!string.IsNullOrEmpty(connectionString));

			if(client != null) { return client; }
			return client = new MongoClient(connectionString);
		}
	}
}