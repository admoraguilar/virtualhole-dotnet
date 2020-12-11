using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace VirtualHole.DB
{
	public class FindSettingsComposition<T> : FindSettings<T>
	{
		public IEnumerable<FilterDefinition<T>> Filters = null;
		public IEnumerable<SortDefinition<T>> Sorts = null;

		internal override FilterDefinition<T> Filter 
		{
			get {
				BsonDocument bson = new BsonDocument();
				if(Filters != null) { 
					foreach(FilterDefinition<T> filter in Filters) {
						bson.Merge(filter.ToBsonDocument());
					}
				}
				return bson;
			}
		}

		internal override SortDefinition<T> Sort
		{
			get {
				BsonDocument bson = new BsonDocument();
				if(Sorts != null) {
					foreach(SortDefinition<T> sort in Sorts) {
						bson.Merge(sort.ToBsonDocument());
					}
				}
				return bson;
			}
		}
	}
}