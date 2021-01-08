using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Midnight;

namespace VirtualHole.Storage
{
	public class VirtualHoleStorageClient
	{
		public string Root { get; private set; } = string.Empty;

		private readonly HttpClient httpClient = null;

		public VirtualHoleStorageClient(string root) 
			: this(HttpClientFactory.HttpClient, root) { }

		public VirtualHoleStorageClient(HttpClient httpClient, string root)
		{
			Debug.Assert(httpClient != null);
			Debug.Assert(!string.IsNullOrEmpty(root));

			Root = root;
			this.httpClient = httpClient;
		}

		public async Task<HttpResponseMessage> GetAsync(
			string relativeObjectPath, CancellationToken cancellationToken = default)
		{
			if(string.IsNullOrEmpty(relativeObjectPath)) {
				throw new InvalidOperationException("Object path mustn't be empty.");
			}

			return await httpClient.GetAsync(
				UriUtilities.CombineUri(Root, relativeObjectPath), 
				cancellationToken);
		}
	}
}
