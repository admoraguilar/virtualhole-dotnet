﻿using System.Collections.Generic;
using MongoDB.Bson;
using Midnight;

namespace VirtualHole.DB.Creators
{
	public class CreatorsFilter : FindFilter
	{
		public bool IsHidden { get; set; } = false;

		public bool IsCheckForIsGroup { get; set; } = true;
		public bool IsGroup { get; set; } = false;

		public bool IsCheckForDepth { get; set; } = false;
		public int Depth { get; set; } = 0;

		public bool IsCheckForAffiliations { get; set; } = false;
		public bool IsAffiliationsAll { get; set; } = false;
		public bool IsAffiliationsInclude { get; set; } = true;
		public List<string> Affiliations { get; set; } = new List<string>();

		internal override BsonDocument Document 
		{
			get {
				BsonDocument bson = new BsonDocument();

				bson.Add(nameof(Creator.IsHidden).ToCamelCase(), IsHidden);

				if(IsCheckForAffiliations) {
					string queryOperator = string.Empty;

					if(IsAffiliationsAll) {  queryOperator = "$all"; } 
					else { queryOperator = IsAffiliationsInclude ? "$in" : "$nin"; }
					
					bson.Add(nameof(Creator.Affiliations).ToCamelCase(), new BsonDocument(queryOperator, new BsonArray(Affiliations)));
				}

				if(IsCheckForIsGroup) {
					bson.Add(nameof(Creator.IsGroup).ToCamelCase(), IsGroup);
				}

				if(IsCheckForDepth) { 
					bson.Add(nameof(Creator.Depth).ToCamelCase(), 0); 
				}

				return bson;
			}
		}
	}
}
