using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.IO;
using System.Net.Http;

using Microsoft.AspNetCore.Mvc;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.ApiRetranslation;

namespace IntegorTelegramBotListeningService.Controllers
{
	using Filters;
	using Helpers;

    [ApiController]
	public class BotApiController : ControllerBase
	{
		private HttpRequestHelper _requestHelper;

		private ITelegramBotApiGate _api;
		private IHttpResponseMessageToHttpResponseAssigner _responseToAsp;

		private ITelegramBotEventsAggregator _eventsAggregator;

		public BotApiController(
			HttpRequestHelper requestHelper,

			ITelegramBotApiGate api,
			IHttpResponseMessageToHttpResponseAssigner responseToActionResult,

			ITelegramBotEventsAggregator eventsAggregator)
        {
			_requestHelper = requestHelper;

			_api = api;
			_responseToAsp = responseToActionResult;

			_eventsAggregator = eventsAggregator;
        }

		[Route("bot{botToken}/{apiMethod}")]
		[IgnoreExceptionFilter]
		[ServiceFilter(typeof(EntityFrameworkTransactionFilter))]
		public async Task ListenToBotAsync(string botToken, string apiMethod)
		{
			using HttpContent content = await _requestHelper.HttpRequestToHttpContentAsync(Request);

			// TODO get rid of First()
			Dictionary<string, string> query = Request.Query.ToDictionary(
				queryToValue => queryToValue.Key, queryToValue => queryToValue.Value.First());

			using HttpResponseMessage response = await _api.SendAsync(
				content, new HttpMethod(Request.Method), botToken, apiMethod, query);

			string? responseMediaType = response.Content.Headers.ContentType?.MediaType;

			if (response.IsSuccessStatusCode && await _eventsAggregator.AllowAggregationAsync(botToken, apiMethod, responseMediaType))
			{
				Stream responseBody = await response.Content.ReadAsStreamAsync();

				try { await _eventsAggregator.AggregateAsync(responseBody); }
				catch { }

				responseBody.Position = 0;
			}

			await _responseToAsp.AssignAsync(Response, response);
		}
	}
}
