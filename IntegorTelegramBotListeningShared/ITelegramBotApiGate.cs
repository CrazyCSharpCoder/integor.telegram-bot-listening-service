using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace IntegorTelegramBotListeningShared
{
	public interface ITelegramBotApiGate
	{
		Task<HttpResponseMessage> SendAsync(
			HttpContent content,
			HttpMethod httpMethod,
			string botToken, string apiMethod, Dictionary<string, string>? queryParameters = null);
	}
}
