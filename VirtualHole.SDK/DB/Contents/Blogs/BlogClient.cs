using MongoDB.Driver;

namespace VirtualHole.DB.Contents.Blogs
{
	public class BlogClient
	{
		internal const string BlogsCollectionName = "blogs";

		private IMongoDatabase database = null;

		internal BlogClient(IMongoDatabase database)
		{
			this.database = database;
		}
	}
}
