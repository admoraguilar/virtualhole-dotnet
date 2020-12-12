namespace VirtualHole.DB.Contents
{
	public class TwitterTweet : Content
	{
		public override string SocialType => SocialTypes.Twitter;
		public override string ContentType => ContentTypes.Blog;

		public string Text { get; set; } = string.Empty;
		public int HeartsCount { get; set; } = 0;
		public int RetweetsCount { get; set; } = 0;
	}
}