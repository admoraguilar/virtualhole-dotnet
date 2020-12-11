using MongoDB.Bson;
using Midnight;

namespace VirtualHole.DB.Contents
{
	public class FindContentSettings : FindSettings<Content>
	{
		public string SocialType { get; set; } = string.Empty;
		public string ContentType { get; set; } = string.Empty;
		public bool IsSortAscending { get; set; } = false;

		internal override BsonDocument FilterDocument
		{
			get {
				BsonDocument bson = new BsonDocument();

				if(!string.IsNullOrEmpty(SocialType)) {
					bson.Add(nameof(Content.SocialType).ToCamelCase(), SocialType);
				}

				if(!string.IsNullOrEmpty(ContentType)) {
					bson.Add(nameof(Content.ContentType).ToCamelCase(), ContentType);
				}

				return bson;
			}
		}

		internal override BsonDocument SortDocument =>
			new BsonDocument() { { nameof(Content.CreationDate).ToCamelCase(), IsSortAscending ? 1 : -1 } };
	}
}
