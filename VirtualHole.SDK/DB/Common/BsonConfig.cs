using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using VirtualHole.DB.Contents.Videos;

namespace VirtualHole.DB
{
	public static class BsonConfig
	{
		public static void SetConvention()
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

			ConventionPack idNaming = new ConventionPack();
			idNaming.Add(new NoIdMemberConvention());
			idNaming.Add(new NamedIdMemberConvention("_id"));
			ConventionRegistry.Register("Id Naming", idNaming, t => true);

			BsonClassMap.RegisterClassMap<Video>(cm => {
				cm.AutoMap();
				cm.SetIsRootClass(true);
			});
			BsonClassMap.RegisterClassMap<Broadcast>();
		}
	}
}
