using MongoDB.Bson;
using MongoDB.Driver;
using Midnight;

namespace VirtualHole.DB.Contents.Videos
{
	public abstract class FindVideosSettings<T> : FindSettings<T> where T : Video
	{
		public SortMode SortMode = SortMode.None;
		public bool isSortAscending = true;

		internal override SortDefinition<T> Sort 
		{
			get 
			{
				if(SortMode == SortMode.ByCreationDate) { return Sort(nameof(Content.CreationDate).ToCamelCase()); } 
				else if(SortMode == SortMode.BySchedule) { return Sort(nameof(Broadcast.ScheduleDate).ToCamelCase()); } 
				else { return null; }

				BsonDocument Sort(string fieldName) =>
					new BsonDocument {
						{ fieldName, isSortAscending ? 1 : -1 }
					};
			}
		}
	}
}
