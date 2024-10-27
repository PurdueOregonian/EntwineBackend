using System.Text.Json;

namespace Friends5___Backend
{
    public class DefaultJsonOptions
    {
        public static readonly JsonSerializerOptions Instance = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
