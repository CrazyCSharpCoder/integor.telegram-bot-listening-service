using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

namespace IntegorTelegramBotListeningShared
{
	public interface IBotApiRequestService
	{
		Task<HttpResponseMessage> RequestApiAsync(
			string botToken, string apiMethod, HttpMethod httpMethod,
			Stream body,
			string? contentMediaType = null,
			IEnumerable<KeyValuePair<string, string>>? queryParameters = null);
	}
}
