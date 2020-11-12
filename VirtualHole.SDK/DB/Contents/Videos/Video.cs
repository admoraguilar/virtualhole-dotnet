using System;

namespace VirtualHole.DB.Contents.Videos
{
	[Serializable]
	public class Video : Content
	{
		public string ThumbnailUrl = string.Empty;
		public string Description = string.Empty;
		public TimeSpan Duration = TimeSpan.Zero;
		public long ViewCount = 0;
	}
}
