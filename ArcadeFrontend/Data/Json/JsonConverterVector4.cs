using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArcadeFrontend.Data.Json
{
    public class JsonConverterVector4 : JsonConverter<Vector4>
    {
        public override Vector4 Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var allValuesRead = false;
            var valueArray = new float[4];
            var index = 0;
            while (!allValuesRead && reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.EndArray:
                        allValuesRead = true;
                        break;
                    case JsonTokenType.Number:
                        valueArray[index] = reader.GetSingle();
                        break;
                }

                index++;
            }

            return new Vector4(valueArray[0], valueArray[1], valueArray[2], valueArray[3]);
        }

        public override void Write(
            Utf8JsonWriter writer,
            Vector4 value,
            JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.X);
            writer.WriteNumberValue(value.Y);
            writer.WriteNumberValue(value.Z);
            writer.WriteNumberValue(value.W);
            writer.WriteEndArray();
        }
    }
}
