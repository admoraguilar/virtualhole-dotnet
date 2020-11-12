﻿using System;

namespace VirtualHole.Common
{
	[Serializable]
	public class Social
	{
		public string Name;
		public Platform Platform;
		public string Id;
		public string Url;
		public string AvatarUrl;

		public string[] CustomKeywords = new string[0];
	}
}