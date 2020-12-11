using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Midnight;

namespace VirtualHole.DB.Contents
{
	public class ContentClient
	{
		internal const string ContentCollectionName = "content";

		private IMongoDatabase database = null;
		private IMongoCollection<Content> contentCollection = null;
		private IMongoCollection<BsonDocument> contentBsonCollection = null;

		internal ContentClient(IMongoDatabase database)
		{
			this.database = database;
			contentCollection = this.database.GetCollection<Content>(ContentCollectionName);
			contentBsonCollection = this.database.GetCollection<BsonDocument>(ContentCollectionName);
		}

		public async Task<FindResults<Content>> FindContentsAsync(
			FindSettings<Content> settings = default, CancellationToken cancellationToken = default)
		{
			return new FindResults<Content>(
				await MongoDBUtilities.FindAsync(contentCollection, settings, cancellationToken));
		}

		public async Task UpsertContentAsync(
			Content content, CancellationToken cancellationToken = default)
		{
			await MongoDBUtilities.UpsertAsync(
				contentCollection,
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
				contentBsonCollection,
				(Content content) => new BsonDocument(nameof(Content.Id).ToCamelCase(), content.Id),
				contents, DateTimeOffset.UtcNow.DateTime, cancellationToken);
		}

		public async Task UpsertManyContentsAsync(
			IEnumerable<Content> contents, CancellationToken cancellationToken = default)
		{
			await MongoDBUtilities.UpsertManyAsync(
				contentBsonCollection,
				(Content content) => new BsonDocument(nameof(Content.Id).ToCamelCase(), content.Id),
				contents, DateTimeOffset.UtcNow.DateTime, cancellationToken);
		}
	}
}
