using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

using Microsoft.AspNetCore.Mvc;

using IntegorTelegramBotListeningShared;

using IntegorTelegramBotListeningAspShared;

namespace IntegorTelegramBotListeningService.Controllers
{
	[ApiController]
	public class BotApiController : ControllerBase
	{
		private IBotApiRequestService _requestService;
		private IHttpResponseMessageToHttpResponseAssigner _responseToActionResult;

		public BotApiController(
			IBotApiRequestService requestService,
			IHttpResponseMessageToHttpResponseAssigner responseToActionResult)
        {
			_requestService = requestService;
			_responseToActionResult = responseToActionResult;
        }

        [Route("bot{token}/{method}")]
		public async Task ListenBotAsync(string token, string method)
		{
			var queryParams = Request.Query.Select(pairQueryParam => new KeyValuePair<string, string>(pairQueryParam.Key, pairQueryParam.Value));
			string? contentMediaType = Request.ContentType?.Split(";", 2)[0].Trim();

			using HttpResponseMessage response = await _requestService.RequestApiAsync(
				token, method, new HttpMethod(Request.Method), Request.Body, contentMediaType, queryParams);

			await _responseToActionResult.AssignAsync(Response, response);
		}
	}
}
