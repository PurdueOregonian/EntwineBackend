using System.Text.Json;
using System.Text.Json.Serialization;

namespace EntwineBackend
{
    public class DefaultJsonOptions
    {
        public static JsonSerializerOptions Instance => LazyInstance.Value;

        private static readonly Lazy<JsonSerializerOptions> LazyInstance = new(() =>
        {
            var options = new JsonSerializerOptions();
            InitOptions(options);
            return options;
        });

        public static void InitOptions(JsonSerializerOptions options) {
            options.PropertyNameCaseInsensitive = true;
            options.Converters.Add(new JsonStringEnumConverter());
        }
    }
}
