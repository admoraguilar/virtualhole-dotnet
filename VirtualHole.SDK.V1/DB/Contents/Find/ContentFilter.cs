using System.Collections.Generic;
using MongoDB.Bson;
using Midnight;
using VirtualHole.DB.Creators;

namespace VirtualHole.DB.Contents
{
	public class ContentFilter : FindFilter
	{
		public bool IsSocialTypeInclude { get; set; } = true;
		public List<string> SocialType { get; set; } = new List<string>();

		public bool IsContentTypeInclude { get; set; } = true;
		public List<string> ContentType { get; set; } = new List<string>();

		public bool IsCheckForAffiliations { get; set; } = false;
		public bool IsAffiliationsAll { get; set; } = false;
		public bool IsAffiliationsInclude { get; set; } = true;
		public List<string> Affiliations { get; set; } = new List<string>();

		internal override BsonDocument Document
		{
			get {
				BsonDocument bson = new BsonDocument();

				BsonArray typeAndExpr = new BsonArray();
				if(SocialType != null && SocialType.Count > 0) {
					typeAndExpr.Add(new BsonDocument(
						nameof(Content.SocialType).ToCamelCase(),
						new BsonDocument(IsSocialTypeInclude ? "$in" : "$nin", new BsonArray(SocialType))));
				}

				if(ContentType != null && ContentType.Count > 0) {
					typeAndExpr.Add(new BsonDocument(
						nameof(Content.ContentType).ToCamelCase(),
						new BsonDocument(IsContentTypeInclude ? "$in" : "$nin", new BsonArray(ContentType))));
				}

				if(IsCheckForAffiliations) {
					string queryOperator = string.Empty;

					if(IsAffiliationsAll) {  queryOperator = "$all"; } 
					else { queryOperator = IsAffiliationsInclude ? "$in" : "$nin"; }

					typeAndExpr.Add(new BsonDocument(
						$"{nameof(Content.Creator).ToCamelCase()}.{nameof(CreatorSimple.Affiliations).ToCamelCase()}", 
						new BsonDocument(queryOperator, new BsonArray(Affiliations))));
				}

				if(typeAndExpr.Count > 0) {
					bson.Add("$and", typeAndExpr);
				}

				return bson;
			}
		}
	}
}
