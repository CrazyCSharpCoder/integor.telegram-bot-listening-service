using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace IntegorTelegramBotListeningShared.ApiRetranslation
{
	using ApiContent;

	public interface IMultipartContentTypesTransformer
    {
		Task<IEnumerable<MultipartFormContentDescriptor>> FormToDescriptorsAsync(IFormCollection form);
    }
}
