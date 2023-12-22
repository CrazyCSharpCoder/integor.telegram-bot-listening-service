using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningDto.Webhook
{
	public class TelegramSetWebhookDto
	{
		// TODO добавить остальной функционал

		public string Url { get; set; } = null!;

		public string[]? AllowedUpdates { get; set; }
		public bool? DropPendingUpdates { get; set; }

		public string? SecretToken { get; set; }

		public int? MaxConnections { get; set; }
	}
}
