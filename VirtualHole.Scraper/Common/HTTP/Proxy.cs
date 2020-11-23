﻿using System;
using System.Net;

namespace VirtualHole.Scraper
{
	public struct Proxy
	{
		public static Proxy Parse(string value)
		{
			if(!TryParse(value, out Proxy result)) {
				throw new InvalidOperationException("Not a valid proxy");
			}
			return result;
		}

		public static bool TryParse(string value, out Proxy resultProxy)
		{
			bool result = false;

			string[] split = value.Split(':');
			IPAddress address = null;
			int port = 0;

			if(IPAddress.TryParse(split[0], out address)) {
				if(split.Length > 1) {
					if(int.TryParse(split[1], out port)) {
						if(port > 0 && port < 65536) {
							result = true;
						}
					}
				}
			}

			if(result) { resultProxy = new Proxy(address.ToString(), port); } 
			else { resultProxy = new Proxy("0.0.0.0", 0); }
			return result;
		}

		public string Host;
		public int Port;

		public Proxy(string host, int port)
		{
			this.Host = host;
			this.Port = port;
		}

		public override string ToString() => $"{Host}:{Port}";
	}
}