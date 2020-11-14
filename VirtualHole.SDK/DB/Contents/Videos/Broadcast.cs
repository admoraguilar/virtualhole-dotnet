using System;

namespace VirtualHole.DB.Contents.Videos
{
	[Serializable]
	public class Broadcast : Video
	{
		public bool IsLive = false;
		public long ViewerCount = 0;
		public DateTimeOffset ScheduleDate = DateTimeOffset.MinValue;
		public string ScheduleDateDisplay = string.Empty;
	}
}
