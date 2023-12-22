using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using System.Net.Http;

using Microsoft.Extensions.Options;

using IntegorErrorsHandling;
using IntegorResponseDecoration.Parsing;

using IntegorTelegramBotListeningDto;

using IntegorServicesInteraction;
using IntegorServicesInteractionHelpers;

using ExternalServicesConfiguration;

using IntegorTelegramBotListeningShared.ApiAggregation.Aggregators;
using IntegorTelegramBotListeningShared;

namespace IntegorTelegramBotListeningServices.ApiAggregation.Aggregators
{
	public class StandardTelegramBotApiAggregator : ITelegramBotApiAggregator
	{
		private const string _sendMessageMethod = "sendMessage";

		private IJsonSerializerOptionsProvider _jsonOptionsProvider;
		private IDecoratedObjectParser<TelegramMessageInfoDto, JsonElement> _messageParser;
		private JsonServicesRequestProcessor<IntegorDataServiceConfiguration> _requestProcessor;

		public StandardTelegramBotApiAggregator(
			IOptions<IntegorDataServiceConfiguration> dataServiceOptions,

			IJsonSerializerOptionsProvider jsonOptionsProvider,

			IDecoratedObjectParser<TelegramMessageInfoDto, JsonElement> messageParser,
			IDecoratedObjectParser<IEnumerable<IResponseError>, JsonElement> errorsParser)
		{
			_messageParser = messageParser;
			_jsonOptionsProvider = jsonOptionsProvider;

			_requestProcessor = new JsonServicesRequestProcessor<IntegorDataServiceConfiguration>(
				errorsParser, dataServiceOptions, "telegram-events");
		}

		public async Task<bool> AllowAggregationAsync(string apiMethod)
		{
			return apiMethod.ToLower() == _sendMessageMethod.ToLower();
		}

		public async Task AggregateAsync(JsonElement data, long botId)
		{
			if (!data.TryGetProperty("result", out JsonElement jsonMessage))
				return;

			JsonSerializerOptions jsonOptions = _jsonOptionsProvider.GetJsonSerializerOptions();
			TelegramMessageInfoDto aggregatedMessage = jsonMessage.Deserialize<TelegramMessageInfoDto>(jsonOptions)!;

			await _requestProcessor.ProcessAsync(
				_messageParser, HttpMethod.Post,
				$"bot-{botId}/message", dtoBody: aggregatedMessage);
		}
	}
}
