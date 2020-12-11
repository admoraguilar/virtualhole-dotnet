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

		private IMongoDatabase _database = null;
		private IMongoCollection<Content> _contentCollection = null;
		private IMongoCollection<BsonDocument> _contentBsonCollection = null;

		internal ContentClient(IMongoDatabase database)
		{
			_database = database;
			_contentCollection = _database.GetCollection<Content>(ContentCollectionName);
			_contentBsonCollection = _database.GetCollection<BsonDocument>(ContentCollectionName);
		}

		public async Task<FindResults<Content>> FindContentsAsync(
			FindSettings<Content> settings = default, CancellationToken cancellationToken = default)
		{
			return new FindResults<Content>(
				await MongoDBUtilities.FindAsync(_contentCollection, settings, cancellationToken));
		}

		public async Task UpsertContentAsync(
			Content content, CancellationToken cancellationToken = default)
		{
			await MongoDBUtilities.UpsertAsync(
				_contentCollection,
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
				_contentBsonCollection,
				(Content content) => new BsonDocument(nameof(Content.Id).ToCamelCase(), content.Id),
				contents, DateTimeOffset.UtcNow.DateTime, cancellationToken);
		}

		public async Task UpsertManyContentsAsync(
			IEnumerable<Content> contents, CancellationToken cancellationToken = default)
		{
			await MongoDBUtilities.UpsertManyAsync(
				_contentBsonCollection,
				(Content content) => new BsonDocument(nameof(Content.Id).ToCamelCase(), content.Id),
				contents, DateTimeOffset.UtcNow.DateTime, cancellationToken);
		}
	}
}
