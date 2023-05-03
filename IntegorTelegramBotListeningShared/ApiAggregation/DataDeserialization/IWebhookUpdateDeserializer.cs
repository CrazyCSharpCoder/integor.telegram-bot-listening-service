using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntegorTelegramBotListeningDto;

namespace IntegorTelegramBotListeningShared.ApiAggregation.DataDeserialization
{
	public interface IWebhookUpdateDeserializer : ITelegramDataDeserializer<TelegramUpdateDto>
	{
	}
}
