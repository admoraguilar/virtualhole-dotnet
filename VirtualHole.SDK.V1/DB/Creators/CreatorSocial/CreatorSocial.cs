
namespace VirtualHole.DB.Creators
{
	public abstract class CreatorSocial
	{
		public abstract string SocialType { get; }

		public string Id { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Url { get; set; } = string.Empty;
		public string AvatarUrl { get; set; } = string.Empty;
	}
}
