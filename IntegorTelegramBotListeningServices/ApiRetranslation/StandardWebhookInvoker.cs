using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IntegorTelegramBotListeningShared.ApiRetranslation;

namespace IntegorTelegramBotListeningServices.ApiRetranslation
{
	public class StandardWebhookInvoker : IWebhookInvoker
	{
		public async Task<HttpResponseMessage> InvokeWebhookAsync(string uri, HttpContent content)
		{
			using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri)
			{
				Content = content
			};
			using HttpClient client = new HttpClient();

			return await client.SendAsync(request);
		}
	}
}
