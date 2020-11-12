using System;

namespace VirtualHole.Common
{
	public static class UriUtilities
	{
		public static Uri CombineUri(
			string baseUri, string relativeOrAbsoluteUri,
			bool isAddHttpsIfNoScheme = true)
		{
			Uri uri = null;
			if(isAddHttpsIfNoScheme) { uri = EnsureHttpsScheme(baseUri); } 
			else { uri = new Uri(baseUri); }
			return new Uri(uri, relativeOrAbsoluteUri);
		}

		public static Uri EnsureHttpsScheme(string uri)
		{
			if(!uri.Contains("://")) { uri = uri.Insert(0, $"{Uri.UriSchemeHttps}://"); }
			else if(uri.StartsWith("://")) { uri = uri.Insert(0, $"{Uri.UriSchemeHttps}"); } 
			else if(uri.StartsWith("//")) { uri = uri.Insert(0, $"{Uri.UriSchemeHttps}:"); }
			return new Uri(uri);
		}
	}
}
