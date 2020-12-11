using System.Collections.Generic;
using MongoDB.Bson;

namespace VirtualHole.DB
{
	public class FindSettingsComposition<T> : FindSettings<T>
	{
		public IEnumerable<FindSettings<T>> Filters = null;
		public IEnumerable<FindSettings<T>> Sorts = null;

		internal override BsonDocument FilterDocument 
		{
			get {
				BsonDocument bson = new BsonDocument();
				if(Filters != null) { 
					foreach(FindSettings<T> setting in Filters) {
						bson.Merge(setting.FilterDocument);
					}
				}
				return bson;
			}
		}

		internal override BsonDocument SortDocument
		{
			get {
				BsonDocument bson = new BsonDocument();
				if(Sorts != null) {
					foreach(FindSettings<T> sort in Sorts) {
						bson.Merge(sort.ToBsonDocument());
					}
				}
				return bson;
			}
		}
	}
}