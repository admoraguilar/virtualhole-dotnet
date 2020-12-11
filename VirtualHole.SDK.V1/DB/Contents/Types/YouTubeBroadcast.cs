using System;

namespace VirtualHole.DB.Contents
{
	public class YouTubeBroadcast : YouTubeVideo
	{
		public override string ContentType => ContentTypes.Broadcast;

		public bool IsLive { get; set; }
		public long ViewerCount { get; set; }
		public DateTimeOffset ScheduleDate { get; set; }
	}
}