using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace VirtualHole.DB
{
	internal static class MongoDBUtilities
	{
		public const string IdFieldKey = "_id";
		public const string TFieldKey = "_t";
		public const string LastOperationTimestampFieldKey = "_lastOperationTimestamp";

		public static async Task<IAsyncCursor<T>> FindAsync<T>(
			IMongoCollection<T> collection, FindSettings settings,
			CancellationToken cancellationToken = default)
		{
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

		public static async Task UpsertAsync<T>(
			IMongoCollection<T> collection, FilterDefinition<T> filter,
			T obj, CancellationToken cancellationToken = default)
		{
			await collection.ReplaceOneAsync(filter, obj,
				new ReplaceOptions {
					IsUpsert = true
				}, cancellationToken);
		}

		public static async Task UpsertManyAndDeleteDanglingAsync<T>(
			IMongoCollection<BsonDocument> collection, Func<T, FilterDefinition<BsonDocument>> filter,
			IEnumerable<T> objs, DateTime timestamp, CancellationToken cancellationToken = default)
		{
			await UpsertManyAsync(collection, filter, objs, timestamp, cancellationToken);
			await collection.DeleteManyAsync(NotEqualTimestampFilter(), cancellationToken);

			BsonDocument NotEqualTimestampFilter()
			{
				return new BsonDocument(
					LastOperationTimestampFieldKey,
					new BsonDocument(
						"$not",
						new BsonDocument("$eq", timestamp)));
			}
		}

		public static async Task UpsertManyAsync<T>(
			IMongoCollection<BsonDocument> collection, Func<T, FilterDefinition<BsonDocument>> filter,
			IEnumerable<T> objs, DateTime timestamp, CancellationToken cancellationToken = default)
		{
			List<WriteModel<BsonDocument>> bulkReplace = new List<WriteModel<BsonDocument>>();
			foreach(T obj in objs) {
				bulkReplace.Add(
					new ReplaceOneModel<BsonDocument>(filter(obj), ToBsonDocumentWithTimestamp(obj, timestamp)) {
						IsUpsert = true,
					});
			}

			await collection.BulkWriteAsync(bulkReplace, null, cancellationToken);
		}

		public static async Task DeleteManyAsync<T>(
			IMongoCollection<BsonDocument> collection, Func<T, BsonDocument> filter,
			IEnumerable<T> objs, DateTime timestamp, CancellationToken cancellationToken = default)
		{
			if(objs.Count() <= 0) {
				return;
			}

			// TODO: Refactor MongoDBUtilities with better generic filters
			// for common operations
			BsonDocument deleteFilter = new BsonDocument();
			foreach(T obj in objs) {
				deleteFilter.AddRange(filter(obj));
			}

			await collection.DeleteManyAsync(deleteFilter, cancellationToken);
		}

		public static BsonDocument ToBsonDocumentWithTimestamp<T>(T obj, DateTime timestamp)
		{
			BsonDocument objBSON = obj.ToBsonDocument();
			objBSON.Add(new BsonElement(LastOperationTimestampFieldKey, timestamp));
			return objBSON;
		}
	}
}
