using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.AspNetCore.Http;

using IntegorTelegramBotListeningShared.ApiRetranslation;
using IntegorTelegramBotListeningShared.ApiRetranslation.ApiContent;

namespace IntegorTelegramBotListeningServices.ApiRetranslation
{
	public class StandardMultipartContentTypesTransformer : IMultipartContentTypesTransformer
	{
		private const string _formValueContentType = "text/plain";
		private const string _fileContentType = "application/octet-stream";

		public async Task<IEnumerable<MultipartFormContentDescriptor>> FormToDescriptorsAsync(IFormCollection form)
		{
			IEnumerable<MultipartFormContentDescriptor> formValues = await ExtractFormValuesAsync(form);
			IEnumerable<MultipartFormContentDescriptor> formFiles = ExtractFormFiles(form.Files);

			return formValues.Concat(formFiles);
		}

		private async Task<IEnumerable<MultipartFormContentDescriptor>> ExtractFormValuesAsync(IFormCollection form)
		{
			List<MultipartFormContentDescriptor> content =
				new List<MultipartFormContentDescriptor>();

			foreach (var formValue in form)
			{
				Stream body = new MemoryStream();
				StreamWriter writer = new StreamWriter(body);

				// TODO pass all values instead of First()
				await writer.WriteAsync(formValue.Value.First().ToString());
				await writer.FlushAsync();

				body.Position = 0;

				MultipartFormContentDescriptor multipartContent =
					new MultipartFormContentDescriptor(_formValueContentType, formValue.Key, body);

				content.Add(multipartContent);
			}

			return content;
		}

		private IEnumerable<MultipartFormContentDescriptor> ExtractFormFiles(IFormFileCollection files)
		{
			List<MultipartFormContentDescriptor> content =
				new List<MultipartFormContentDescriptor>();

			foreach (var file in files)
			{
				Stream body = file.OpenReadStream();

				MultipartFormContentDescriptor multipartContent =
					new MultipartFormContentDescriptor(
						_fileContentType, file.Name, body, file.FileName);

				content.Add(multipartContent);
			}

			return content;
		}
	}
}
