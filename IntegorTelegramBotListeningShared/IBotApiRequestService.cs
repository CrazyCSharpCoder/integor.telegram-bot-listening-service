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
			string botToken, string apiMethod, HttpMethod httpMethod, Stream body, IEnumerable<KeyValuePair<string, string>>? queryPrarameters = null);
	}
}
