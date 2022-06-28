using System.Text.Json;
using System.Text.Json.Serialization;

namespace BasketballStats.Domain
{
    public static class Constants
    {
        public static JsonSerializerOptions EnumSerializerOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };
    }
}
