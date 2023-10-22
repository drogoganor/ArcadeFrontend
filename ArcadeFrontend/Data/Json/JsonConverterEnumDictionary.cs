using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArcadeFrontend.Data.Json
{
    public class JsonConverterEnumDictionary<TEnum, T> : JsonConverter<Dictionary<TEnum, T>>
        where TEnum : Enum
        where T : class
    {
        public override Dictionary<TEnum, T> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var allValuesRead = false;
            var results = new Dictionary<TEnum, T>();

            while (!allValuesRead)
            {
                T value;

                reader.Read(); // PropertyName
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    allValuesRead = true;
                    break;
                }

                var propertyName = reader.GetString();
                var keyEnum = (TEnum)Enum.Parse(typeof(TEnum), propertyName);

                //reader.Read(); // StartObject

                using (var jsonDoc = JsonDocument.ParseValue(ref reader))
                {
                    value = jsonDoc.Deserialize<T>(options);
                }

                results.Add(keyEnum, value);
            }

            return results;
        }

        public override void Write(
            Utf8JsonWriter writer,
            Dictionary<TEnum, T> value,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var kvp in value)
            {
                writer.WritePropertyName(kvp.Key.ToString());

                writer.WriteStartObject();
                JsonSerializer.Serialize(writer, kvp.Value, options);
                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }
    }
}
