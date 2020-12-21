using System.Collections.Generic;
using MongoDB.Bson;
using Midnight;

namespace VirtualHole.DB.Contents
{
	public class ContentsFilter : FindFilter
	{
		public bool IsSocialTypeInclude { get; set; } = true;
		public List<string> SocialType { get; set; } = new List<string>();

		public bool IsContentTypeInclude { get; set; } = true;
		public List<string> ContentType { get; set; } = new List<string>();

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

				if(typeAndExpr.Count > 0) {
					bson.Add("$and", typeAndExpr);
				}

				return bson;
			}
		}
	}
}
