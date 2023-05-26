using System.Text.Json;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using System.IO;

using AutoMapper;

using Microsoft.Extensions.Options;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using IntegorErrorsHandling;
using IntegorErrorsHandling.Filters;
using IntegorErrorsHandling.Converters;

using IntegorSharedResponseDecorators.Shared.Attributes;

using IntegorTelegramBotListeningModel;

using IntegorTelegramBotListeningDto;
using IntegorTelegramBotListeningDto.Webhook;
using IntegorTelegramBotListeningDto.Webhook.WebhookInfo;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.Configuration;
using IntegorTelegramBotListeningShared.Bots;

using IntegorTelegramBotListeningShared.ApiRetranslation;
using IntegorTelegramBotListeningShared.ApiRetranslation.ApiContent;
using IntegorTelegramBotListeningShared.DataDeserialization;

using IntegorTelegramBotListeningShared.ApiAggregation.Aggregators;

namespace IntegorTelegramBotListeningService.Controllers
{
    using Filters;
    using Helpers;
    using Settings.ErrorMessages;

    [ApiController]
	[Route("bot{botToken}")]
	public class WebhookController : ControllerBase
	{
		private const string _translateWebhookControllerPath = "translateWebhook";

		private TelegramBotListeningServiceConfiguration _serviceConfiguration;

		private IMapper _mapper;
		private IStringErrorConverter _stringError;

		private HttpRequestHelper _requestHelper;

		private ITelegramWebhookApi _webhookApi;
		private IWebhookInvoker _webhookInvoker;
		private IBotApiHttpContentFactory _contentFactory;
		private IHttpResponseMessageToHttpResponseAssigner _responseToAsp;

		private IBotInfoAccessor _botAccessor;
		private IBotWebhookManagementService _botWebhooksManagement;

		private IWebhookUpdateDeserializer _updatesDeserializer;
		private IWebhookInfoDeserializer _webhookInfoDeserializer;

		private ITelegramBotWebhookAggregator _updatesAggregator;

		public WebhookController(
			IOptions<TelegramBotListeningServiceConfiguration> serviceOptions,

			IMapper mapper,
			IStringErrorConverter stringError,

			HttpRequestHelper requestHelper,

			ITelegramWebhookApi webhookApi,
			IWebhookInvoker webhookInvoker,

			IBotApiHttpContentFactory contentFactory,
			IHttpResponseMessageToHttpResponseAssigner responseToAsp,

			IBotInfoAccessor botsManagement,
			IBotWebhookManagementService botWebhooksManagement,

			IWebhookUpdateDeserializer updatesDeserializer,
			IWebhookInfoDeserializer webhookInfoDeserializer,

			ITelegramBotWebhookAggregator updatesAggregator)
		{
			_serviceConfiguration = serviceOptions.Value;

			_mapper = mapper;
			_stringError = stringError;

			_requestHelper = requestHelper;

			_webhookApi = webhookApi;
			_webhookInvoker = webhookInvoker;
			_contentFactory = contentFactory;
			_responseToAsp = responseToAsp;

			_botAccessor = botsManagement;
			_botWebhooksManagement = botWebhooksManagement;

			_updatesDeserializer = updatesDeserializer;
			_webhookInfoDeserializer = webhookInfoDeserializer;

			_updatesAggregator = updatesAggregator;
		}

		[ExceptionHandling]
		[Route("setWebhook")]
		[DecorateErrorsResponse]
		[ExtensibleExceptionHandlingLazyFilterFactory]
		[ServiceFilter(typeof(EntityFrameworkTransactionFilter))]
		public async Task<IActionResult> SetWebhookAsync(string botToken, TelegramSetWebhookDto webhookDto)
		{
			// TODO get using IntegorServicesInteractionHelpers and show errors
			TelegramBotInfoDto? bot = await _botAccessor.GetByTokenAsync(botToken);

			if (bot == null)
			{
				IErrorConvertationResult error = _stringError.Convert(
					BotErrorMessages.BotWithIdDoesNotExist)!;

				return BadRequest(error);
			}

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
				BotToken = botToken
			};
			await _botWebhooksManagement.SetAsync(webhook);

			HttpResponseMessage response;

			try
			{
				response = await _webhookApi.SetWebhookAsync(
					botToken, Request.Method, webhookDto);
			}
			catch
			{
				await _botWebhooksManagement.DeleteAsync(botToken);
				throw;
			}

