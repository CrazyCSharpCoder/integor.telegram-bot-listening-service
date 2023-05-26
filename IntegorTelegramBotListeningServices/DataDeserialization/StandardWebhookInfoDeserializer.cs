using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using IntegorTelegramBotListeningDto.Webhook.WebhookInfo;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.DataDeserialization;

namespace IntegorTelegramBotListeningServices.DataDeserialization
{
	using Internal.DataDeserialization;

	public class StandardWebhookInfoDeserializer : IWebhookInfoDeserializer
	{
		private IJsonSerializerOptionsProvider _jsonOptionsProvider;

		public StandardWebhookInfoDeserializer(
			IJsonSerializerOptionsProvider jsonOptionsProvider)
		{
			_jsonOptionsProvider = jsonOptionsProvider;
		}

		public TelegramApiWebhookInfoDto? Deserialize(JsonElement body)
		{
			JsonElement? result = TelegramResponseDeserializationHelpers.GetResult(body);

			if (result == null)
				return null;

			JsonSerializerOptions serializerOptions =
				_jsonOptionsProvider.GetJsonSerializerOptions();

			return result.Value.Deserialize<TelegramApiWebhookInfoDto>(serializerOptions);
		}
	}
}
