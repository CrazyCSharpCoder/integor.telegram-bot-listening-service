using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningShared.ApiRetranslation
{
	public interface IWebhookInvoker
	{
		Task<HttpResponseMessage> InvokeWebhookAsync(string uri, HttpContent content);
	}
}
