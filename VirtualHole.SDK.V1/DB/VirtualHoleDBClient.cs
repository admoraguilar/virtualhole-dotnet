using System.Diagnostics;
using MongoDB.Driver;
using VirtualHole.DB.Contents;
using VirtualHole.DB.Creators;

namespace VirtualHole.DB
{
	public class VirtualHoleDBClient
	{
		public const string prodRootDatabaseName = "virtualhole-prod";
		public const string devRootDatabaseName = "virtualhole-dev";

		public ContentsClient Contents { get; private set; } = null;
		public CreatorsClient Creators { get; private set; } = null;

		private IMongoClient _client = null;
		private IMongoDatabase _rootDatabase = null;

		public VirtualHoleDBClient(
			string connectionString, string userName,
			string password) : this(
				string.Empty, connectionString, 
				userName, password) { }

		public VirtualHoleDBClient(
			string rootDatabaseName, string connectionString, 
			string userName, string password)
		{
			Debug.Assert(!string.IsNullOrEmpty(connectionString));
			Debug.Assert(!string.IsNullOrEmpty(userName));
			Debug.Assert(!string.IsNullOrEmpty(password));

			if(string.IsNullOrEmpty(rootDatabaseName)) {
				rootDatabaseName = prodRootDatabaseName;
			}

			connectionString = connectionString
				.Replace("<username>", userName)
				.Replace("<password>", password);

			_client = MongoDBClientFactory.GetMongoClient(connectionString);
			_rootDatabase = _client.GetDatabase(rootDatabaseName);

			Contents = new ContentsClient(_rootDatabase);
			Creators = new CreatorsClient(_rootDatabase);
		}
	}
}
