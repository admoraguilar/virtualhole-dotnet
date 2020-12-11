using System.Collections.Generic;
using System.Text;

namespace VirtualHole.DB.Creators
{
	public class Creator
	{
		/// <summary>
		/// Unique set of strings and numbers to identify a creator.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Default name.
		/// </summary>
		public string Name { get; set; }

		public string AvatarUrl { get; set; }
		public bool IsHidden { get; set; }

		public bool IsGroup { get; set; }
		public int Depth { get; set; }
		public List<string> Affiliations { get; set; }

		public List<CreatorSocial> Socials { get; set; }
	}
}
