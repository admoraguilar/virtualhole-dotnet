using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Midnight;

namespace VirtualHole.DB.Contents
{
	public class BroadcastContentFilter : FindFilter
	{
		public bool IsLive { get; set; } = true;
		public DateTimeOffset UntilDate { get; set; } = DateTimeOffset.UtcNow;

		internal override IEnumerable<Type> ConflictingTypes => new Type[0];
		internal override BsonDocument Document
		{
			get {
				BsonDocument bson = new BsonDocument();

				bson.Add(nameof(YouTubeBroadcast.IsLive).ToCamelCase(), IsLive);				
				if(UntilDate != DateTimeOffset.UtcNow) {
					bson.Add(
						nameof(YouTubeBroadcast.ScheduleDate).ToCamelCase(),
						new BsonDocument("$lte", UntilDate.ToString("O")));
				}
				
				return bson;
			}
		}
	}
}
