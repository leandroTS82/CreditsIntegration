using System.Text.Json;
using System.Text.Json.Serialization;

namespace Credits.Application.DTOs;

public class SimpleNationalRequestConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value?.Trim().Equals("Sim", StringComparison.OrdinalIgnoreCase) ?? false;
    }
    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        => writer.WriteStringValue(value ? "Sim" : "Não");
}