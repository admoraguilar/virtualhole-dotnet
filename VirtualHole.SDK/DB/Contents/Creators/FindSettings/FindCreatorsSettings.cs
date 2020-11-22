using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace VirtualHole.DB.Contents.Creators
{
	public class FindCreatorsSettings : FindSettings<Creator>
	{
		public bool IsHidden = false;

		public bool IsCheckForAffiliations = false;
		public List<string> Affiliations = new List<string>();

		public bool IsGroup = false;

		public bool IsCheckForDepth = false;
		public int Depth = 0;

		internal override FilterDefinition<Creator> Filter => CreateFilterDocument();
		internal override SortDefinition<Creator> Sort => null;

		protected virtual BsonDocument CreateFilterDocument()
		{
			BsonDocument filter = new BsonDocument();

			filter.Add(nameof(Creator.IsHidden).ToCamelCase(), IsHidden);

			if(IsCheckForAffiliations) {
				filter.Add(nameof(Creator.Affiliations).ToCamelCase(), new BsonDocument("$all", new BsonArray(Affiliations)));
			}

			filter.Add(nameof(Creator.IsGroup).ToCamelCase(), IsGroup);

			if(IsCheckForDepth) { filter.Add(nameof(Creator.Depth).ToCamelCase(), 0); }

			return filter;
		}
	}
}
