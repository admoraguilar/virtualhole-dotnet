using System.Linq;
using System.Collections.Generic;
using MongoDB.Bson;
using Midnight;
using VirtualHole.DB.Creators;

namespace VirtualHole.DB.Contents
{
	public class FindCreatorContentSettings : FindContentSettings
	{
		public List<Creator> Creators = new List<Creator>();

		internal override BsonDocument FilterDocument
		{
			get {
				BsonDocument bson = base.FilterDocument;

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
