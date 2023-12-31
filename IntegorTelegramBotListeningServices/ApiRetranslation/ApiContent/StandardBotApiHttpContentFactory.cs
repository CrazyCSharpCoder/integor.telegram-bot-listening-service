﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.ApiRetranslation.ApiContent;

namespace IntegorTelegramBotListeningServices.ApiRetranslation.ApiContent
{
    public class StandardBotApiHttpContentFactory : IBotApiHttpContentFactory
    {
		private IJsonSerializerOptionsProvider _jsonOptionsProvider;

		public StandardBotApiHttpContentFactory(
			IJsonSerializerOptionsProvider jsonOptionsProvider)
        {
			_jsonOptionsProvider = jsonOptionsProvider;
        }

        public HttpContent CreateMultipartFormContent(
            IEnumerable<MultipartFormContentDescriptor> contentParts, string boundary)
        {
			MultipartFormDataContent content = new MultipartFormDataContent(boundary);

            foreach (MultipartFormContentDescriptor contentPart in contentParts)
            {
                StreamContent addedContent = new StreamContent(contentPart.Body);
                addedContent.Headers.ContentType = new MediaTypeHeaderValue(contentPart.ContentType);

                ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("form-data");
                contentDisposition.Parameters.Add(new NameValueHeaderValue("name", contentPart.Name));

                if (contentPart.FileName != null)
                    contentDisposition.Parameters.Add(new NameValueHeaderValue("filename", contentPart.FileName));

                addedContent.Headers.ContentDisposition = contentDisposition;

                content.Add(addedContent);
            }

            return content;
        }
		
		public HttpContent CreateDefaultContent(Stream body, string? mediaType)
        {
            HttpContent content = new StreamContent(body);

            if (mediaType != null)
                content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

            return content;
        }

		public HttpContent CreateJsonContent<TBody>(TBody body)
		{
			JsonSerializerOptions options =
				_jsonOptionsProvider.GetJsonSerializerOptions();

			return JsonContent.Create(body, options: options);
		}
	}
}
