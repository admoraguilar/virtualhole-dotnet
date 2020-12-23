using System;
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
		private IMongoCollection<BsonDocument> contentsBsonCollection = null;

		internal ContentsClient(IMongoDatabase database)
		{
			this.database = database;
			contentsCollection = this.database.GetCollection<Content>(ContentsCollectionName);
			contentsBsonCollection = this.database.GetCollection<BsonDocument>(ContentsCollectionName);
		}

		public async Task<FindResults<Content>> FindContentsAsync(
			FindSettings settings = default, CancellationToken cancellationToken = default)
		{
			return new FindResults<Content>(
				await MongoDBUtilities.FindAsync(contentsCollection, settings, cancellationToken));
		}

		public async Task UpsertContentAsync(
			Content content, CancellationToken cancellationToken = default)
		{
			await MongoDBUtilities.UpsertAsync(
				contentsCollection,
				new BsonDocument {
					{ nameof(Content.Id).ToCamelCase(), content.Id },
					{ nameof(Content.SocialType).ToCamelCase(), content.SocialType }
				},
				content, cancellationToken);
		}

		public async Task UpsertManyContentsAndDeleteDanglingAsync(
			IEnumerable<Content> contents, CancellationToken cancellationToken = default)
		{
			await MongoDBUtilities.UpsertManyAndDeleteDanglingAsync(
				contentsBsonCollection,
				(Content content) => new BsonDocument(nameof(Content.Id).ToCamelCase(), content.Id),
				contents, DateTimeOffset.UtcNow.DateTime, cancellationToken);
		}

		public async Task UpsertManyContentsAsync(
			IEnumerable<Content> contents, CancellationToken cancellationToken = default)
		{
			await MongoDBUtilities.UpsertManyAsync(
				contentsBsonCollection,
				(Content content) => new BsonDocument(nameof(Content.Id).ToCamelCase(), content.Id),
				contents, DateTimeOffset.UtcNow.DateTime, cancellationToken);
		}
	}
}
