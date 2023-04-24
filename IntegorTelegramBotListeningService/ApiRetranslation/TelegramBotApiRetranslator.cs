using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace IntegorTelegramBotListeningService.ApiRetranslation
{
	public class TelegramBotApiRetranslator
	{
		private const string _telegramBotApiDomain = "https://api.telegram.org/";

        public async Task Invoke(HttpContext context)
		{
			string? botToken = context.Request.RouteValues["token"]?.ToString();
			string? apiMethod = context.Request.RouteValues["apiMethod"]?.ToString();

			if (botToken == null || apiMethod == null)
				return;

			await Console.Out.WriteLineAsync($"{botToken}; {apiMethod}");

			string uri = CreateTelegramUri(context.Request);

			using HttpContent content = context.Request.HasFormContentType ?
				await CreateMultipartFormDataContentAsync(context.Request) :
				CreateDefaultContent(context.Request);

			using HttpRequestMessage request = new HttpRequestMessage(
				new HttpMethod(context.Request.Method), uri)
			{
				Content = content
			};

			using HttpClient client = new HttpClient();

			using HttpResponseMessage response = await client.SendAsync(request);

			context.Response.StatusCode = (int)response.StatusCode;

			if (response.Content.Headers.ContentType != null)
				context.Response.Headers.ContentType = response.Content.Headers.ContentType.MediaType;

			await response.Content.CopyToAsync(context.Response.Body);
		}

		private HttpContent CreateDefaultContent(HttpRequest requestSource)
		{
			HttpContent content = new StreamContent(requestSource.Body);

			if (requestSource.ContentType != null)
				content.Headers.ContentType = new MediaTypeHeaderValue(GetMediaType(requestSource.ContentType));

			return content;
		}

		private async Task<MultipartFormDataContent> CreateMultipartFormDataContentAsync(HttpRequest requestSource)
		{
			//MultipartFormDataParser multipartContent = await MultipartFormDataParser.ParseAsync(requestSource.Body, Encoding.Default);

			return new MultipartFormDataContent(requestSource.GetMultipartBoundary())
				{
					new StreamContent(requestSource.Body)
				};
		}

		private string GetMediaType(string strContentType)
			=> strContentType.Split(";", 2).First().Trim();

		private string CreateTelegramUri(HttpRequest request)
		{
			string uri = Path.Combine(_telegramBotApiDomain, request.Path.Value?.Trim('/') ?? string.Empty);

			if (request.QueryString.HasValue)
				uri += request.QueryString.Value;

			return uri;
		}
	}
}
