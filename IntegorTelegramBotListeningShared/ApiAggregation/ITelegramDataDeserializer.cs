using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningShared.ApiAggregation
{
	public interface ITelegramDataDeserializer<T>
	{
		T? Deserialize(JsonElement body);
	}
}
