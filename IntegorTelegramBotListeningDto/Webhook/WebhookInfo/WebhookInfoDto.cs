using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningDto.Webhook.WebhookInfo
{
	public class WebhookInfoDto
	{
		public bool IsSet { get; set; }

		public WebhookMetaDto? Meta { get; set; }
		public TelegramApiWebhookPublicInfoDto? TelegramWebhook { get; set; }
	}
}
