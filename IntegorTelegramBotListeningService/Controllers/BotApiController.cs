using System;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

using Microsoft.AspNetCore.Mvc;

using IntegorTelegramBotListeningShared;

using IntegorTelegramBotListeningAspShared;

namespace IntegorTelegramBotListeningService.Controllers
{
	[ApiController]
	[Route("bot")]
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

        [Route("{token}/{method}")]
		public async Task ListenBotAsync(string token, string method)
		{
			HttpResponseMessage response;
			var queryParams = Request.Query.Select(pairQueryParam => new KeyValuePair<string, string>(pairQueryParam.Key, pairQueryParam.Value));
			
			try
			{
				response = await _requestService.RequestApiAsync(
					token, method, new HttpMethod(Request.Method), Request.Body, queryParams);
			}
			catch (Exception exc)
			{
				throw;
			}

			await _responseToActionResult.AssignAsync(Response, response);
			await Response.StartAsync();

			response.Dispose();
		}
	}
}
