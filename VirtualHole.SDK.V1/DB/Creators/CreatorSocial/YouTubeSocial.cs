
namespace VirtualHole.DB.Creators
{
	public class YouTubeSocial : CreatorSocial
	{
		public override string SocialType => SocialTypes.YouTube;

		public int SubscribersCount { get; set; } = 0;
		public int TotalViewsCount { get; set; } = 0;
	}
}
