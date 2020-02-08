using System;
using System.Collections.Generic;
using System.Reflection;
using ExternalNonChangableLibrary;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;

namespace MongoDbAggregateSample
{
	class MakeTheBadManStop<T> : SerializerBase<T>, IBsonIdProvider, IBsonDocumentSerializer
		where T : class//, new()
	{
		override public T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			var serializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));
			var document = serializer.Deserialize(context, args);

			var bsonDocument = document.ToBsonDocument();

			var result = BsonExtensionMethods.ToJson(bsonDocument);
			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(result, new Newtonsoft.Json.JsonSerializerSettings {
				ContractResolver = new PrivateSetterContractResolver(),
				Converters = new List<JsonConverter> { new JsonDateTimeOffsetSerializer() }
			});
		}

		override public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
		{
			var jsonDocument = Newtonsoft.Json.JsonConvert.SerializeObject(value, new JsonSerializerSettings {
				Converters = new List<JsonConverter> { new JsonDateTimeOffsetSerializer() }
			});
			var bsonDocument = BsonSerializer.Deserialize<BsonDocument>(jsonDocument);

			if (value is DataObjectRoot dor && !string.IsNullOrWhiteSpace(dor.Details.Id))
				bsonDocument.Add("_id", dor.Details.Id);

			var serializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));
			var doc = bsonDocument.AsBsonValue;

			serializer.Serialize(context, doc);
		}

		public bool GetDocumentId(object document, out object id, out Type idNominalType, out IIdGenerator idGenerator)
		{
			if (!(document is DataObjectRoot dor) || string.IsNullOrWhiteSpace(dor.Details.Id))
			{
				id = default!;
				idNominalType = default!;
				idGenerator = default!;

				return false;
			}

			id = dor.Details.Id;
			idNominalType = typeof(string);
			idGenerator = StringObjectIdGenerator.Instance;

			return true;
		}

		public void SetDocumentId(object document, object id)
		{
			if (!(document is DataObjectRoot dor))
				return;

			dor.Details.Id = id?.ToString();
		}

		public bool TryGetMemberSerializationInfo(string memberName, out BsonSerializationInfo? serializationInfo)
		{
			var memberType = ValueType.GetProperty(memberName)?.PropertyType;
			if (memberType == null)
			{
				serializationInfo = null;
				return false;
			}

			serializationInfo = new BsonSerializationInfo(memberName, BsonSerializer.LookupSerializer(memberType), ValueType);

			return true;
		}

		private class PrivateSetterContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
		{
			override protected Newtonsoft.Json.Serialization.JsonProperty CreateProperty(MemberInfo member, Newtonsoft.Json.MemberSerialization memberSerialization)
			{
				var prop = base.CreateProperty(member, memberSerialization);
				if (prop.Ignored)
					return prop;

				if (prop.Writable)
					return prop;

				var property = member as PropertyInfo;
				if (property == null)
					return prop;

				var hasPrivateSetter = property.GetSetMethod(true) != null;
				prop.Writable = hasPrivateSetter;

				return prop;
			}
		}
	}
}
