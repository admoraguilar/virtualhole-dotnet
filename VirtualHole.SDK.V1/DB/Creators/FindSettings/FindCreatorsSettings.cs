using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Midnight;

namespace VirtualHole.DB.Creators
{
	public class FindCreatorsSettings : FindSettings<Creator>
	{
		public bool IsHidden { get; set; } = false;

		public bool IsGroup { get; set; } = false;

		public bool IsCheckForDepth { get; set; } = false;
		public int Depth { get; set; } = 0;

		public bool IsCheckForAffiliations { get; set; } = false;
		public List<string> Affiliations { get; set; } = new List<string>();

		internal override FilterDefinition<Creator> Filter => CreateFilterDocument();
		internal override SortDefinition<Creator> Sort => null;

		protected virtual BsonDocument CreateFilterDocument()
		{
			BsonDocument bson = new BsonDocument();

			bson.Add(nameof(Creator.IsHidden).ToCamelCase(), IsHidden);

			if(IsCheckForAffiliations) {
				bson.Add(nameof(Creator.Affiliations).ToCamelCase(), new BsonDocument("$all", new BsonArray(Affiliations)));
			}

			bson.Add(nameof(Creator.IsGroup).ToCamelCase(), IsGroup);

			if(IsCheckForDepth) { bson.Add(nameof(Creator.Depth).ToCamelCase(), 0); }

			return bson;
		}
	}
}
