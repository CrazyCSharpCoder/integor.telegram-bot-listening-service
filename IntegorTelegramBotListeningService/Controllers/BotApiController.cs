using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.IO;
using System.Net.Http;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

using Microsoft.AspNetCore.Mvc;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.ApiContent;

namespace IntegorTelegramBotListeningService.Controllers
{
	[ApiController]
	public class BotApiController : ControllerBase
	{
		private IBotApiHttpContentFactory _contentFactory;
		private ITelegramBotApiGate _api;
		private IHttpResponseMessageToHttpResponseAssigner _responseToAsp;

		public BotApiController(
			IBotApiHttpContentFactory contentFactory,
			ITelegramBotApiGate api,
			IHttpResponseMessageToHttpResponseAssigner responseToActionResult)
        {
			_contentFactory = contentFactory;
			_api = api;
			_responseToAsp = responseToActionResult;
        }

		[Route("bot{botToken}/{apiMethod}")]
		public async Task ListenToBotAsync(string botToken, string apiMethod)
		{
			string? mediaType = Request.ContentType == null ? null : GetMediaType(Request.ContentType);

			HttpContent content;
			IEnumerable<MultipartFormContentDescriptor>? multipartContent = null;

			if (Request.HasFormContentType)
			{
				multipartContent = await ExtractFormValuesAsync(Request.Form);
				multipartContent = multipartContent.Concat(ExtractFormFiles(Request.Form.Files));

				content = _contentFactory.CreateMultipartFormContent(multipartContent, Request.GetMultipartBoundary());
			}
			else
			{
				content = _contentFactory.CreateDefaultContent(Request.Body, mediaType);
			}

			// TODO get rid of First()
			Dictionary<string, string> query = Request.Query.ToDictionary(
				queryToValue => queryToValue.Key, queryToValue => queryToValue.Value.First());

			using HttpResponseMessage response = await _api.SendAsync(
				content, new HttpMethod(Request.Method), botToken, apiMethod, query);

			await _responseToAsp.AssignAsync(Response, response);

			content.Dispose();

			if (multipartContent == null)
				return;

			foreach (MultipartFormContentDescriptor disposedMultipart in multipartContent)
				await disposedMultipart.DisposeAsync();
		}

		private string GetMediaType(string contentType)
			=> contentType.Split(";", 2).First().Trim();

		private async Task<IEnumerable<MultipartFormContentDescriptor>> ExtractFormValuesAsync(IFormCollection form)
		{
			List<MultipartFormContentDescriptor> content = new List<MultipartFormContentDescriptor>();

			foreach (var formValue in form)
			{
				Stream body = new MemoryStream();
				StreamWriter writer = new StreamWriter(body);

				// TODO pass all values instead of First()
				await writer.WriteAsync(formValue.Value.First().ToString());
				await writer.FlushAsync();

				body.Position = 0;

				MultipartFormContentDescriptor multipartContent =
					new MultipartFormContentDescriptor("text/plain", formValue.Key, body);

				content.Add(multipartContent);
			}

			return content;
		}

		private IEnumerable<MultipartFormContentDescriptor> ExtractFormFiles(IFormFileCollection files)
		{
			List<MultipartFormContentDescriptor> content = new List<MultipartFormContentDescriptor>();

			foreach (var file in files)
			{
				Stream body = file.OpenReadStream();

				MultipartFormContentDescriptor multipartContent =
					new MultipartFormContentDescriptor("application/octet-stream", file.Name, body, file.FileName);

				content.Add(multipartContent);
			}

			return content;
		}
	}
}
