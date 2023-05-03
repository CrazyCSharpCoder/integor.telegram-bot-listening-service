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
	public class StandardWebhookUpdateDeserializer : IWebhookUpdateDeserializer
	{
		private IJsonSerializerOptionsProvider _jsonOptionsProvider;

		public StandardWebhookUpdateDeserializer(
			IJsonSerializerOptionsProvider jsonOptionsProvider)
		{
			_jsonOptionsProvider = jsonOptionsProvider;
		}

		public TelegramUpdateDto? Deserialize(JsonElement body)
		{
			JsonSerializerOptions serializerOptions = _jsonOptionsProvider.GetJsonSerializerOptions();
			return body.Deserialize<TelegramUpdateDto>(serializerOptions);
		}
	}
}
