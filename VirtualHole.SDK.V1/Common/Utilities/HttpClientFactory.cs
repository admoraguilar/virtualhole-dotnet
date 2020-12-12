using System.Net.Http;

namespace VirtualHole
{
	internal static class HttpClientFactory
	{
		public static HttpClient HttpClient
		{
			get {
				if(httpClient != null) { return httpClient; }
				return httpClient = new HttpClient();
			}
			set {
				httpClient = value;
			}
		}
		private static HttpClient httpClient = null;
	}
}
