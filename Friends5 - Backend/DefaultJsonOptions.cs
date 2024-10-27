using System.Text.Json;
using System.Text.Json.Serialization;

namespace Friends5___Backend
{
    public class DefaultJsonOptions
    {
        public static readonly JsonSerializerOptions Instance = new(JsonSerializerOptions.Default)
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }
}
