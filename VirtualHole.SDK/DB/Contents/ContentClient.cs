using MongoDB.Driver;

namespace VirtualHole.DB.Contents
{
	using Blogs;
	using Creators;
	using Videos;

	public class ContentClient
	{
		internal const string ContentDatabaseName = "content";

		public BlogClient Blogs { get; private set; } = null;
		public CreatorClient Creators { get; private set; } = null;
		public VideoClient Videos { get; private set; } = null;

		private IMongoDatabase database = null;

		internal ContentClient(IMongoClient client)
		{
			database = client.GetDatabase(ContentDatabaseName);

			Blogs = new BlogClient(database);
			Creators = new CreatorClient(database);
			Videos = new VideoClient(database);
		}
	}
}
