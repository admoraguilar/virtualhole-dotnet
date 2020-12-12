using System.Collections.Generic;
using MongoDB.Bson;
using Midnight;

namespace VirtualHole.DB.Creators
{
	public class FindCreatorsSettings : FindSettings
	{
		public bool IsHidden { get; set; } = false;

		public bool IsCheckForIsGroup { get; set; } = true;
		public bool IsGroup { get; set; } = false;

		public bool IsCheckForDepth { get; set; } = false;
		public int Depth { get; set; } = 0;

		public bool IsCheckForAffiliations { get; set; } = false;
		public List<string> Affiliations { get; set; } = new List<string>();

		internal override BsonDocument FilterDocument
		{
			get {
				BsonDocument bson = new BsonDocument();

				bson.Add(nameof(Creator.IsHidden).ToCamelCase(), IsHidden);

				if(IsCheckForAffiliations) {
					bson.Add(nameof(Creator.Affiliations).ToCamelCase(), new BsonDocument("$all", new BsonArray(Affiliations)));
				}

				if(IsCheckForIsGroup) {
					bson.Add(nameof(Creator.IsGroup).ToCamelCase(), IsGroup);
				}

				if(IsCheckForDepth) { bson.Add(nameof(Creator.Depth).ToCamelCase(), 0); }

				return bson;
			}
		}

		internal override BsonDocument SortDocument => null;
	}
}
