using MongoDB.Bson;
using MongoDB.Driver;

namespace VirtualHole.DB.Common
{
	public abstract class FindSettings<T>
	{
		public int BatchSize = 20;
		public int ResultsLimit = 500;
		public int Skip = 0;

		internal abstract FilterDefinition<T> Filter { get; }
		internal abstract SortDefinition<T> Sort { get; }

		public BsonDocument GetFilterDocument() => Filter.ToBsonDocument();
		public BsonDocument GetSortDocument() => Sort.ToBsonDocument();
	}
}