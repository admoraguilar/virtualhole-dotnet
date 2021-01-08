using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Midnight;

namespace VirtualHole.DB.Contents
{
	public class ContentsClient
	{
		internal const string ContentsCollectionName = "contents";

		private IMongoDatabase database = null;
		private IMongoCollection<Content> contentsCollection = null;

		internal ContentsClient(IMongoDatabase database)
		{
			this.database = database;
			contentsCollection = this.database.GetCollection<Content>(ContentsCollectionName);
		}

		public async Task<List<Content>> FindAllAsync(CancellationToken cancellationToken = default)
		{
			return await MongoDBUtilities.FindAllAsync(contentsCollection, cancellationToken);
		}

		public async Task<FindResults<Content>> FindAsync(
			FindSettings settings = default, CancellationToken cancellationToken = default)
		{
			return new FindResults<Content>(
				await MongoDBUtilities.FindAsync(contentsCollection, settings, cancellationToken));
		}

		public async Task UpsertManyAsync(
			IEnumerable<Content> contents, CancellationToken cancellationToken = default)
		{
			await MongoDBUtilities.UpsertManyAsync(
				contentsCollection,
				(Content content) => new BsonDocument(nameof(Content.Id).ToCamelCase(), content.Id),
				contents, cancellationToken);
		}

		public async Task DeleteManyAsync(
			IEnumerable<Content> contents, CancellationToken cancellationToken = default)
		{
			await MongoDBUtilities.DeleteManyAsync(
				contentsCollection,
				(Content content) => new BsonDocument(nameof(Content.Id).ToCamelCase(), content.Id),
				contents, cancellationToken);
		}
	}
}
