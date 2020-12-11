using MongoDB.Bson;

namespace VirtualHole.DB
{
	public abstract class FindSettings<T>
	{
		public int BatchSize { get; set; } = 20;
		public int ResultsLimit { get; set; } = 500;
		public int Skip { get; set; } = 0;

		internal abstract BsonDocument FilterDocument { get; }
		internal abstract BsonDocument SortDocument { get; }
	}
}