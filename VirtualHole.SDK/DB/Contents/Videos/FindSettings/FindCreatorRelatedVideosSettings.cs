using System.Linq;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace VirtualHole.DB.Contents.Videos
{
	using Creators;

	public class FindCreatorRelatedVideosSettings<T> : FindVideosSettings<T> where T : Video
	{
		public bool IsBroadcast = false;
		public bool IsLive = true;
		public List<Creator> Creators = new List<Creator>();

		internal override FilterDefinition<T> Filter 
		{
			get
			{
				IEnumerable<string> creatorNames = Creators.Select(c => c.UniversalName);
				IEnumerable<string> creatorIds = Creators.SelectMany(c => c.Socials.Select(s => s.Id));
				IEnumerable<string> creatorIdsUniversal = Creators.Select(c => c.UniversalId);
				IEnumerable<string> creatorUrls = Creators.SelectMany(c => c.Socials.Select(s => s.Url));
				IEnumerable<string> creatorCustomKeywords = Creators.SelectMany(c => c.Socials.SelectMany(s => s.CustomKeywords).Concat(c.CustomKeywords));

				BsonArray andExpressions = new BsonArray();

				if(IsBroadcast) {
					andExpressions.Add(new BsonDocument(DBUtilities.TField, "Broadcast"));
					andExpressions.Add(new BsonDocument(nameof(Broadcast.IsLive).ToCamelCase(), IsLive));
				} else {
					andExpressions.Add(
						new BsonDocument(
							DBUtilities.TField,
							new BsonDocument("$not",
								new BsonDocument("$elemMatch",
									new BsonDocument("$eq", "Broadcast")))));
				}

				andExpressions.Add(
					new BsonDocument(
						nameof(Content.CreatorIdUniversal).ToCamelCase(),
						new BsonDocument("$not", new BsonDocument("$in", new BsonArray(creatorIdsUniversal)))));
				andExpressions.Add(
					new BsonDocument("$text", 
						new BsonDocument("$search", Join(Concat(creatorNames, creatorIds, creatorCustomKeywords)))));

				BsonDocument filter = new BsonDocument("$and", andExpressions);
				return filter;

				string Join(IEnumerable<string> values) =>
					string.Join(" ", values);

				IEnumerable<string> Concat(params IEnumerable<string>[] enumerables) =>
					enumerables.SelectMany(e => e);
			}
		}
	}
}