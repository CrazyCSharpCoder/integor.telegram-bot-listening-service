using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using IntegorTelegramBotListeningShared.ApiContent;

namespace IntegorTelegramBotListeningServices.ApiContent
{
	public class StandardBotApiHttpContentFactory : IBotApiHttpContentFactory
	{
		public HttpContent CreateJsonContent(JsonElement? body)
		{
			return JsonContent.Create(body);
		}

		public HttpContent CreateMultipartFormContent(
			IEnumerable<MultipartFormContent> contentParts, string boundary)
		{
			MultipartFormDataContent content = new MultipartFormDataContent(boundary);

			foreach (MultipartFormContent contentPart in contentParts)
			{
				StreamContent addedContent = new StreamContent(contentPart.Body);
				addedContent.Headers.ContentType = new MediaTypeHeaderValue(contentPart.ContentType);

				if (contentPart.FileName == null)
				{
					content.Add(addedContent, contentPart.Name);
					continue;
				}

				string sentFilename = new string(
					Encoding.UTF8.GetBytes(contentPart.FileName)
					.Select(b => (char)b)
					.ToArray());

				content.Add(addedContent, contentPart.Name, sentFilename);
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
	}
}
