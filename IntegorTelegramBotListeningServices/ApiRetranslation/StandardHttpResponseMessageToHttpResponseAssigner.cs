using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;

using IntegorTelegramBotListeningShared.ApiRetranslation;

namespace IntegorTelegramBotListeningServices.ApiRetranslation
{
    public class StandardHttpResponseMessageToHttpResponseAssigner : IHttpResponseMessageToHttpResponseAssigner
    {
        public async Task AssignAsync(HttpResponse target, HttpResponseMessage source)
        {
            target.StatusCode = (int)source.StatusCode;

            AssignHeaders(target.Headers, source.Headers);
            await AssignContentAsync(target, source.Content);
        }

        private void AssignHeaders(IHeaderDictionary target, HttpResponseHeaders source)
        {
            foreach (KeyValuePair<string, IEnumerable<string>> headerToValues in source)
                target[headerToValues.Key] = new StringValues(headerToValues.Value.ToArray());
        }

        private async Task AssignContentAsync(HttpResponse targetResponse, HttpContent contentSource)
        {
            if (contentSource.Headers.ContentType != null)
                targetResponse.ContentType = contentSource.Headers.ContentType.MediaType;

            if (contentSource.Headers.ContentLength != null)
                targetResponse.ContentLength = contentSource.Headers.ContentLength;

            //await contentSource.CopyToAsync(targetResponse.Body);

            string content = await contentSource.ReadAsStringAsync();
            await targetResponse.WriteAsync(content);
        }
    }
}
