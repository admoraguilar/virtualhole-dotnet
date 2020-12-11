using System.Collections.Generic;

namespace VirtualHole.DB.Creators
{
	public class Creator
	{
		/// <summary>
		/// Unique set of strings and numbers to identify a creator.
		/// </summary>
		public string Id { get; set; } = string.Empty;

		/// <summary>
		/// Default name.
		/// </summary>
		public string Name { get; set; } = string.Empty;

		public string AvatarUrl { get; set; } = string.Empty;
		public bool IsHidden { get; set; } = false;

		public bool IsGroup { get; set; } = false;
		public int Depth { get; set; } = 0;
		public List<string> Affiliations { get; set; } = new List<string>();

		public List<CreatorSocial> Socials { get; set; } = new List<CreatorSocial>();
	}
}
