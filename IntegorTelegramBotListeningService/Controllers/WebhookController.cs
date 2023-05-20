using System.Text.Json;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using IntegorTelegramBotListeningModel;

using IntegorTelegramBotListeningDto;
using IntegorTelegramBotListeningDto.Webhook;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.Configuration;
using IntegorTelegramBotListeningShared.Bots;

using IntegorTelegramBotListeningShared.ApiRetranslation;
using IntegorTelegramBotListeningShared.ApiRetranslation.ApiContent;

using IntegorTelegramBotListeningShared.ApiAggregation.Aggregators;
using IntegorTelegramBotListeningShared.ApiAggregation.DataDeserialization;

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

		private IJsonSerializerOptionsProvider _jsonOptionsProvider;

		private ITelegramBotApiGate _api;
		private IWebhookInvoker _webhookInvoker;
		private IBotApiHttpContentFactory _contentFactory;
		private IHttpResponseMessageToHttpResponseAssigner _responseToAsp;

		private IBotInfoAccessor _botAccessor;
		private IBotWebhookManagementService _botWebhooksManagement;

		private IWebhookUpdateDeserializer _updatesDeserializer;
		private ITelegramBotWebhookAggregator _updatesAggregator;

		public WebhookController(
			IOptions<TelegramBotListeningServiceConfiguration> serviceOptions,

			HttpRequestHelper requestHelper,

			IJsonSerializerOptionsProvider jsonOptionsProvider,

			ITelegramBotApiGate api,
			IWebhookInvoker webhookInvoker,
			IBotApiHttpContentFactory contentFactory,
			IHttpResponseMessageToHttpResponseAssigner responseToAsp,

			IBotInfoAccessor botsManagement,
			IBotWebhookManagementService botWebhooksManagement,

			IWebhookUpdateDeserializer updatesDeserializer,
			ITelegramBotWebhookAggregator updatesAggregator)
		{
			_serviceConfiguration = serviceOptions.Value;

			_requestHelper = requestHelper;

			_jsonOptionsProvider = jsonOptionsProvider;

			_api = api;
			_webhookInvoker = webhookInvoker;
			_contentFactory = contentFactory;
			_responseToAsp = responseToAsp;

			_botAccessor = botsManagement;
			_botWebhooksManagement = botWebhooksManagement;

			_updatesDeserializer = updatesDeserializer;
			_updatesAggregator = updatesAggregator;
		}

		[Route("setWebhook")]
		[ServiceFilter(typeof(EntityFrameworkTransactionFilter))]
		public async Task SetWebhookAsync(string botToken, TelegramWebhookDto webhookDto)
		{
			// TODO get using IntegorServicesInteractionHelpers and show erros
			TelegramBotInfoDto? bot = await _botAccessor.GetByTokenAsync(botToken);

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
				Id = bot.Id,
				Url = botUrl,
				BotTokenCache = botToken
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
			TelegramBotInfoDto? bot = await _botAccessor.GetByTokenAsync(botToken);

			if (bot == null)
				// TODO return error
				throw new System.Exception("Bot with specified id is not registered");

			using HttpContent httpContent = _contentFactory.CreateJsonContent(deleteDto);
			using HttpResponseMessage response = await _api.SendAsync(
				httpContent, new HttpMethod(Request.Method),
				botToken, _deleteWebhookApiMethod);

			if (response.IsSuccessStatusCode)
			{
				TelegramBotWebhookInfo? webhook = await _botWebhooksManagement.GetByIdAsync(bot.Id);

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
		[ServiceFilter(typeof(EntityFrameworkTransactionFilter))]
		public async Task TranslateWebhookAsync(string botToken)
		{
			TelegramBotInfoDto? bot = null;
			bool connectionFailed = false;

			try { bot = await _botAccessor.GetByTokenAsync(botToken); }
			catch { connectionFailed = true; }

			TelegramBotWebhookInfo? webhook;

			if (connectionFailed)
			{
				// Если не удалось установить соединение с сервисом,
				// используем закэшированную информацию

				webhook = await _botWebhooksManagement.GetByTokenCacheAsync(botToken);
			}
			else
			{
				if (bot == null)
					// TODO consider in what situations it can happen and handle errors
					throw new System.Exception();

				webhook = await _botWebhooksManagement.GetByIdAsync(bot.Id);
			}

			if (webhook == null)
				// TODO consider in what situations it can happen and handle errors
				throw new System.Exception();

			HttpContent content;

			// Если формат данных json, и бот был раннее получен от сервиса с данными
			// (то есть не было проблем с соединением)
			if (bot != null && IsApplicationJson(Request))
			{
				JsonSerializerOptions jsonOptions = _jsonOptionsProvider.GetJsonSerializerOptions();
				JsonElement jsonBody = await JsonSerializer.DeserializeAsync<JsonElement>(Request.Body, options: jsonOptions);

				try { await AggregateWebhookAsync(jsonBody, bot.Id); }
				catch { /* Ignore */ }

				content = _contentFactory.CreateJsonContent(jsonBody);
			}
			else
			{
				content = await _requestHelper.HttpRequestToHttpContentAsync(Request);
			}

			using HttpResponseMessage response =
				await _webhookInvoker.InvokeWebhookAsync(webhook.Url, content);

			await _responseToAsp.AssignAsync(Response, response);
		}

		private string ComposeUrlOfWebhook(string botToken)
			=> string.Join('/',
				_serviceConfiguration.HostedUrl,
				$"bot{botToken}", _translateWebhookControllerPath);

		private async Task AggregateWebhookAsync(JsonElement jsonBody, int botId)
		{
			TelegramUpdateDto? update;

			try { update = _updatesDeserializer.Deserialize(jsonBody); }
			catch { return; } // Impossible to aggregate

			if (update == null)
				return;

			try { await _updatesAggregator.AggregateAsync(update, botId); }
			catch { /*Ignore*/ }
		}

		private bool IsApplicationJson(HttpRequest request)
		{
			if (request.ContentType == null)
				return false;

			string mediaType = HttpRequestStaticHelpers.GetMediaType(request.ContentType);
			
			return mediaType == MediaTypeNames.Application.Json;
		}
	}
}
