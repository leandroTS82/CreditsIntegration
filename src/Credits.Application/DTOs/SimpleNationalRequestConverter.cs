using System.Text.Json;
using System.Text.Json.Serialization;

namespace Credits.Application.DTOs;

public class SimpleNationalRequestConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
            return reader.GetBoolean();

        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"Valor inválido para o campo simplesNacional: tipo '{reader.TokenType}' não esperado.");

        var value = reader.GetString();

        if (string.IsNullOrWhiteSpace(value))
            throw new JsonException("O campo simplesNacional não pode ser nulo ou vazio.");

        return value.Trim().Equals("Sim", StringComparison.OrdinalIgnoreCase);
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        => writer.WriteStringValue(value ? "Sim" : "Não");
}