﻿using System.Diagnostics;

namespace VirtualHole.DB.Creators
{
	public class CreatorSocialSimple
	{
		public string Id { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Url { get; set; } = string.Empty;
		public string AvatarUrl { get; set; } = string.Empty;

		public CreatorSocialSimple() { }

		public CreatorSocialSimple(CreatorSocial creatorSocial)
		{
			Debug.Assert(creatorSocial != null);

			Id = creatorSocial.Id;
			Name = creatorSocial.Name;
			Url = creatorSocial.Url;
			AvatarUrl = creatorSocial.AvatarUrl;
		}
	}
}
