using System.Collections.Generic;

namespace VirtualHole.API.Models
{
	public class CreatorQuery : PagedQuery
	{
		public string Search { get; set; } = string.Empty;
		
		public bool IsHidden { get; set; } = false;
		public bool IsCheckForIsGroup { get; set; } = true;
		public bool IsGroup { get; set; } = false;

		public bool IsCheckForDepth { get; set; } = false;
		public int Depth { get; set; } = 0;

		public bool IsCheckForAffiliations { get; set; } = false;
		public bool IsAffiliationsAll { get; set; } = false;
		public bool IsAffiliationsInclude { get; set; } = true;
		public List<string> Affiliations { get; set; } = new List<string>();
	}
}