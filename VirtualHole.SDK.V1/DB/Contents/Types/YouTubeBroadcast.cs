using System;

namespace VirtualHole.DB.Contents
{
	public class YouTubeBroadcast : YouTubeVideo
	{
		public override string ContentType => ContentTypes.Broadcast;

		public bool IsLive { get; set; } = false;
		public long ViewerCount { get; set; } = 0;
		public DateTimeOffset ScheduleDate { get; set; } = DateTimeOffset.MinValue;

		public override bool Equals(object obj)
		{
			YouTubeBroadcast other = obj as YouTubeBroadcast;
			if(other is null) { return false; }

			return base.Equals(obj) && IsLive == other.IsLive;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ (IsLive).GetHashCode();
		}
	}
}