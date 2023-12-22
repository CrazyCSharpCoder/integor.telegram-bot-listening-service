using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;

using System.IO;
using System.Net.Http;
using System.Net.Mime;

using Microsoft.AspNetCore.Mvc;

using IntegorTelegramBotListeningDto;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.Bots;

using IntegorTelegramBotListeningShared.ApiRetranslation;
using IntegorTelegramBotListeningShared.ApiRetranslation.ApiContent;
using IntegorTelegramBotListeningShared.ApiAggregation.Aggregators;

using IntegorTelegramBotListeningShared.DataDeserialization;

namespace IntegorTelegramBotListeningService.Controllers
{
    using Filters;
    using Helpers;

    [ApiController]
	public class BotApiController : ControllerBase
	{
		private const string _getUpdatesApiMethodName = "getUpdates";

		private HttpRequestHelper _requestHelper;

		private IJsonSerializerOptionsProvider _jsonOptionsProvider;

		private ITelegramBotApiGate _api;
		private IBotApiHttpContentFactory _contentFactory;
		private IHttpResponseMessageToHttpResponseAssigner _responseToAsp;

		private IBotInfoAccessor _botsManagement;

		private ILongPollingUpdatesDeserializer _updatesDeserializer;
		private ITelegramBotLongPollingAggregator _updatesAggregator;
		private ITelegramBotApiAggregator _apiAggregator;

		public BotApiController(
			HttpRequestHelper requestHelper,

			IJsonSerializerOptionsProvider jsonOptionsProvider,

			ITelegramBotApiGate api,
			IBotApiHttpContentFactory contentFactory,
			IHttpResponseMessageToHttpResponseAssigner responseToAsp,

			IBotInfoAccessor botsManagement,

			ILongPollingUpdatesDeserializer updatesDeserializer,
			ITelegramBotLongPollingAggregator updatesAggregator,
			ITelegramBotApiAggregator apiAggregator)
        {
			_requestHelper = requestHelper;
			_contentFactory = contentFactory;

			_jsonOptionsProvider = jsonOptionsProvider;

			_api = api;
			_responseToAsp = responseToAsp;

			_botsManagement = botsManagement;

			_updatesDeserializer = updatesDeserializer;
			_updatesAggregator = updatesAggregator;

			_apiAggregator = apiAggregator;
		}

		[IgnoreExceptionFilter]
		[Route("bot{botToken}/{apiMethod}")]
		[ServiceFilter(typeof(EntityFrameworkTransactionFilter))]
		public async Task ListenToBotAsync(string botToken, string apiMethod)
		{
			using HttpContent content = await _requestHelper.HttpRequestToHttpContentAsync(Request);

			// TODO get rid of First()
			Dictionary<string, string> query = Request.Query.ToDictionary(
				queryToValue => queryToValue.Key, queryToValue => queryToValue.Value.First());

			using HttpResponseMessage response = await _api.SendAsync(
				content, new HttpMethod(Request.Method), botToken, apiMethod, query);

			if (!response.IsSuccessStatusCode)
			{
				await _responseToAsp.AssignAsync(Response, response);
				return;
			}

			long? botId = await GetBotIdSafeAsync(botToken);

			if (botId == null || !IsApplicationJson(response.Content))
			{
				await _responseToAsp.AssignAsync(Response, response);
				return;
			}

			using Stream streamResponseBody = await response.Content.ReadAsStreamAsync();
			JsonSerializerOptions jsonOptions = _jsonOptionsProvider.GetJsonSerializerOptions();

			JsonElement jsonBody = await JsonSerializer.DeserializeAsync<JsonElement>(
				streamResponseBody, options: jsonOptions);

			if (apiMethod.ToLower() == _getUpdatesApiMethodName.ToLower())
			{
				try { await AggregateUpdatesAsync(jsonBody, botId.Value); }
				catch { /* Ignore */ }
			}
			else if (IsApplicationJson(response.Content) &&
				await _apiAggregator.AllowAggregationAsync(apiMethod))
			{
				try { await _apiAggregator.AggregateAsync(jsonBody, botId.Value); }
				catch { /* Ignore */}
			};

			using HttpContent responseContent = _contentFactory.CreateJsonContent(jsonBody);

			await _responseToAsp.AssignAsync(Response,
				response.StatusCode, responseContent, response.Headers);
		}

		private async Task<long?> GetBotIdSafeAsync(string botToken)
		{
			try
			{
				return (await _botsManagement.GetByTokenAsync(botToken))?.Id;
			}
			catch
			{
				return null;
			}
		}

		private async Task AggregateUpdatesAsync(JsonElement updatesJson, long botId)
		{
			IEnumerable<TelegramUpdateDto>? updates;

			try { updates = _updatesDeserializer.Deserialize(updatesJson); }
			catch { return; } // Impossible to aggregate

			if (updates == null)
				return;

			try { await _updatesAggregator.AggregateAsync(updates, botId); }
			catch { /*Ignore*/ }
		}

		private bool IsApplicationJson(HttpContent content)
			=> content.Headers.ContentType?.MediaType == MediaTypeNames.Application.Json;
	}
}
