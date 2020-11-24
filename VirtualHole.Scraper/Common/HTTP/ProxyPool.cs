using System;
using System.Collections.Generic;
using System.Linq;
using Midnight;

namespace VirtualHole.Scraper
{
	public class ProxyPool
	{
		public ICollection<Proxy> Proxies => proxyList;
		private List<Proxy> proxyList = new List<Proxy>();
		private Queue<Proxy> proxyQueue = new Queue<Proxy>();

		private Random random = new Random();

		public ProxyPool()
		{ }

		public ProxyPool(string source)
		{
			Set(source);
		}

		public Proxy Get()
		{
			if(proxyQueue.Count <= 0) {
				proxyList = proxyList.OrderBy(p => random.Next()).ToList();
				proxyQueue = new Queue<Proxy>(proxyList);
			}
			return proxyQueue.Dequeue();
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
