using System.Collections.Generic;
using MongoDB.Bson;
using Midnight;

namespace VirtualHole.DB.Contents
{
	public class FindCreatorContentSettings : FindContentSettings
	{
		public bool IsCreatorsInclude { get; set; } = true;
		public List<string> CreatorIds { get; set; } = new List<string>();

		internal override BsonDocument FilterDocument
		{
			get {
				BsonDocument bson = new BsonDocument();

				if(CreatorIds != null) {
					bson.Add(
						nameof(Content.CreatorId).ToCamelCase(),
						new BsonDocument(IsCreatorsInclude ? "$in" : "$nin", new BsonArray(CreatorIds)));
				}

				return bson.Merge(base.FilterDocument);
			}
		}
	}
}
