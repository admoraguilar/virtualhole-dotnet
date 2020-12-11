using System;

namespace VirtualHole.DB.Contents
{
	public class YouTubeBroadcast : YouTubeVideo
	{
		public override string ContentType => ContentTypes.Broadcast;

		public bool IsLive { get; set; } = false;
		public long ViewerCount { get; set; } = 0;
		public DateTimeOffset ScheduleDate { get; set; } = DateTimeOffset.MinValue;
	}
}