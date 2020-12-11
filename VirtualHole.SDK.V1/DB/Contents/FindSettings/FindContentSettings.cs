using MongoDB.Bson;
using MongoDB.Driver;

namespace VirtualHole.DB.Contents
{
	public class FindContentSettings : FindSettings<Content>
	{
		public string SocialType { get; set; } = string.Empty;
		public string ContentType { get; set; } = string.Empty;
		public bool IsSortAscending { get; set; } = false;

		internal override FilterDefinition<Content> Filter
		{
			get {
				BsonDocument bson = new BsonDocument();

				if(!string.IsNullOrEmpty(SocialType)) {
					bson.Add(nameof(Content.SocialType), SocialType);
				}

				if(!string.IsNullOrEmpty(ContentType)) {
					bson.Add(nameof(Content.ContentType), ContentType);
				}

				return bson;
			}
		}

		internal override SortDefinition<Content> Sort =>
			new BsonDocument() { { nameof(Content.CreationDate), IsSortAscending ? 1 : -1 } };
	}
}
