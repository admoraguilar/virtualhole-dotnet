using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Midnight;

namespace VirtualHole.Storage
{
	public class VirtualHoleStorageClient
	{
		public string Root { get; private set; } = string.Empty;

		public VirtualHoleStorageClient(string root)
		{
			Root = root;
		}

		public async Task<HttpResponseMessage> GetAsync(
			string relativeObjectPath, CancellationToken cancellationToken = default)
		{
			if(string.IsNullOrEmpty(relativeObjectPath)) {
				throw new InvalidOperationException("Object path mustn't be empty.");
			}

			HttpClient httpClient = HttpClientFactory.HttpClient;
			return await httpClient.GetAsync(UriUtilities.CombineUri(Root, relativeObjectPath));
		}
	}
}
