﻿using System.Collections.Generic;
using MongoDB.Bson;
using Midnight;

namespace VirtualHole.DB.Creators
{
	public class FindCreatorsRegexSettings : FindCreatorsSettings
	{
		public List<string> SearchQueries { get; set; } = new List<string>();

		public bool IsCheckName { get; set; } = true;
		public bool IsCheckId { get; set; } = true;
		public bool IsCheckSocialName { get; set; } = true;
		//public bool IsCheckSocialCustomKeywords { get; set; } = true;
		//public bool IsCheckCustomKeywords { get; set; } = true;

		protected override BsonDocument CreateFilterDocument()
		{
			BsonDocument bson = new BsonDocument();

			if(SearchQueries.Count > 0) {
				BsonArray orArray = new BsonArray();
				foreach(string searchQuery in SearchQueries) {
					if(IsCheckName) {
						orArray.Add(new BsonDocument(nameof(Creator.Name).ToCamelCase(), CreateRegexQuery(searchQuery)));
					}

					if(IsCheckId) {
						orArray.Add(new BsonDocument(nameof(Creator.Id).ToCamelCase(), CreateRegexQuery(searchQuery)));
					}

					if(IsCheckSocialName) {
						orArray.Add(
							new BsonDocument($"{nameof(Creator.Socials).ToCamelCase()}.{nameof(CreatorSocial.Name).ToCamelCase()}",
								CreateRegexQuery(searchQuery)));
					}

					//if(IsCheckSocialCustomKeywords) {
					//	orArray.Add(
					//		new BsonDocument($"{nameof(Creator.Socials).ToCamelCase()}.{nameof(CreatorSocial.CustomKeywords).ToCamelCase()}",
					//			new BsonDocument("$elemMatch", CreateRegexQuery(searchQuery))));
					//}

					//if(IsCheckCustomKeywords) {
					//	orArray.Add(
					//		new BsonDocument(nameof(Creator.CustomKeywords).ToCamelCase(),
					//			new BsonDocument("$elemMatch", CreateRegexQuery(searchQuery))));
					//}
				}

				bson.Add("$or", orArray);
			}

			bson.AddRange(base.CreateFilterDocument());

			return bson;
		}

		private BsonDocument CreateRegexQuery(string searchQuery)
		{
			return new BsonDocument() {
				{ "$regex", $".*{searchQuery}.*" },
				{ "$options", "i" }
			};
		}
	}
}
