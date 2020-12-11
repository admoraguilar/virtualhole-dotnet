using System.Linq;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Midnight;
using VirtualHole.DB.Creators;

namespace VirtualHole.DB.Contents
{
	public class FindCreatorContentSettings : FindContentSettings
	{
		public List<Creator> Creators = new List<Creator>();

		internal override FilterDefinition<Content> Filter
		{
			get {
				BsonDocument bson = base.Filter.ToBsonDocument();

				if(Creators != null) {
					bson.Add(
						nameof(Content.CreatorId).ToCamelCase(),
						new BsonDocument("$in", new BsonArray(Creators.Select(c => c.Id))));
				}

				return bson;
			}
		}
	}
}
