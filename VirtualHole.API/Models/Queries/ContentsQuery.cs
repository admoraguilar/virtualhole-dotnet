using System.Collections.Generic;

namespace VirtualHole.API.Models
{
	public class ContentsQuery : PagedQuery
	{
		public bool IsSocialTypeInclude { get; set; } = true;
		public List<string> SocialType { get; set; } = new List<string>();

		public bool IsContentTypeInclude { get; set; } = true;
		public List<string> ContentType { get; set; } = new List<string>();

		public bool IsCreatorsInclude { get; set; } = true;
		public bool IsCreatorRelated { get; set; } = false;
		public List<string> CreatorIds { get; set; } = new List<string>();

		public bool IsCheckCreatorAffiliations { get; set; } = false;
		public bool IsAffiliationsAll { get; set; } = false;
		public bool IsAffiliationsInclude { get; set; } = true;
		public List<string> CreatorAffiliations { get; set; } = new List<string>();

		public bool IsSortAscending { get; set; } = false;
	}
}