			if (!response.IsSuccessStatusCode)
				await _botWebhooksManagement.DeleteAsync(botToken);

			await _responseToAsp.AssignAsync(Response, response);

			response.Dispose();

			return new EmptyResult();
		}

		[ExceptionHandling]
		[Route("deleteWebhook")]
		[DecorateErrorsResponse]
		[ExtensibleExceptionHandlingLazyFilterFactory]
		[ServiceFilter(typeof(EntityFrameworkTransactionFilter))]
		public async Task<IActionResult> DeleteWebhookAsync(string botToken, DeleteWebhookDto deleteDto)
		{
			// TODO accept DeleteWebhookDto from query string in snake case notation
			TelegramBotInfoDto? bot = await _botAccessor.GetByTokenAsync(botToken);

			if (bot == null)
			{
				IErrorConvertationResult error = _stringError.Convert(
					BotErrorMessages.BotWithIdDoesNotExist)!;

				return BadRequest(error);
			}

			using HttpResponseMessage response =
				await _webhookApi.DeleteWebhookAsync(
					botToken, Request.Method, deleteDto);

			if (response.IsSuccessStatusCode)
			{
				TelegramBotWebhookInfo? webhook =
					await _botWebhooksManagement.GetByBotTokenAsync(botToken);

				if (webhook != null)
					await _botWebhooksManagement.DeleteAsync(botToken);
			}

			await _responseToAsp.AssignAsync(Response, response);

			return new EmptyResult();
		}

		[ExceptionHandling]
		[Route("getWebhookInfo")]
		[DecorateErrorsResponse]
		[ExtensibleExceptionHandlingLazyFilterFactory]
		public async Task<IActionResult> GetWebhookInfoAsync(string botToken)
		{
			using HttpResponseMessage response =
				await _webhookApi.GetWebhookInfoAsync(
					botToken, Request.Method);

			TelegramApiWebhookInfoDto? telegramWebhook = null;

			if (response.IsSuccessStatusCode)
			{
				using Stream content = await response.Content.ReadAsStreamAsync();

				try
				{
					JsonElement jsonContent = await JsonSerializer
						.DeserializeAsync<JsonElement>(content);

					telegramWebhook = _webhookInfoDeserializer.Deserialize(jsonContent);
				}
				catch
				{/* Ignore */}
			}

			WebhookMetaDto? webhookMeta = null;
			TelegramApiWebhookPublicInfoDto? webhookInfo = null;

			TelegramBotWebhookInfo? webhookModel =
				await _botWebhooksManagement.GetByBotTokenAsync(botToken);

			if (webhookModel != null)
			{
				webhookMeta = _mapper
					.Map<TelegramBotWebhookInfo, WebhookMetaDto>(webhookModel);
			}

			if (telegramWebhook != null)
			{
				webhookInfo = _mapper.Map<
					TelegramApiWebhookInfoDto,
					TelegramApiWebhookPublicInfoDto>(

					telegramWebhook);
			}

			WebhookInfoDto webhook = new WebhookInfoDto()
			{
				IsSet = !string.IsNullOrWhiteSpace(telegramWebhook?.Url),

				Meta = webhookMeta,
				TelegramWebhook = webhookInfo
			};

			return Ok(webhook);
		}

		[ExceptionHandling]
		[DecorateErrorsResponse]
		[ExtensibleExceptionHandlingLazyFilterFactory]
		[HttpPost(_translateWebhookControllerPath)]
		[ServiceFilter(typeof(EntityFrameworkTransactionFilter))]
		public async Task<IActionResult> TranslateWebhookAsync(string botToken)
		{
			TelegramBotInfoDto? bot = null;

			try { bot = await _botAccessor.GetByTokenAsync(botToken); }
			catch { /* Ignore */ }

			TelegramBotWebhookInfo? webhook =
				await _botWebhooksManagement.GetByBotTokenAsync(botToken);

			if (webhook == null)
			{
				IErrorConvertationResult error = _stringError.Convert(
					"Webhook was not set via the service")!;

				return BadRequest(error);
			}

			HttpContent content;

			// Если формат данных json, и бот был раннее получен от сервиса с данными
			// (то есть не было проблем с соединением)
			if (bot != null && IsApplicationJson(Request))
			{
				JsonElement jsonBody = await JsonSerializer
					.DeserializeAsync<JsonElement>(Request.Body);

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

			return new EmptyResult();
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
