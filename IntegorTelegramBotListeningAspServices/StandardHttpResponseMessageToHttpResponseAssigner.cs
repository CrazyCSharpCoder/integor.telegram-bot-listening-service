using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

using IntegorTelegramBotListeningAspShared;

namespace IntegorTelegramBotListeningAspServices
{
	public class StandardHttpResponseMessageToHttpResponseAssigner : IHttpResponseMessageToHttpResponseAssigner
	{
		public async Task AssignAsync(HttpResponse httpResponse, HttpResponseMessage responseMessage)
		{
			httpResponse.StatusCode = (int)responseMessage.StatusCode;

			CopyHeaders(httpResponse.Headers, responseMessage.Headers);
			await responseMessage.Content.CopyToAsync(httpResponse.Body);
		}

		private void CopyHeaders(IHeaderDictionary target, HttpResponseHeaders source)
		{
			foreach (KeyValuePair<string, IEnumerable<string>> headerToValues in source)
				target[headerToValues.Key] = new StringValues(headerToValues.Value.ToArray());
		}
	}
}
