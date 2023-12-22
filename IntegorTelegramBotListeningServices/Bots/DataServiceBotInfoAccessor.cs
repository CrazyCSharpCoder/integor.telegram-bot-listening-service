using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

using Microsoft.Extensions.Options;

using IntegorErrorsHandling;
using IntegorResponseDecoration.Parsing;

using IntegorServicesInteraction;
using IntegorServicesInteractionHelpers;

using ExternalServicesConfiguration;

using IntegorTelegramBotListeningDto;
using IntegorTelegramBotListeningShared.Bots;

namespace IntegorTelegramBotListeningServices.Bots
{
    public class DataServiceBotInfoAccessor : IBotInfoAccessor
    {
		private IDecoratedObjectParser<TelegramBotInfoDto, JsonElement> _botParser;
		private JsonServicesRequestProcessor<IntegorDataServiceConfiguration> _requestProcessor;

		public DataServiceBotInfoAccessor(
			IOptions<IntegorDataServiceConfiguration> dataServiceOptions,

			IDecoratedObjectParser<TelegramBotInfoDto, JsonElement> botParser,
			IDecoratedObjectParser<IEnumerable<IResponseError>, JsonElement> errorsParser)
        {
			_botParser = botParser;
			_requestProcessor = new JsonServicesRequestProcessor<IntegorDataServiceConfiguration>(
				errorsParser, dataServiceOptions, "bots/unsafe");
		}

		public async Task<TelegramBotInfoDto?> GetByTokenAsync(string botToken)
		{
			ServiceResponse<TelegramBotInfoDto> response =
				await _requestProcessor.ProcessAsync(_botParser, HttpMethod.Get, botToken);

			return response.Value;
		}
	}
}
