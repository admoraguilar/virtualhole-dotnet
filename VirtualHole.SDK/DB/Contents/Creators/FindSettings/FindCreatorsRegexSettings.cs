using System.Collections.Generic;
using MongoDB.Bson;
using VirtualHole.Common;

namespace VirtualHole.DB.Contents.Creators
{
	public class FindCreatorsRegexSettings : FindCreatorsSettings
	{
		public List<string> SearchQueries = new List<string>();

		public bool IsCheckUniversalName = true;
		public bool IsCheckUniversalId = true;

		public bool IsCheckSocialName = true;
		public bool IsCheckSocialCustomKeywords = true;

		public bool IsCheckCustomKeywords = true;

		protected override BsonDocument CreateFilterDocument()
		{
			BsonDocument filter = new BsonDocument();

			if(SearchQueries.Count > 0) {
				BsonArray orArray = new BsonArray();
				foreach(string searchQuery in SearchQueries) {
					if(IsCheckUniversalName) {
						orArray.Add(new BsonDocument(nameof(Creator.UniversalName).ToCamelCase(), CreateRegexQuery(searchQuery)));
					}

					if(IsCheckUniversalId) {
						orArray.Add(new BsonDocument(nameof(Creator.UniversalId).ToCamelCase(), CreateRegexQuery(searchQuery)));
					}

					if(IsCheckSocialName) {
						orArray.Add(
							new BsonDocument($"{nameof(Creator.Socials).ToCamelCase()}.{nameof(Social.Name).ToCamelCase()}",
								CreateRegexQuery(searchQuery)));
					}

					if(IsCheckSocialCustomKeywords) {
						orArray.Add(
							new BsonDocument($"{nameof(Creator.Socials).ToCamelCase()}.{nameof(Social.CustomKeywords).ToCamelCase()}",
								new BsonDocument("$elemMatch", CreateRegexQuery(searchQuery))));
					}

					if(IsCheckCustomKeywords) {
						orArray.Add(
							new BsonDocument(nameof(Creator.CustomKeywords).ToCamelCase(),
								new BsonDocument("$elemMatch", CreateRegexQuery(searchQuery))));
					}
				}

				filter.Add("$or", orArray);
			}

			filter.AddRange(base.CreateFilterDocument());

			return filter;
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
