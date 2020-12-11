using System.Collections.Generic;
using MongoDB.Bson;

namespace VirtualHole.DB
{
	public class FindSettingsComposition : FindSettings
	{
		public IEnumerable<FindSettings> Filters { get; set; } = null;
		public IEnumerable<FindSettings> Sorts { get; set; } = null;

		internal override BsonDocument FilterDocument 
		{
			get {
				BsonDocument bson = new BsonDocument();
				if(Filters != null) { 
					foreach(FindSettings setting in Filters) {
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
					foreach(FindSettings sort in Sorts) {
						bson.Merge(sort.ToBsonDocument());
					}
				}
				return bson;
			}
		}
	}
}