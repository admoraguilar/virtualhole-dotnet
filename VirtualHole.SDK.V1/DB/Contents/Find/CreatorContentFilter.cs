using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Midnight;
using VirtualHole.DB.Creators;

namespace VirtualHole.DB.Contents
{
	public class CreatorContentFilter : FindFilter
	{
		public bool IsCreatorsInclude { get; set; } = true;
		public List<string> CreatorIds { get; set; } = new List<string>();

		internal override IEnumerable<Type> ConflictingTypes => new Type[] {
			typeof(CreatorRelatedContentFilter)
		};

		internal override BsonDocument Document
		{
			get {
				BsonDocument bson = new BsonDocument();

				if(CreatorIds != null) {
					bson.Add(
						$"{nameof(Content.Creator).ToCamelCase()}.{nameof(CreatorSimple.Id).ToCamelCase()}",
						new BsonDocument(IsCreatorsInclude ? "$in" : "$nin", new BsonArray(CreatorIds)));;
				}

				return bson;
			}
		}
	}
}
