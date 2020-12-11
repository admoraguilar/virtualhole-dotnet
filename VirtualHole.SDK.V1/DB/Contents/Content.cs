using System;
using System.Collections.Generic;

namespace VirtualHole.DB.Contents
{
	public abstract class Content
	{
		public abstract string SocialType { get; }
		public abstract string ContentType { get; }

		public string CreatorId { get; set; } = string.Empty;

		public string Id { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Url { get; set; } = string.Empty;
		public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.MinValue;
		public List<string> Tags { get; set; } = new List<string>();
	}
}