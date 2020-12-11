using MongoDB.Driver;

namespace VirtualHole.DB
{
	public abstract class FindSettings<T>
	{
		public int BatchSize { get; set; } = 20;
		public int ResultsLimit { get; set; } = 500;
		public int Skip { get; set; } = 0;

		internal abstract FilterDefinition<T> Filter { get; }
		internal abstract SortDefinition<T> Sort { get; }
	}
}