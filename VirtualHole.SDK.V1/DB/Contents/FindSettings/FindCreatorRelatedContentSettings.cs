using System.Linq;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Midnight;
using VirtualHole.DB.Creators;

namespace VirtualHole.DB.Contents
{
	public class FindCreatorRelatedContentSettings : FindContentSettings
	{
		public List<Creator> Creators = new List<Creator>();

		internal override FilterDefinition<Content> Filter
		{
			get {
				BsonDocument bson = base.Filter.ToBsonDocument();

				IEnumerable<string> creatorIds = Creators.Select(c => c.Id);
				IEnumerable<string> creatorNames = Creators.Select(c => c.Name);
				IEnumerable<string> creatorSocialIds = Creators.SelectMany(c => c.Socials.Select(s => s.Id));
				IEnumerable<string> creatorSocialUrls = Creators.SelectMany(c => c.Socials.Select(s => s.Url));

				BsonArray andExpressions = new BsonArray();

				andExpressions.Add(
					new BsonDocument(
						nameof(Content.CreatorId).ToCamelCase(),
						new BsonDocument("$not", new BsonDocument("$in", new BsonArray(creatorIds)))));
				andExpressions.Add(
					new BsonDocument("$text",
						new BsonDocument("$search", Join(Concat(creatorNames, creatorSocialIds)))));

				bson.AddRange(new BsonDocument("$and", andExpressions));
				return bson;

				string Join(IEnumerable<string> values) =>
					string.Join(" ", values);

				IEnumerable<string> Concat(params IEnumerable<string>[] enumerables) =>
					enumerables.SelectMany(e => e);
			}
		}
	}
}
