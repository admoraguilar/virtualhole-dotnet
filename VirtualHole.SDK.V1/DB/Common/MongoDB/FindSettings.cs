using System;
using MongoDB.Bson;

namespace VirtualHole.DB
{
	public abstract class FindSettings<T>
	{
		public int PageSize 
		{
			get => pageSize; 
			set { pageSize = Math.Max(0, value); }
		}
		private int pageSize = 20;

		public int MaxPages 
		{
			get => maxPages; 
			set { maxPages = Math.Max(0, value); }
		}
		private int maxPages = 30;

		public int Page 
		{
			get => page;
			set { page = Math.Max(1, value); }
		}
		private int page = 1;

		public int GetBatchSize() => PageSize;
		public int GetResultsLimit() => PageSize * MaxPages;
		public int GetSkip() => Math.Min(GetResultsLimit(), (Page - 1) * PageSize);

		internal abstract BsonDocument FilterDocument { get; }
		internal abstract BsonDocument SortDocument { get; }
	}
}