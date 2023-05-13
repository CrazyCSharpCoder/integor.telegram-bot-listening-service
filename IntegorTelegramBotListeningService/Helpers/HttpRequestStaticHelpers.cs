using System.Linq;
using System.Net.Http;

using Microsoft.AspNetCore.Http;

using IntegorTelegramBotListeningShared.ApiRetranslation.ApiContent;

namespace IntegorTelegramBotListeningService.Helpers
{
	public static class HttpRequestStaticHelpers
	{
		public static string GetMediaType(string contentType)
			=> contentType.Split(";", 2).First().Trim();
	}
}
