using System;
using MongoDB.Bson;

namespace VirtualHole.DB
{
	public abstract class FindSettings<T>
	{
		public int PageSize { get; set; } = 20;
		public int MaxPages { get; set; } = 30;
		public int Page { get; set; } = 1;

		public int GetBatchSize() => PageSize;
		public int GetResultsLimit() => PageSize * MaxPages;
		public int GetSkip() => Math.Min(GetResultsLimit(), (Page - 1) * PageSize);

		internal abstract BsonDocument FilterDocument { get; }
		internal abstract BsonDocument SortDocument { get; }
	}
}