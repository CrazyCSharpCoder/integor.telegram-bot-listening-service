using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningDto.Webhook.WebhookInfo
{
    public class TelegramApiWebhookInfoDto : TelegramApiWebhookGeneral
    {
		public string Url { get; set; } = null!;
	}
}
