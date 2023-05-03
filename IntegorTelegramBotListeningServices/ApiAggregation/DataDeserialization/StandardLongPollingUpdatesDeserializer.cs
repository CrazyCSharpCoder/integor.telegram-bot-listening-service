using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using IntegorTelegramBotListeningDto;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.ApiAggregation.DataDeserialization;

namespace IntegorTelegramBotListeningServices.ApiAggregation.DataDeserialization
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
			JsonSerializerOptions serializerOptions = _jsonOptionsProvider.GetJsonSerializerOptions();

			if (!body.GetProperty("ok").GetBoolean())
				return null;

			JsonElement.ArrayEnumerator updates = body.GetProperty("result").EnumerateArray();

			return updates
				.Select(jsonUpdate => JsonElementHelpers.TryDeserializeJson<TelegramUpdateDto>(jsonUpdate, serializerOptions))
				.Where(deserializationResult => deserializationResult != null)!;
		}
	}
}
