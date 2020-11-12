using System;

namespace VirtualHole.Common
{
	[Serializable]
	public class Content
	{
		public string Title = string.Empty;
		public Platform Platform = Platform.None;
		public string Id = string.Empty;
		public string Url = string.Empty;

		public string Creator = string.Empty;
		public string CreatorId = string.Empty;
		public string CreatorUniversal = string.Empty;
		public string CreatorIdUniversal = string.Empty;

		public DateTimeOffset CreationDate = DateTimeOffset.MinValue;
		public string[] Tags = new string[0];
	}
}