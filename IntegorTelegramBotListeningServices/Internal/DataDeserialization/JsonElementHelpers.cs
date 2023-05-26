using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningServices.Internal.DataDeserialization
{
    internal static class JsonElementHelpers
    {
        public static JsonProperty? TryGetPropertyCaseInsensitive(
            JsonElement.ObjectEnumerator properties, string propName)
        {
            try
            {
                return properties.First(prop => prop.Name.ToLower() == propName.ToLower());
            }
            catch
            {
                return null;
            }
        }

        public static TParseResult? TryDeserializeJson<TParseResult>(
            JsonElement json, JsonSerializerOptions options)
            where TParseResult : class
        {
            try
            {
                return json.Deserialize<TParseResult>(options);
            }
            catch
            {
                return null;
            }
        }
    }
}
