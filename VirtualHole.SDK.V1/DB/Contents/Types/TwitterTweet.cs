namespace VirtualHole.DB.Contents
{
	public class TwitterTweet : Content
	{
		public override string SocialType => SocialTypes.Twitter;
		public override string ContentType => ContentTypes.Blog;

		public string Text { get; set; } = string.Empty;
		public int HeartsCount { get; set; } = 0;
		public int RetweetsCount { get; set; } = 0;

		public override bool Equals(object obj)
		{
			TwitterTweet other = obj as TwitterTweet;
			if(other is null) { return false; }

			return base.Equals(obj) && Text == other.Text;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ (Text).GetHashCode();
		}
	}
}