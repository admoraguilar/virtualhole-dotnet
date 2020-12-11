
namespace VirtualHole.DB.Creators
{
	public class TwitterSocial : CreatorSocial
	{
		public override string SocialType => SocialTypes.Twitter;

		public int FollowersCount { get; set; }
	}
}
