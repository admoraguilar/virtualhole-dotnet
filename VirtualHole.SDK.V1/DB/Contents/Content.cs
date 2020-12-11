using System;
using System.Collections.Generic;

namespace VirtualHole.DB.Contents
{
	public abstract class Content
	{
		public abstract string SocialType { get; }
		public abstract string ContentType { get; }

		public string CreatorId { get; set; }

		public string Id { get; set; }
		public string Title { get; set; }
		public string Url { get; set; }
		public DateTimeOffset CreationDate { get; set; }
		public List<string> Tags { get; set; }
	}
}