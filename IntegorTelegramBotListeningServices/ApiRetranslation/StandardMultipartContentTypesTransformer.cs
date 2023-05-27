using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using IntegorTelegramBotListeningShared.ApiRetranslation;
using IntegorTelegramBotListeningShared.ApiRetranslation.ApiContent;

namespace IntegorTelegramBotListeningServices.ApiRetranslation
{
	using MultipartNamesEncoding;

	public class StandardMultipartContentTypesTransformer : IMultipartContentTypesTransformer
	{
		private const string _formValueContentType = "text/plain";
		private const string _fileContentType = "application/octet-stream";

		private MultipartNamesEncodersProvider _encodersProvider;

		public StandardMultipartContentTypesTransformer(
			MultipartNamesEncodersProvider encodersProvider)
        {
			_encodersProvider = encodersProvider;
        }

        public async Task<IEnumerable<MultipartFormContentDescriptor>> FormToDescriptorsAsync(IFormCollection form)
		{
			IEnumerable<MultipartFormContentDescriptor> formValues = await ExtractFormValuesAsync(form);
			IEnumerable<MultipartFormContentDescriptor> formFiles = ExtractFormFiles(form.Files);

			formFiles = formFiles.Select(formFile =>
			{
				try { return MultipartFileToEncoded(formFile); }
				catch { return formFile; }
			});

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

				string value = formValue.Value.First().ToString();

				if (formValue.Key.ToLower() == "media")
				{
					string? parsedMedia = null;

					try { parsedMedia = TryEncodeMediaFormValue(value); }
					catch { /*Ignore*/ }

					if (parsedMedia != null)
						value = parsedMedia;
				}

				// TODO pass all values instead of First()
				await writer.WriteAsync(value);
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

		private string? TryEncodeMediaFormValue(string value)
		{
			JArray? media = JsonConvert.DeserializeObject<JArray>(value);

			if (media == null)
				return null;

			foreach (JObject mediaEntry in media)
				TryEncodeMedia(mediaEntry);

			return JsonConvert.SerializeObject(media);
		}

		private void TryEncodeMedia(JObject mediaEntry)
		{
			const string fileDescriptionPropertyName = "media";
			const string attachPrefix = "attach://";

			JProperty? fileDescriptionProperty = mediaEntry
				.Property(fileDescriptionPropertyName);

			if (fileDescriptionProperty == null)
				return;

			string fileDescription = (string)fileDescriptionProperty!;

			if (!fileDescription.StartsWith(attachPrefix))
				return;

			// Убираем attachPrefix из начала строки, чтобы получить сам filaname
			string fileName = fileDescription.Remove(0, attachPrefix.Length);

			foreach (IMultipartNameEncoder nameEncoder in _encodersProvider)
			{
				if (!nameEncoder.EncodingRequired(fileName))
					continue;

				fileName = nameEncoder.Encode(fileName);
				break;
			}

			fileDescriptionProperty.Value = attachPrefix + fileName;
		}

		private MultipartFormContentDescriptor MultipartFileToEncoded
			(MultipartFormContentDescriptor file)
		{
			string? fileName = null;
			string? name = null;

			foreach (IMultipartNameEncoder nameEncoder in _encodersProvider)
			{
				// If source filename is not null and if destination file name is not assigned ...
				if (file.FileName != null && fileName == null &&
					nameEncoder.EncodingRequired(file.FileName))

					fileName = nameEncoder.Encode(file.FileName);

				// If destination name is not assigned ...
				if (name == null && nameEncoder.EncodingRequired(file.Name))
					name = nameEncoder.Encode(file.Name);
			}

			return new MultipartFormContentDescriptor(
				file.ContentType, body: file.Body,

				name: name ?? file.Name,
				fileName: fileName ?? file.FileName);
		}
	}
}
