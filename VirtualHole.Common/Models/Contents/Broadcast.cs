using System;

namespace VirtualHole.Common
{
	[Serializable]
	public class Broadcast : Video
	{
		public bool IsLive = false;
		public long ViewerCount = 0;
		public DateTimeOffset Schedule = DateTimeOffset.MinValue;
	}
}
