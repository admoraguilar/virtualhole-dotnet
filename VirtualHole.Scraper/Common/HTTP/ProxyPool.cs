using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using Midnight;

namespace VirtualHole.Scraper
{
	public class ProxyPool
	{
		public ICollection<Proxy> Proxies => proxyList;
		private List<Proxy> proxyList = new List<Proxy>();
		private ConcurrentQueue<Proxy> proxyQueue = new ConcurrentQueue<Proxy>();

		private Random random = new Random();

		public ProxyPool()
		{ }

		public ProxyPool(string source)
		{
			Set(source);
		}

		public Proxy Get()
		{
			if(proxyQueue.Count <= 0 || !proxyQueue.TryDequeue(out Proxy result)) {
				proxyList = proxyList.OrderBy(p => random.Next()).ToList();
				proxyQueue = new ConcurrentQueue<Proxy>(proxyList);
				proxyQueue.TryDequeue(out result);
			}
			return result;
		}

		public void Set(string source)
		{
			proxyList.Clear();

			IReadOnlyList<string> rawProxies = TextFileUtilities.GetNLSV(source);
			foreach(string rawProxy in rawProxies) {
				if(Proxy.TryParse(rawProxy, out Proxy proxy)) {
					proxyList.Add(proxy);
				}
			}
		}
	}
}
