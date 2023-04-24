using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Extensions.Options;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.Configuration;

namespace IntegorTelegramBotListeningServices
{
	public class StandardTelegramBotApiUriBuilder : ITelegramBotApiUriBuilder
	{
		private TelegramBotApiConfiguration _telegramBotApiConfiguration;

		public StandardTelegramBotApiUriBuilder(IOptions<TelegramBotApiConfiguration> telegramBotApiOptions)
        {
			_telegramBotApiConfiguration = telegramBotApiOptions.Value;
        }

        public string CreateUri(string botToken, string apiMethod, Dictionary<string, string>? queryParameters)
		{
			// Crating global uri for the bot with the given token
			string uri = Path.Combine(_telegramBotApiConfiguration.Domain, $"bot{botToken}");
			// Appending api method
			uri = Path.Combine(uri, apiMethod);

			// Appending query parameters
			if (queryParameters != null && queryParameters.Count() != 0)
				uri += $"?{CreateQueryString(queryParameters)}";

			return uri;
		}

		private string CreateQueryString(Dictionary<string, string> queryParameters)
			=> string.Join('&', queryParameters.Select(paramToValue => $"{paramToValue.Key}={paramToValue.Value}"));
	}
}
