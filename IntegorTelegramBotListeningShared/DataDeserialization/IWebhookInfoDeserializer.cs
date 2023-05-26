using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntegorTelegramBotListeningDto.Webhook.WebhookInfo;

namespace IntegorTelegramBotListeningShared.DataDeserialization
{
	public interface IWebhookInfoDeserializer
		: ITelegramDataDeserializer<TelegramApiWebhookInfoDto>
	{
	}
}
