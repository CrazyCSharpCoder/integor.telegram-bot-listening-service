using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

using IntegorTelegramBotListeningDto;
using IntegorTelegramBotListeningDto.Webhook;

using IntegorTelegramBotListeningShared.Configuration;
using IntegorTelegramBotListeningShared.Bots;
using IntegorTelegramBotListeningShared.ApiRetranslation;
using IntegorTelegramBotListeningShared.ApiRetranslation.ApiContent;

using IntegorTelegramBotListeningModel;

namespace IntegorTelegramBotListeningService.Controllers
{
	using Filters;
	using Helpers;

	[ApiController]
	[Route("bot{botToken}")]
	public class WebhookController : ControllerBase
	{
		private const string _translateWebhookControllerPath = "translateWebhook";

		private const string _setWebhookApiMethod = "setWebhook";
		private const string _deleteWebhookApiMethod = "deleteWebhook";

		private TelegramBotListeningServiceConfiguration _serviceConfiguration;

		private HttpRequestHelper _requestHelper;

		private ITelegramBotApiGate _api;
		private IBotApiHttpContentFactory _contentFactory;
		private IHttpResponseMessageToHttpResponseAssigner _responseToAsp;

		private IBotsManagementService _botsManagement;
		private IBotWebhookManagementService _botWebhooksManagement;

		public WebhookController(
			IOptions<TelegramBotListeningServiceConfiguration> serviceOptions,

			HttpRequestHelper requestHelper,

			ITelegramBotApiGate api,
			IBotApiHttpContentFactory contentFactory,
			IHttpResponseMessageToHttpResponseAssigner responseToAsp,

			IBotsManagementService botsManagement,
			IBotWebhookManagementService botWebhooksManagement)
		{
			_serviceConfiguration = serviceOptions.Value;

			_requestHelper = requestHelper;

			_api = api;
			_contentFactory = contentFactory;
			_responseToAsp = responseToAsp;

			_botsManagement = botsManagement;
			_botWebhooksManagement = botWebhooksManagement;
		}

		[Route("setWebhook")]
		[ServiceFilter(typeof(EntityFrameworkTransactionFilter))]
		public async Task SetWebhookAsync(string botToken, TelegramWebhookDto webhookDto)
		{
			TelegramBotInfoDto? bot = await _botsManagement.GetByTokenAsync(botToken);

			if (bot == null)
				// TODO return error
				throw new System.Exception("Bot with specified id is not registered");

			// Webhook, отправляемый на Telegram Bot API, устанавливается на метод контроллера 
			// TranslateWebhookAsync, который отправляет новый webhook боту
			string botUrl = webhookDto.Url;
			webhookDto.Url = ComposeUrlOfWebhook(botToken);

			// Сначала необходимо добавить в базу данных информацию о webhook,
			// а затем сделать запрос к Telegram

			// В случае, когда добавление в базу данных производится после
			// запроса, возникает ситуация, при которой Telegram отправляет информацию
			// об обновлении сразу после запроса и до того, как информация о боте
			// добавлена в базу данных, соответственно, система не в состоянии перебросить
			// webhook к боту, что явялется ошибкой
			TelegramBotWebhookInfo webhook = new TelegramBotWebhookInfo()
			{
				BotId = bot.Id,
				Url = botUrl
			};
			await _botWebhooksManagement.SetAsync(webhook);

			using HttpContent httpContent = _contentFactory.CreateJsonContent(webhookDto);
			using HttpResponseMessage response = await _api.SendAsync(
				httpContent, new HttpMethod(Request.Method),
				botToken, _setWebhookApiMethod);

			if (!response.IsSuccessStatusCode)
				await _botWebhooksManagement.DeleteAsync(bot.Id);

			await _responseToAsp.AssignAsync(Response, response);
		}

		[Route("deleteWebhook")]
		[ServiceFilter(typeof(EntityFrameworkTransactionFilter))]
		public async Task DeleteWebhookAsync(string botToken, DeleteWebhookDto deleteDto)
		{
			// TODO accept DeleteWebhookDto from query string in snake case notation
			TelegramBotInfoDto? bot = await _botsManagement.GetByTokenAsync(botToken);

			if (bot == null)
				// TODO return error
				throw new System.Exception("Bot with specified id is not registered");

			using HttpContent httpContent = _contentFactory.CreateJsonContent(deleteDto);
			using HttpResponseMessage response = await _api.SendAsync(
				httpContent, new HttpMethod(Request.Method),
				botToken, _deleteWebhookApiMethod);

			if (response.IsSuccessStatusCode)
			{
				TelegramBotWebhookInfo? webhook = await _botWebhooksManagement.GetAsync(bot.Id);

				if (webhook != null)
					await _botWebhooksManagement.DeleteAsync(bot.Id);
			}

			await _responseToAsp.AssignAsync(Response, response);
		}

		// TODO create own class for webhook info with additional information about
		// the system work

		//[Route("getWebhookInfo")]
		//public Task GetWebhookInfoAsync(string botToken)
		//{

		//}

		[HttpPost(_translateWebhookControllerPath)]
		public async Task TranslateWebhookAsync(string botToken)
		{
			TelegramBotInfoDto? bot = await _botsManagement.GetByTokenAsync(botToken);

			if (bot == null)
				// TODO consider in what situations it can happen and handle errors
				throw new System.Exception();

			TelegramBotWebhookInfo? webhook = await _botWebhooksManagement.GetAsync(bot.Id);

			if (webhook == null)
				// TODO consider in what situations it can happen and handle errors
				throw new System.Exception();

			using HttpContent content = await _requestHelper.HttpRequestToHttpContentAsync(Request);
			using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, webhook.Url)
			{
				Content = content
			};

			using HttpClient client = new HttpClient();
			using HttpResponseMessage response = await client.SendAsync(request);

			await _responseToAsp.AssignAsync(Response, response);
		}

		private string ComposeUrlOfWebhook(string botToken)
			=> string.Join('/', _serviceConfiguration.HostedUrl, $"bot{botToken}", _translateWebhookControllerPath);
	}
}
