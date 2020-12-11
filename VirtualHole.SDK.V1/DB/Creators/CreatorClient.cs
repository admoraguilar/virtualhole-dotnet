using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Midnight;

namespace VirtualHole.DB.Creators
{
	public class CreatorClient
	{
		internal const string CreatorsCollectionName = "creators";

		private IMongoDatabase _database = null;
		private IMongoCollection<Creator> _creatorCollection = null;
		private IMongoCollection<BsonDocument> _creatorBsonCollection = null;

		internal CreatorClient(IMongoDatabase database)
		{
			_database = database;
			_creatorCollection = _database.GetCollection<Creator>(CreatorsCollectionName);
			_creatorBsonCollection = _database.GetCollection<BsonDocument>(CreatorsCollectionName);
		}

		public async Task<FindResults<Creator>> FindCreatorsAsync(
			FindSettings<Creator> settings = default, CancellationToken cancellationToken = default)
		{
			return new FindResults<Creator>(
				await MongoDBUtilities.FindAsync(_creatorCollection, settings, cancellationToken));
		}

		public async Task UpsertCreatorAsync(
			Creator creator, CancellationToken cancellationToken = default)
		{
			await MongoDBUtilities.UpsertAsync(
				_creatorCollection,
				new BsonDocument(nameof(Creator.Id).ToCamelCase(), creator.Id),
				creator, cancellationToken);
		}

		public async Task UpsertManyCreatorsAndDeleteDanglingAsync(
			IEnumerable<Creator> creators, CancellationToken cancellationToken = default)
		{
			await MongoDBUtilities.UpsertManyAndDeleteDanglingAsync(
				_creatorBsonCollection,
				(Creator creator) => new BsonDocument(nameof(Creator.Id).ToCamelCase(), creator.Id),
				creators, DateTimeOffset.UtcNow.DateTime, cancellationToken);
		}

		public async Task UpsertManyCreatorsAsync(
			IEnumerable<Creator> creators, CancellationToken cancellationToken = default)
		{
			await MongoDBUtilities.UpsertManyAsync(
				_creatorBsonCollection,
				(Creator creator) => new BsonDocument(nameof(Creator.Id).ToCamelCase(), creator.Id),
				creators, DateTimeOffset.UtcNow.DateTime, cancellationToken);
		}	
	}
}
