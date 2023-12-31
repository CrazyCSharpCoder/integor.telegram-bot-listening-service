﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net.Http;

namespace IntegorTelegramBotListeningShared.ApiRetranslation.ApiContent
{
    public interface IBotApiHttpContentFactory
    {
        HttpContent CreateDefaultContent(Stream body, string? mediaType);
        HttpContent CreateMultipartFormContent(IEnumerable<MultipartFormContentDescriptor> contentParts, string boundary);
		HttpContent CreateJsonContent<TBody>(TBody body);
	}
}
