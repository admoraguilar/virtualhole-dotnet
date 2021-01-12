using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Driver;

namespace VirtualHole.DB
{
	// TODO: Refactor MongoDBUtilities with better generic filters
	// for common operations
	internal static class MongoDBUtilities
	{
		public static async Task<List<T>> FindAllAsync<T>(
			IMongoCollection<T> collection, CancellationToken cancellationToken = default)
		{
			Debug.Assert(collection != null);

			return await collection
				.Find(Builders<T>.Filter.Empty)
				.ToListAsync(cancellationToken);
		}

		public static async Task<IAsyncCursor<T>> FindAsync<T>(
			IMongoCollection<T> collection, FindSettings settings,
			CancellationToken cancellationToken = default)
		{
			Debug.Assert(collection != null);
			Debug.Assert(settings != null);

			return await collection.FindAsync(
				settings.FilterDocument,
				new FindOptions<T, T> {
					Sort = settings.SortDocument,
					BatchSize = settings.GetBatchSize(),
					Limit = settings.GetResultsLimit(),
					Skip = settings.GetSkip()
				},
				cancellationToken);
		}

		public static async Task UpsertManyAsync<T>(
			IMongoCollection<T> collection, Func<T, FilterDefinition<T>> filter,
			IEnumerable<T> objs, CancellationToken cancellationToken = default)
		{
			Debug.Assert(collection != null);
			Debug.Assert(filter != null);
			Debug.Assert(objs != null);

			if(objs.Count() <= 0) { return; }

			List<WriteModel<T>> bulkReplace = new List<WriteModel<T>>();
			foreach(T obj in objs) {
				bulkReplace.Add(
					new ReplaceOneModel<T>(filter(obj), obj) {
						IsUpsert = true,
					});
			}

			await collection.BulkWriteAsync(bulkReplace, null, cancellationToken);
		}

		public static async Task DeleteManyAsync<T>(
			IMongoCollection<T> collection, Func<T, FilterDefinition<T>> filter,
			IEnumerable<T> objs, CancellationToken cancellationToken = default)
		{
			Debug.Assert(collection != null);
			Debug.Assert(filter != null);
			Debug.Assert(objs != null);

			if(objs.Count() <= 0) { return; }

			List<WriteModel<T>> bulkDelete = new List<WriteModel<T>>();
			foreach(T obj in objs) {
				bulkDelete.Add(new DeleteOneModel<T>(filter(obj)));
			}

			await collection.BulkWriteAsync(bulkDelete, null, cancellationToken);
		}
	}
}
