
namespace VirtualHole.DB.Creators
{
	public abstract class CreatorSocial
	{
		public abstract string SocialType { get; }

		public string Id { get; set; }
		public string Name { get; set; }
		public string Url { get; set; }
		public string AvatarUrl { get; set; }
	}
}
