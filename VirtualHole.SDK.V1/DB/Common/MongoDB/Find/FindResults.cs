using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using MongoDB.Driver;

namespace VirtualHole.DB
{
	public class FindResults<T> : IDisposable
	{
		public IEnumerable<T> Current => Cursor.Current;
		internal IAsyncCursor<T> Cursor { get; private set; } = null;

		internal FindResults(IAsyncCursor<T> cursor)
		{
			Debug.Assert(cursor != null);

			this.Cursor = cursor;
		}

		public bool MoveNext(CancellationToken cancellationToken = default)
		{
			return Cursor.MoveNext(cancellationToken);
		}

		public async Task<bool> MoveNextAsync(CancellationToken cancellationToken = default)
		{
			return await Cursor.MoveNextAsync(cancellationToken);
		}

		public void Dispose()
		{
			Cursor.Dispose();
		}
	}

	public static class FindResultsExtension
	{
		public static Task<List<T>> ToListAsync<T>(
			this FindResults<T> findResults, CancellationToken cancellationToken = default)
		{
			return findResults.Cursor.ToListAsync(cancellationToken);
		}
	}
}
