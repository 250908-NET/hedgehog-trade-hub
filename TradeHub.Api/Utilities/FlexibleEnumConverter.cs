using System.Text.Json;
using System.Text.Json.Serialization;

namespace TradeHub.Api.Utilities;

/// <summary>
/// A JsonConverterFactory that provides a FlexibleEnumConverter for any enum type.
/// This allows deserialization of enums from both string names and integer values.
/// Serialization will always be to string names.
/// </summary>
public class FlexibleEnumConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsEnum;
    }

    public override JsonConverter? CreateConverter(
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (!typeToConvert.IsEnum)
        {
            return null;
        }

        // Create a converter for the specific enum type
        var converterType = typeof(FlexibleEnumConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}

/// <summary>
/// A JsonConverter for enums that can deserialize from both string names and integer values.
/// Serialization will always be to string names.
/// </summary>
/// <typeparam name="TEnum">The enum type.</typeparam>
public class FlexibleEnumConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    public override TEnum Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string? enumString = reader.GetString();
            if (enumString != null && Enum.TryParse(enumString, true, out TEnum result))
            {
                return result;
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt32(out int enumInt))
            {
                // Check if the integer value is a defined enum member
                if (Enum.IsDefined(typeToConvert, enumInt))
                {
                    return (TEnum)(object)enumInt;
                }
            }
        }

        // Provide more context in the error message for better debugging
        string tokenValue = reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString() ?? "null string",
            JsonTokenType.Number => reader.TryGetInt64(out long l)
                ? l.ToString()
                : (reader.TryGetDouble(out double d) ? d.ToString() : "invalid number format"),
            _ => reader.TokenType.ToString(),
        };
        throw new JsonException(
            $"Unable to convert '{tokenValue}' (TokenType: {reader.TokenType}) to enum {typeToConvert.Name}. Expected string or integer value."
        );
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        // For writing, we'll stick to string representation for consistency and readability
        writer.WriteStringValue(value.ToString());
    }
}
