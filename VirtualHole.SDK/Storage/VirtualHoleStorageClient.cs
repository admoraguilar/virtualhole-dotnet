using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VirtualHole.Storage
{
	using Common;

	public class VirtualHoleStorageClient
	{
		public string Endpoint { get; private set; } = string.Empty;

		public VirtualHoleStorageClient(string endpoint)
		{
			Endpoint = endpoint;
		}

		public async Task<string> GetAsync(string relativeObjectPath, CancellationToken cancellationToken = default)
		{
			if(string.IsNullOrEmpty(relativeObjectPath)) {
				throw new InvalidOperationException("Object path mustn't be empty.");
			}

			relativeObjectPath = relativeObjectPath.Replace(Endpoint, "");
			string fullObjectPath = BuildObjectUri(relativeObjectPath).AbsoluteUri;

			string result = string.Empty;
			HttpClient client = HttpClientUtilities.GetClient();
			using(HttpResponseMessage response = await client.GetAsync(fullObjectPath, cancellationToken)) {
				result = await response.Content.ReadAsStringAsync();
			}
			return result;
		}

		public Uri BuildObjectUri(string relativeObjectPath)
		{
			return UriUtilities.CombineUri(Endpoint, relativeObjectPath);
		}
	}
}
