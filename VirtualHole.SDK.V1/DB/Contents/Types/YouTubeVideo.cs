using System;

namespace VirtualHole.DB.Contents
{
	public class YouTubeVideo : Content
	{
		public override string SocialType => SocialTypes.YouTube;
		public override string ContentType => ContentTypes.Video;

		public string ThumbnailUrl { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public TimeSpan Duration { get; set; } = TimeSpan.MinValue;
		public long ViewsCount { get; set; } = 0;
		public long LikesCount { get; set; } = 0;
		public long DislikesCount { get; set; } = 0;
	}
}
