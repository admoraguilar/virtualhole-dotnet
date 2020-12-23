using MongoDB.Bson;
using Midnight;

namespace VirtualHole.DB.Contents
{
	public class ContentSort : FindSort
	{
		public SortMode SortMode { get; set; } = SortMode.ByCreationDate;
		public bool IsSortAscending { get; set; } = false;

		internal override BsonDocument Document
		{
			get {
				BsonDocument bson = new BsonDocument();
				
				if(SortMode == SortMode.ByCreationDate) {
					bson.Add(nameof(Content.CreationDate).ToCamelCase(), IsSortAscending ? 1 : -1);
				} else {
					bson.Add(nameof(YouTubeBroadcast.ScheduleDate).ToCamelCase(), IsSortAscending ? 1 : -1);
				}

				return bson;
			}
		}
	}
}
