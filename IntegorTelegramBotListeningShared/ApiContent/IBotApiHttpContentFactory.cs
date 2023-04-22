using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using System.IO;
using System.Net.Http;

namespace IntegorTelegramBotListeningShared.ApiContent
{
    public interface IBotApiHttpContentFactory
	{
		HttpContent CreateDefaultContent(Stream body, string? mediaType);
		HttpContent CreateMultipartFormContent(IEnumerable<MultipartFormContent> contentParts, string boundary);
    }
}
