using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeltaFour.CrossCutting.Ioc
{
    /// <summary>
    /// Garante que todo DateTime seja serializado como UTC com o sufixo 'Z'.
    /// Os valores são armazenados em UTC no banco; ao expor com 'Z', o frontend
    /// consegue convertê-los automaticamente para o fuso horário local do usuário.
    /// A leitura mantém o comportamento padrão do System.Text.Json.
    /// </summary>
    public class UtcDateTimeJsonConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetDateTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var utc = value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                // Valores vindos do banco (MySQL) chegam como Unspecified, mas
                // por convenção são gravados em UTC.
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc),
            };

            writer.WriteStringValue(utc.ToString(Format, CultureInfo.InvariantCulture));
        }
    }

    /// <summary>
    /// Variante para DateTime? — mesma regra de serialização em UTC ('Z').
    /// </summary>
    public class NullableUtcDateTimeJsonConverter : JsonConverter<DateTime?>
    {
        private const string Format = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            return reader.GetDateTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            var raw = value.Value;
            var utc = raw.Kind switch
            {
                DateTimeKind.Utc => raw,
                DateTimeKind.Local => raw.ToUniversalTime(),
                _ => DateTime.SpecifyKind(raw, DateTimeKind.Utc),
            };

            writer.WriteStringValue(utc.ToString(Format, CultureInfo.InvariantCulture));
        }
    }
}
