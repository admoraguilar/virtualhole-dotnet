using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VirtualHole.DB
{
	/// <summary>
	/// Source: https://stackoverflow.com/questions/19307752/deserializing-polymorphic-json-classes-without-type-information-using-json-net
	/// </summary>
	public abstract class PolymorphicObjectConverter<T> : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(T).IsAssignableFrom(objectType);
		}

		public override bool CanRead => true;
		public override bool CanWrite => false;

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			// Ignore null JTokens and read until 
			// there's a non-null JToken
			JToken jToken = JToken.Load(reader);
			while((jToken == null || jToken.Type == JTokenType.Null) &&
				  reader.Read()) {
				jToken = JToken.Load(reader);
			}

			if(jToken is JObject jObj) {
				return PopulateSerializer(jObj);
			} else if(jToken is JArray jArr) {
				List<T> results = new List<T>();
				foreach(JObject jo in jArr) {
					results.Add(PopulateSerializer(jo));
				}
				return results;
			} else {
				throw new InvalidOperationException($"JToken type: {jToken.GetType()}");
			}

			T PopulateSerializer(JObject jObject)
			{
				T result = ProcessJObject(jObject);
				serializer.Populate(jObject.CreateReader(), result);
				return result;
			}
		}

		public abstract T ProcessJObject(JObject jObj);

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
