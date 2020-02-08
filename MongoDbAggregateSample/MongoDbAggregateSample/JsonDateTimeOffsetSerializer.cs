using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MongoDbAggregateSample
{
	internal class JsonDateTimeOffsetSerializer : JsonConverter<DateTimeOffset>
	{
		override public DateTimeOffset ReadJson(JsonReader reader, Type objectType, [AllowNull] DateTimeOffset existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (!hasExistingValue)
				existingValue = new DateTimeOffset();

			if (reader.TokenType == JsonToken.StartObject)
			{
				// Property name read
				reader.Skip();
				
				// Value read
				if (!reader.Read())
					return existingValue;

				reader.ReadAsString();

				//JValue x = new JValue()

				//var dateTimeObject = JToken.Load(reader);

			}

			if (!reader.Read()) // Read document
				return existingValue;

			reader.Skip();//.Read(); // Read property name
			reader.ReadAsInt32(); // DateTime...

			var ticks = reader.ReadAsInt32()!;
			existingValue.AddTicks(ticks.Value);

			var offsetInMinutes = reader.ReadAsInt32()!;

			return existingValue.ToOffset(TimeSpan.FromMinutes(offsetInMinutes.Value));
		}

		override public void WriteJson(JsonWriter writer, [AllowNull] DateTimeOffset value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			
			writer.WritePropertyName("DateTime");
			writer.WriteValue(MongoDB.Bson.BsonUtils.ToMillisecondsSinceEpoch(value.UtcDateTime));

			writer.WritePropertyName("Ticks");
			writer.WriteValue(value.Ticks);

			writer.WritePropertyName("Offset");
			writer.WriteValue(value.Offset.TotalMinutes);

			writer.WriteEndObject();
		}
	}
}
