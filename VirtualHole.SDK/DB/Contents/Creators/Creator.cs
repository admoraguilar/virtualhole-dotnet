using System;

namespace VirtualHole.DB.Contents.Creators
{
	[Serializable]
	public class Creator
	{
		public string UniversalName = string.Empty;
		public string UniversalId = string.Empty;
		public string WikiUrl = string.Empty;
		public string AvatarUrl = string.Empty;

		public bool IsHidden = false;

		public string[] Affiliations = new string[0];
		public bool IsGroup = false;
		public int Depth = 0;

		public Social[] Socials = new Social[0];
		public string[] CustomKeywords = new string[0];
	}
}
