using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using Midnight;
using VirtualHole.DB.Creators;
using VirtualHole.DB.Contents;

namespace VirtualHole.DB
{
	internal static class BsonConfig
	{
		public static void Initialize()
		{
			SetConvention();
			RegisterClassMaps();
		}

		private static void SetConvention()
		{
			ConventionPack camelCaseFields = new ConventionPack() { new CamelCaseElementNameConvention() };
			ConventionRegistry.Register("Camel Case Fields", camelCaseFields, t => true);

			ConventionPack deserializeOnlyAvailableFields = new ConventionPack();
			deserializeOnlyAvailableFields.Add(new IgnoreExtraElementsConvention(true));
			ConventionRegistry.Register("Deserialize Only Available Fields", deserializeOnlyAvailableFields, t => true);

			ConventionPack serializeDateTimeOffsetAsString = new ConventionPack();
			serializeDateTimeOffsetAsString.AddMemberMapConvention(
				"Serialize DateTimeOffset As String",
				(BsonMemberMap m) => {
					if(m.MemberType == typeof(DateTimeOffset)) {
						m.SetSerializer(new DateTimeOffsetSerializer(BsonType.String));
					}
				}
			);
			ConventionRegistry.Register("Serialize DateTimeOffset As String", serializeDateTimeOffsetAsString, t => true);

			ConventionPack serializeImmutableProperties = new ConventionPack();
			serializeImmutableProperties.Add(new ReadOnlyPropertyConvention());
			ConventionRegistry.Register("Serialize Immutable Properties", serializeImmutableProperties, t => true);

			ConventionPack idNaming = new ConventionPack();
			idNaming.Add(new NoIdMemberConvention());
			idNaming.Add(new NamedIdMemberConvention("_id"));
			ConventionRegistry.Register("Id Naming", idNaming, t => true);
		}

		private static void RegisterClassMaps()
		{
			RegisterClassMap<CreatorSocial>();
			RegisterClassMap<Content>();

			void RegisterClassMap<T>()
			{
				BsonClassMap.RegisterClassMap<T>(cm => {
					cm.AutoMap();
					cm.SetIsRootClass(true);
				});

				Type type = typeof(T);
				Type[] types = type.GetAllTypeDerivatives(false);
				types.ForEach(t => BsonClassMap.LookupClassMap(t));
			}
		}
	}
}