﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace VirtualHole.DB
{
	public abstract class FindSettings<T>
	{
		public DateTimeOffset Timestamp = DateTimeOffset.UtcNow;
		public string Locale = "en-US";

		public int BatchSize = 20;
		public int ResultsLimit = 500;
		public int Skip = 0;

		internal abstract FilterDefinition<T> Filter { get; }
		internal abstract SortDefinition<T> Sort { get; }

		public BsonDocument GetFilterDocument() => Filter.ToBsonDocument();
		public BsonDocument GetSortDocument() => Sort.ToBsonDocument();
	}
}