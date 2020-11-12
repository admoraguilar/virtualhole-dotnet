using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Driver;

namespace VirtualHole.DB.Common
{
	public class FindResults<T> : IDisposable
	{
		public IEnumerable<T> Current => cursor.Current;

		private IAsyncCursor<T> cursor = null;

		internal FindResults(IAsyncCursor<T> cursor)
		{
			this.cursor = cursor;
		}

		public bool MoveNext(CancellationToken cancellationToken = default)
		{
			return cursor.MoveNext(cancellationToken);
		}

		public async Task<bool> MoveNextAsync(CancellationToken cancellationToken = default)
		{
			return await cursor.MoveNextAsync(cancellationToken);
		}

		public void Dispose()
		{
			cursor.Dispose();
		}
	}
}
