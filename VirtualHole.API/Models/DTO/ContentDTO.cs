using VirtualHole.DB.Contents;

namespace VirtualHole.API.Models
{
	public class ContentDTO
	{
		public Content Content { get; set; } = null;
		
		public string CreatorSocialId { get; set; } = string.Empty;
		public string CreatorSocialName { get; set; } = string.Empty;
		public string CreatorAvatarUrl { get; set; } = string.Empty;

		public string CreationDateDisplay { get; set; } = string.Empty;
		public string ScheduleDateDisplay { get; set; } = string.Empty;
	}
}