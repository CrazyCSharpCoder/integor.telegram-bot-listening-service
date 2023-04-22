using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace IntegorTelegramBotListeningShared.ApiContent
{
	public class MultipartFormContent : IDisposable, IAsyncDisposable
	{
		public string ContentType { get; }

		public string Name { get; }
		public string? FileName { get; }

		public Stream Body { get; }

        public MultipartFormContent(
			string contentType, string name, Stream body, string? fileName = null)
        {
            ContentType = contentType;

			Name = name;
			Body = body;

			FileName = fileName;
        }

		public void Dispose()
		{
			Body.Dispose();
		}

		public async ValueTask DisposeAsync()
		{
			await Body.DisposeAsync();
		}
	}
}
