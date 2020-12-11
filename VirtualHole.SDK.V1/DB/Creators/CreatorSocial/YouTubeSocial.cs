
namespace VirtualHole.DB.Creators
{
	public class YouTubeSocial : CreatorSocial
	{
		public override string SocialType => SocialTypes.YouTube;
		
		public int SubscribersCount { get; set; }
		public int TotalViewsCount { get; set; }
	}
}
