using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningServices.Internal.DataDeserialization
{
	internal static class TelegramResponseDeserializationHelpers
	{
		public static JsonElement? GetResult(JsonElement body)
		{
			if (!body.GetProperty("ok").GetBoolean())
				return null;

			return body.GetProperty("result");
		}
	}
}
