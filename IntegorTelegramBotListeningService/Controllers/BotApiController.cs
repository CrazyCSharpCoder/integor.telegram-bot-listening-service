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
		private class BotApiActionDescriptor
		{
			public string Domain { get; }

			public string BotToken { get; }
			public string ApiMethod { get; }
			public HttpMethod HttpMethod { get; }

			public string? QuestionQueryString { get; }

            public BotApiActionDescriptor(
				string domain, string token, string apiMethod,
				HttpMethod httpMethod, string? questionQueryString)
            {
				Domain = domain;

				BotToken = token;
				ApiMethod = apiMethod;
				HttpMethod = httpMethod;

				QuestionQueryString = questionQueryString;
            }

			public string ToUri()
			{
				return Path.Combine(Domain, $"bot{BotToken}", ApiMethod) + QuestionQueryString;
			}
        }

		private IBotApiHttpContentFactory _contentFactory;
		private IHttpResponseMessageToHttpResponseAssigner _responseToAsp;

		public BotApiController(
			IBotApiHttpContentFactory contentFactory,
			IHttpResponseMessageToHttpResponseAssigner responseToActionResult)
        {
			_contentFactory = contentFactory;
			_responseToAsp = responseToActionResult;
        }

		[Route("bot{botToken}/{apiMethod}")]
		public async Task ListenToBotAsync(string botToken, string apiMethod)
		{
			string? mediaType = Request.ContentType == null ? null : GetMediaType(Request.ContentType);

			// TODO place domain to config file
			BotApiActionDescriptor apiAction = new BotApiActionDescriptor(
				"https://api.telegram.org/", botToken, apiMethod,
				new HttpMethod(Request.Method), Request.QueryString.Value);

			HttpResponseMessage response;

			if (Request.HasFormContentType)
				response = await TranslateMultipartFormAsync(apiAction, Request.Form, Request.GetMultipartBoundary());
			else
				response = await TranslateDefaultAsync(apiAction, Request.Body, mediaType);			

			await _responseToAsp.AssignAsync(Response, response);

			response.Dispose();
		}

		private string GetMediaType(string contentType)
			=> contentType.Split(";", 2).First().Trim();

		private async Task<HttpResponseMessage> TranslateMultipartFormAsync(
			BotApiActionDescriptor apiAction, IFormCollection formData, string boundary)
		{
			IEnumerable<MultipartFormContent> multipartContentParts = await CopyFormValues(formData);
			multipartContentParts = multipartContentParts.Concat(CopyFiles(formData.Files));

			using HttpContent content = _contentFactory.CreateMultipartFormContent(multipartContentParts, boundary);
			HttpResponseMessage response = await SendAsync(apiAction, content);

			foreach (MultipartFormContent multipartContent in multipartContentParts)
				await multipartContent.DisposeAsync();

			return response;
		}

		private async Task<HttpResponseMessage> TranslateDefaultAsync(
			BotApiActionDescriptor apiAction, Stream body, string? mediaType)
		{
			using HttpContent content = _contentFactory.CreateDefaultContent(body, mediaType);
			return await SendAsync(apiAction, content);
		}

		private async Task<HttpResponseMessage> SendAsync(
			BotApiActionDescriptor apiAction, HttpContent content)
		{
			using HttpClient client = new HttpClient();
			using HttpRequestMessage request = new HttpRequestMessage(apiAction.HttpMethod, apiAction.ToUri())
			{
				Content = content
			};

			return await client.SendAsync(request);
		}

		private async Task<IEnumerable<MultipartFormContent>> CopyFormValues(IFormCollection form)
		{
			List<MultipartFormContent> content = new List<MultipartFormContent>();

			foreach (var formValue in form)
			{
				Stream body = new MemoryStream();
				StreamWriter writer = new StreamWriter(body);

				// TODO pass all values instead of First()
				await writer.WriteAsync(formValue.Value.First().ToString());
				await writer.FlushAsync();

				body.Position = 0;

				MultipartFormContent multipartContent =
					new MultipartFormContent("text/plain", formValue.Key, body);

				content.Add(multipartContent);
			}

			return content;
		}

		private IEnumerable<MultipartFormContent> CopyFiles(IFormFileCollection files)
		{
			List<MultipartFormContent> content = new List<MultipartFormContent>();

			foreach (var file in files)
			{
				Stream body = file.OpenReadStream();

				MultipartFormContent multipartContent =
					new MultipartFormContent("application/octet-stream", file.Name, body, file.FileName);

				content.Add(multipartContent);
			}

			return content;
		}
	}
}
