using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;

using IntegorTelegramBotListeningShared.ApiRetranslation;

namespace IntegorTelegramBotListeningServices.ApiRetranslation
{
    public class StandardHttpResponseMessageToHttpResponseAssigner : IHttpResponseMessageToHttpResponseAssigner
    {
		public async Task AssignAsync(HttpResponse target, HttpStatusCode statusCode,
			HttpContent contentSource, HttpResponseHeaders headers)
		{
			target.StatusCode = HttpStatusCodeToInt(statusCode);

			AssignHeaders(target.Headers, headers);
			await AssignContentAsync(target, contentSource);
		}

		public async Task AssignAsync(HttpResponse target, HttpResponseMessage source)
        {
            target.StatusCode = HttpStatusCodeToInt(source.StatusCode);

            AssignHeaders(target.Headers, source.Headers);
            await AssignContentAsync(target, source.Content);
        }

		private int HttpStatusCodeToInt(HttpStatusCode statusCode) => (int)statusCode;

        private void AssignHeaders(IHeaderDictionary target, HttpResponseHeaders source)
        {
			target.Clear();

            foreach (KeyValuePair<string, IEnumerable<string>> headerToValues in source)
                target[headerToValues.Key] = new StringValues(headerToValues.Value.ToArray());
        }

        private async Task AssignContentAsync(HttpResponse targetResponse, HttpContent contentSource)
        {
            if (contentSource.Headers.ContentType != null)
                targetResponse.ContentType = contentSource.Headers.ContentType.MediaType;

            await contentSource.CopyToAsync(targetResponse.Body);
        }
    }
}
