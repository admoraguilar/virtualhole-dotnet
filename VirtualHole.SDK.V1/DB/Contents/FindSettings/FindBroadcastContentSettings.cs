using Midnight;
using MongoDB.Bson;
using MongoDB.Driver;

namespace VirtualHole.DB.Contents
{
	public class FindBroadcastContentSettings : FindContentSettings
	{
		public bool IsSortAscendingBySchedule = false;

		internal override BsonDocument FilterDocument
		{
			get {
				BsonDocument bson = base.FilterDocument;
				bson.Add(nameof(Content.ContentType).ToCamelCase(), ContentTypes.Broadcast);
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
