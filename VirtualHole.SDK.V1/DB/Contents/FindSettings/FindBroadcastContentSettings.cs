using System.Collections.Generic;
using MongoDB.Bson;
using Midnight;

namespace VirtualHole.DB.Contents
{
	public class FindBroadcastContentSettings : FindContentSettings
	{
		public bool IsSortAscendingBySchedule = false;

		internal override BsonDocument FilterDocument
		{
			get {
				ContentType = new List<string>() { ContentTypes.Broadcast };
				BsonDocument bson = base.FilterDocument;
				return bson;
			}
		}

		internal override BsonDocument SortDocument 
		{
			get {
				BsonDocument bson = new BsonDocument() { 
					{ nameof(YouTubeBroadcast.ScheduleDate).ToCamelCase(), IsSortAscendingBySchedule ? 1 : -1 } 
				};
				return bson;
			}
		}
	}
}
