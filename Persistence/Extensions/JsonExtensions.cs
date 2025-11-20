using System.Text.Json.Serialization;
using System.Text.Json;

namespace Persistence.Extensions
{
    public static class JsonExtensions
    {
        private static JsonSerializerOptions SerializerOptions { get; set; } = new JsonSerializerOptions();

        public static void SetOptions(this JsonSerializerOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            options.WriteIndented = true; // pretty print when serializing
            options.PropertyNameCaseInsensitive = true; // "name" or "Name" both work
            options.AllowTrailingCommas = true;
            // options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // auto convert PascalCase → camelCase
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

            options.Converters.Add(new JsonStringEnumConverter());

            SerializerOptions = options;
        }

        public static string? ToJson<T>(this T obj)
        {
            if (obj == null)
                return null;

            return JsonSerializer.Serialize(obj, obj.GetType(), SerializerOptions);
        }

        public static T? FromJson<T>(this string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? default : JsonSerializer.Deserialize<T>(value, SerializerOptions);
        }
    }
}