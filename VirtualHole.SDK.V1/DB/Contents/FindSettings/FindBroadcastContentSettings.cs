using MongoDB.Bson;
using MongoDB.Driver;

namespace VirtualHole.DB.Contents
{
	public class FindBroadcastContentSettings : FindContentSettings
	{
		public bool IsSortAscendingBySchedule = false;

		internal override FilterDefinition<Content> Filter
		{
			get {
				BsonDocument bson = base.Filter.ToBsonDocument();
				bson.Add(nameof(Content.ContentType), "Broadcast");
				return bson;
			}
		}

		internal override SortDefinition<Content> Sort 
		{
			get {
				BsonDocument bson = new BsonDocument() { 
					{ nameof(YouTubeBroadcast.ScheduleDate), IsSortAscendingBySchedule ? 1 : -1 } 
				};
				return bson;
			}
		}
	}
}
