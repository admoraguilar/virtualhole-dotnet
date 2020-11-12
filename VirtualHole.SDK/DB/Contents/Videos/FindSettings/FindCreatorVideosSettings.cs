using System.Linq;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using VirtualHole.Common;

namespace VirtualHole.DB.Contents.Videos
{
	using Common;
	using Creators;

	public class FindCreatorVideosSettings<T> : FindVideosSettings<T> where T : Video
	{
		public bool IsBroadcast = false;
		public bool IsLive = true;
		public List<Creator> Creators = new List<Creator>();

		internal override FilterDefinition<T> Filter
		{
			get
			{
				BsonDocument filter = new BsonDocument();

				if(IsBroadcast) {
					filter.Add(DBUtilities.TField, "Broadcast");
					filter.Add(nameof(Broadcast.IsLive).ToCamelCase(), IsLive);
				} else {
					filter.Add(
						DBUtilities.TField,
						new BsonDocument("$not",
							new BsonDocument("$elemMatch",
								new BsonDocument("$eq", "Broadcast"))));
				}

				if(Creators != null) {
					filter.Add(
						nameof(Content.CreatorIdUniversal).ToCamelCase(), 
						new BsonDocument("$in", new BsonArray(Creators.Select(c => c.UniversalId))));
				}

				return filter;
			}
		}
	}
}