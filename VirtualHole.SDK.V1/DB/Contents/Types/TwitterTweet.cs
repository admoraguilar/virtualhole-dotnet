namespace VirtualHole.DB.Contents
{
	public class TwitterTweet : Content
	{
		public override string SocialType => SocialTypes.Twitter;
		public override string ContentType => ContentTypes.Blog;

		public string Text;
		public int HeartsCount;
		public int RetweetsCount;
	}
}