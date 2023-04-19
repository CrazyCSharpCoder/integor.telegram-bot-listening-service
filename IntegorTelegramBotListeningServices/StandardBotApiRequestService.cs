using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;

using Microsoft.Extensions.Options;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.Configuration;

namespace IntegorTelegramBotListeningServices
{
	public class StandardBotApiRequestService : IBotApiRequestService
	{
		private TelegramBotApiConfiguration _apiConfiguration;

		public StandardBotApiRequestService(IOptions<TelegramBotApiConfiguration> apiOptions)
        {
			_apiConfiguration = apiOptions.Value;
        }

		public async Task<HttpResponseMessage> RequestApiAsync(
			string botToken, string apiMethod, HttpMethod httpMethod, Stream body,
			string? contentMediaType = null, IEnumerable<KeyValuePair<string, string>>? queryParameters = null)
		{
			Uri uri = BuildApiUri(_apiConfiguration.Domain, botToken, apiMethod, queryParameters);

			using HttpClient client = new HttpClient();

			using HttpContent content = new StreamContent(body);

			if (contentMediaType != null)
				content.Headers.ContentType = new MediaTypeHeaderValue(contentMediaType);

			using HttpRequestMessage request = new HttpRequestMessage(httpMethod, uri)
			{
				Content = content,
			};

			return await client.SendAsync(request);
		}

		private Uri BuildApiUri(string telegramBotApiDomain, string botToken, string apiMethod, IEnumerable<KeyValuePair<string, string>>? queryParameters)
		{
			NameValueCollection queryParams = HttpUtility.ParseQueryString(string.Empty);

			if (queryParameters != null)
			{
				foreach (KeyValuePair<string, string> param in queryParameters)
					queryParams[param.Key] = param.Value;
			}

			UriBuilder uriBuilder = new UriBuilder(telegramBotApiDomain)
			{
				Path = $"bot{botToken}/{apiMethod}",
				Query = queryParams.ToString()
			};

			return uriBuilder.Uri;
		}
	}
}
