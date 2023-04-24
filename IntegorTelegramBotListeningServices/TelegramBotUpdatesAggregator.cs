using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

using IntegorTelegramBotListeningShared;

namespace IntegorTelegramBotListeningServices
{
	public class TelegramBotUpdatesAggregator : ITelegramBotEventsAggregator
	{
		private const string _getUpdatesMethod = "getUpdates";

		private string _botToken = null!;

		public bool AllowAggregation(string botToken, string apiMethod, string? mediaType)
		{
			if (apiMethod.ToLower() != _getUpdatesMethod.ToLower())
				return false;

			if (mediaType != "application/json")
				return false;

			_botToken = botToken;

			return true;
		}

		public async Task AggregateAsync(Stream body)
		{
			JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = true
			};
			JsonElement jsonRoot = await JsonSerializer.DeserializeAsync<JsonElement>(body, serializerOptions);

			if (!jsonRoot.TryGetProperty("ok", out JsonElement ok) || !ok.GetBoolean())
				return;

			if (!jsonRoot.TryGetProperty("result", out JsonElement result) || result.GetArrayLength() == 0)
				return;

			string updatesJson = JsonSerializer.Serialize(result, serializerOptions);

			// TODO add to database
			string logMessage =
				$"Bot {_botToken}\n:" +
				$"{updatesJson}\n";

			await Console.Out.WriteLineAsync(logMessage);
		}
	}
}
