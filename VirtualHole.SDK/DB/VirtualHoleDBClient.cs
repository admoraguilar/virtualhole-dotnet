using MongoDB.Driver;
using VirtualHole.DB.Contents;

namespace VirtualHole.DB
{
	public class VirtualHoleDBClient
	{
		public ContentClient Contents { get; private set; } = null;

		private IMongoClient client = null;

		public VirtualHoleDBClient(
			string connectionString, string userName, 
			string password)
		{
			string connection = connectionString
				.Replace("<username>", userName)
				.Replace("<password>", password);

			client = ClientFactory.GetMongoClient(connection);
			Contents = new ContentClient(client);
		}
	}
}
