using System;
using System.Collections.Generic;
using Midnight;
using MongoDB.Bson;

namespace VirtualHole.DB.Contents
{
	public class BroadcastContentFilter : FindFilter
	{
		public bool IsLive { get; set; } = true;

		internal override IEnumerable<Type> ConflictingTypes => new Type[0];
		internal override BsonDocument Document
		{
			get {
				BsonDocument bson = new BsonDocument();
				bson.Add(nameof(YouTubeBroadcast.IsLive).ToCamelCase(), IsLive);
				return bson;
			}
		}
	}
}
