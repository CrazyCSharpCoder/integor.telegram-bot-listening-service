using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.Json.Converters;
using IntegorTelegramBotListeningShared.Json.NamingPolicies;

namespace IntegorTelegramBotListeningServices
{
	public class StandardJsonSerializerOptionsProvider : IJsonSerializerOptionsProvider
	{
		public JsonSerializerOptions GetJsonSerializerOptions()
		{
			JsonSerializerOptions options = new JsonSerializerOptions();
			AddTelegramJsonOptions(options);

			return options;
		}

		public void AssignJsonSerizalizerOptions(JsonSerializerOptions target)
		{
			ClearJsonOptions(target);
			AddTelegramJsonOptions(target);
		}

		private void ClearJsonOptions(JsonSerializerOptions options)
		{
			options.Converters.Clear();
		}

		private void AddTelegramJsonOptions(JsonSerializerOptions options)
		{
			options.PropertyNameCaseInsensitive = true;

			options.DictionaryKeyPolicy = new SnakeCaseJsonNamingPolicy();
			options.PropertyNamingPolicy = new SnakeCaseJsonNamingPolicy();

			options.Converters.Add(new UnixDateTimeJsonConverter());
		}
	}
}
