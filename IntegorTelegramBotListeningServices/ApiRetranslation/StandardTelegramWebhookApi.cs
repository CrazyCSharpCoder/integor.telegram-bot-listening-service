using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;

using IntegorTelegramBotListeningDto.Webhook;
using IntegorTelegramBotListeningShared.ApiRetranslation;
using IntegorTelegramBotListeningShared.ApiRetranslation.ApiContent;

namespace IntegorTelegramBotListeningServices.ApiRetranslation
{
	public class StandardTelegramWebhookApi : ITelegramWebhookApi
	{
		private const string _setWebhookApiMethod = "setWebhook";
		private const string _deleteWebhookApiMethod = "deleteWebhook";
		private const string _getWebhookApiMethod = "getWebhookInfo";

		private IBotApiHttpContentFactory _contentFactory;
		private ITelegramBotApiGate _api;

		public StandardTelegramWebhookApi(
			IBotApiHttpContentFactory contentFactory,
			ITelegramBotApiGate api)
        {
			_contentFactory = contentFactory;
			_api = api;
        }

		public async Task<HttpResponseMessage> GetWebhookInfoAsync(string botToken, string httpMethod)
		{
			return await _api.SendAsync(
				null, new HttpMethod(httpMethod),
				botToken, _getWebhookApiMethod);
		}

		public async Task<HttpResponseMessage> SetWebhookAsync(
			string botToken, string httpMethod, TelegramSetWebhookDto webhookDto)
		{
			using HttpContent httpContent =
				_contentFactory.CreateJsonContent(webhookDto);

			return await _api.SendAsync(
				httpContent, new HttpMethod(httpMethod),
				botToken, _setWebhookApiMethod);
		}

		public async Task<HttpResponseMessage> DeleteWebhookAsync(
			string botToken, string httpMethod, DeleteWebhookDto deleteDto)
		{
			using HttpContent httpContent =
				_contentFactory.CreateJsonContent(deleteDto);

			return await _api.SendAsync(
				httpContent, new HttpMethod(httpMethod),
				botToken, _deleteWebhookApiMethod);
		}
	}
}
