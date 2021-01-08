﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VirtualHole.DB.Creators
{
	public class CreatorSimple
	{
		public string Id { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string AvatarUrl { get; set; } = string.Empty;

		public List<string> Affiliations { get; set; } = new List<string>();
		public CreatorSocialSimple Social { get; set; } = new CreatorSocialSimple();

		public CreatorSimple() { }

		public CreatorSimple(Creator creator, CreatorSocial creatorSocial)
		{
			Debug.Assert(creator != null);
			Debug.Assert(creatorSocial != null);

			Id = creator.Id;
			Name = creator.Name;
			AvatarUrl = creator.AvatarUrl;
			Affiliations = creator.Affiliations;
			Social = new CreatorSocialSimple(creatorSocial);
		}
	}
}
