using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IntegorTelegramBotListeningShared;

namespace IntegorTelegramBotListeningServices
{
	public class StandardTelegramBotApiGate : ITelegramBotApiGate
	{
		private ITelegramBotApiUriBuilder _uriBuilder;

		public StandardTelegramBotApiGate(ITelegramBotApiUriBuilder uriBuilder)
		{
			_uriBuilder = uriBuilder;
		}

		public async Task<HttpResponseMessage> SendAsync(HttpContent content, HttpMethod httpMethod, string botToken, string apiMethod, Dictionary<string, string>? queryParameters = null)
		{
			string uri = _uriBuilder.CreateUri(botToken, apiMethod, queryParameters);

			using HttpRequestMessage request = new HttpRequestMessage(httpMethod, uri) { Content = content };
			using HttpClient client = new HttpClient();

			return await client.SendAsync(request);
		}
	}
}
