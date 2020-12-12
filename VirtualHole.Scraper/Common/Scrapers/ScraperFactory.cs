using System;
using System.Collections.Concurrent;
using Midnight.Logs;

namespace VirtualHole.Scraper
{
	public abstract class ScraperFactory<T> : ScraperFactory
	{
		private ConcurrentDictionary<string, T> cache = new ConcurrentDictionary<string, T>();

		public ScraperFactory(ProxyPool proxyPool) : base(proxyPool)
		{ }

		public T Get()
		{
			if(proxyPool != null || IsUseProxy) {
				Proxy proxy = proxyPool.Get();
				MLog.Log(typeof(T).Name, $"Proxy: {proxy}");
				return InternalGet(proxy); 
			} else {
				MLog.Log(typeof(T).Name, $"Proxy: None");
				return InternalGet(); 
			}
		}

		protected T InternalGet(Proxy proxy)
		{
			return FromCacheGetOrSet(proxy.ToString(), () => InternalGet_Impl(proxy));
		}

		protected abstract T InternalGet_Impl(Proxy proxy);

		protected T InternalGet() 
		{
			return FromCacheGetOrSet("0", () => InternalGet_Impl());
		}

		protected abstract T InternalGet_Impl();

		private T FromCacheGetOrSet(string id, Func<T> factory)
		{
			if(!cache.TryGetValue(id, out T instance)) {
				cache[id] = instance = factory();	
			}
			return instance;
		}
	}

	public abstract class ScraperFactory 
	{
		public bool IsUseProxy 
		{
			get => isUseProxy;
			set {
				if(proxyPool == null && value) {
					throw new InvalidOperationException("Can't use proxies without a proxy pool.");
				}
				isUseProxy = value;
			}
		}
		private bool isUseProxy = false;

		protected ProxyPool proxyPool { get; private set; } = null;

		public ScraperFactory(ProxyPool proxyPool)
		{
			this.proxyPool = proxyPool;
			if(this.proxyPool == null) { 
				IsUseProxy = false; 
			}
		}
	}
}
