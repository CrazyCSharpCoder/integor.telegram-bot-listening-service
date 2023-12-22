using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

using IntegorTelegramBotListeningShared.ApiRetranslation;
using IntegorTelegramBotListeningShared.ApiRetranslation.ApiContent;

namespace IntegorTelegramBotListeningService.Helpers
{
	public class HttpRequestHelper
	{
		private IBotApiHttpContentFactory _contentFactory;
		private IMultipartContentTypesTransformer _multipartTransformer;

		public HttpRequestHelper(
			IBotApiHttpContentFactory contentFactory,
			IMultipartContentTypesTransformer multipartTransformer)
        {
			_contentFactory = contentFactory;
			_multipartTransformer = multipartTransformer;
        }

        public async Task<HttpContent> HttpRequestToHttpContentAsync(HttpRequest request)
		{
			if (request.HasFormContentType)
			{
				IEnumerable<MultipartFormContentDescriptor>? multipartDescriptors =
					await _multipartTransformer.FormToDescriptorsAsync(request.Form);

				return _contentFactory.CreateMultipartFormContent(
					multipartDescriptors, request.GetMultipartBoundary());
			}

			string? mediaType = null;

			if (request.ContentType != null)
				mediaType = HttpRequestStaticHelpers.GetMediaType(request.ContentType);

			return _contentFactory.CreateDefaultContent(request.Body, mediaType);
		}
	}
}
