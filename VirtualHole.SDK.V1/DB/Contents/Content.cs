using System;
using System.Collections.Generic;
using VirtualHole.DB.Creators;

namespace VirtualHole.DB.Contents
{
	public abstract class Content
	{
		public static bool operator ==(Content a, Content b) => Equals(a, b);
		public static bool operator !=(Content a, Content b) => !Equals(a, b);

		public abstract string SocialType { get; }
		public abstract string ContentType { get; }

		public string Id { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Url { get; set; } = string.Empty;
		public CreatorSimple Creator { get; set; } = new CreatorSimple();
		public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.MinValue;
		public List<string> Tags { get; set; } = new List<string>();

		public override bool Equals(object obj) 
		{
			Content other = obj as Content;
			if(other is null) { return false; }

			return SocialType == other.SocialType &&
				ContentType == other.ContentType &&
				Id == other.Id &&
				Title == other.Title &&
				Url == other.Url;
		}

		public override int GetHashCode()
		{
			return (SocialType, ContentType,
				Id, Title,
				Url).GetHashCode();
		}
	}		
}
