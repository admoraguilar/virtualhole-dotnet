using System.Net;
using System.Net.Http;
using System.Collections.Concurrent;

namespace VirtualHole.Scraper
{
	public class HttpClientFactory
	{
		private static ConcurrentDictionary<string, HttpClient> lookup = new ConcurrentDictionary<string, HttpClient>();

		public static HttpClient Get(Proxy proxy)
		{
			string proxyString = proxy.ToString();

			if(!lookup.TryGetValue(proxyString, out HttpClient client)) {
				HttpClientHandler clientHandler = new HttpClientHandler();
				clientHandler.Proxy = new WebProxy(proxy.Host, proxy.Port);
				clientHandler.UseCookies = false;

				if(clientHandler.SupportsAutomaticDecompression) {
					clientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
				}

				client = new HttpClient(clientHandler, true);
				client.DefaultRequestHeaders.Add(
					"User-Agent",
					"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.163 Safari/537.36"
				);
				client.DefaultRequestHeaders.ConnectionClose = true;

				lookup[proxyString] = client;
			}

			return client;
		}
	}
}
