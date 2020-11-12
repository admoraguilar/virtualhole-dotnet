using System.Net.Http;

namespace VirtualHole.Common
{
	public static class HttpClientUtilities
	{
		private static HttpClient httpClient = null;

		public static HttpClient GetClient()
		{
			if(httpClient != null) { return httpClient; }
			return httpClient = new HttpClient();
		}

		public static HttpClient CreateAndCacheClient(HttpClient client)
		{
			return httpClient = client;
		}
	}
}
