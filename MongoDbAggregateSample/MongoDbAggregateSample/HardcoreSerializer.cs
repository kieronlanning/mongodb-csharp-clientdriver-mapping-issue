using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using ExternalNonChangableLibrary;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoDbAggregateSample
{
	public class HardcoreSerializer<TAggregate> : SerializerBase<TAggregate>, IBsonDocumentSerializer
		where TAggregate : DataObjectRoot, new()
	{
		public const string BsonDocuemntIdPropertyName = "_id";

		override public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TAggregate value)
		{
			var json = JsonSerializer.Serialize(value);
			var bsonDocument = BsonSerializer.Deserialize<BsonDocument>(json);

			bsonDocument.Add(BsonDocuemntIdPropertyName, value.Details.Id);

			var serializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));
			var doc = bsonDocument.AsBsonValue;

			serializer.Serialize(context, doc);
		}

		override public TAggregate Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			var serializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));
			var document = serializer.Deserialize(context, args);
			var bsonDocument = document.ToBsonDocument();			
			
			var options = new JsonSerializerOptions {
				PropertyNameCaseInsensitive = true,
				AllowTrailingCommas = false,
				IgnoreReadOnlyProperties = false,
				IgnoreNullValues = true,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};

			var result = BsonExtensionMethods.ToJson(bsonDocument);
			return JsonSerializer.Deserialize<TAggregate>(result, options);
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
	}
}
