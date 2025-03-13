using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PsStore.Application.Common
{
    public class JsonDateTimeConverter : JsonConverter<DateTime>
    {
        private const string DateFormat = "dd MMM yyyy, HH:mm";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && DateTime.TryParse(reader.GetString(), out DateTime date))
            {
                return date;
            }
            throw new JsonException("Invalid date format");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
        }
    }

    public class NullableJsonDateTimeConverter : JsonConverter<DateTime?>
    {
        private const string DateFormat = "dd MMM yyyy, HH:mm";

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && DateTime.TryParse(reader.GetString(), out DateTime date))
            {
                return date;
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString(DateFormat, CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteStringValue("Not Updated");
            }
        }
    }
}
