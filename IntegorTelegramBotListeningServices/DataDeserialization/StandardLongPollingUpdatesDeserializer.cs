using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using IntegorTelegramBotListeningDto;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.DataDeserialization;

namespace IntegorTelegramBotListeningServices.DataDeserialization
{
    using Internal.DataDeserialization;

    public class StandardLongPollingUpdatesDeserializer : ILongPollingUpdatesDeserializer
    {
        private IJsonSerializerOptionsProvider _jsonOptionsProvider;

        public StandardLongPollingUpdatesDeserializer(
            IJsonSerializerOptionsProvider jsonOptionsProvider)
        {
            _jsonOptionsProvider = jsonOptionsProvider;
        }

        public IEnumerable<TelegramUpdateDto>? Deserialize(JsonElement body)
        {
			JsonElement? result = TelegramResponseDeserializationHelpers.GetResult(body);

			if (result == null)
				return null;

			JsonSerializerOptions serializerOptions =
				_jsonOptionsProvider.GetJsonSerializerOptions();

			JsonElement.ArrayEnumerator updates = result.Value.EnumerateArray();

            return updates
				.Select(jsonUpdate => JsonElementHelpers
					.TryDeserializeJson<TelegramUpdateDto>(jsonUpdate, serializerOptions))

                .Where(deserializationResult => deserializationResult != null)!;
        }
    }
}
