using System.Collections.Generic;
using MongoDB.Bson;
using Midnight;

namespace VirtualHole.DB.Contents
{
	public class FindContentSettings : FindSettings
	{
		public List<string> SocialType { get; set; } = new List<string>();
		public List<string> ContentType { get; set; } = new List<string>();
		public bool IsSortAscending { get; set; } = false;

		internal override BsonDocument FilterDocument
		{
			get {
				BsonDocument bson = new BsonDocument();

				BsonArray typeAndExpr = new BsonArray();
				if(SocialType != null && SocialType.Count > 0) {
					typeAndExpr.Add(new BsonDocument(
						nameof(Content.SocialType).ToCamelCase(),
						new BsonDocument("$in", new BsonArray(SocialType))));
				}

				if(ContentType != null && ContentType.Count > 0) {
					typeAndExpr.Add(new BsonDocument(
						nameof(Content.ContentType).ToCamelCase(),
						new BsonDocument("$in", new BsonArray(ContentType))));
				}

				bson.Add("$and", typeAndExpr);

				return bson;
			}
		}

		internal override BsonDocument SortDocument =>
			new BsonDocument() { { nameof(Content.CreationDate).ToCamelCase(), IsSortAscending ? 1 : -1 } };
	}
}
