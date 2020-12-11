using System;

namespace VirtualHole.DB.Contents
{
	public class YouTubeVideo : Content
	{
		public override string SocialType => SocialTypes.YouTube;
		public override string ContentType => ContentTypes.Video;

		public string ThumbnailUrl { get; set; }
		public string Description { get; set; }
		public TimeSpan Duration { get; set; }
		public int ViewsCount { get; set; }
		public int LikesCount { get; set; }
		public int DislikesCount { get; set; }
	}
}