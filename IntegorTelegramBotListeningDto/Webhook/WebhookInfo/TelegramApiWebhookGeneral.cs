using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningDto.Webhook.WebhookInfo
{
	public abstract class TelegramApiWebhookGeneral
	{
		public bool HasCustomCertificate { get; set; }

		public int PendingUpdateCount { get; set; }

		public int? LastErrorDate { get; set; }
		public string? LastErrorMessage { get; set; }
		public int? LastSynchronizationErrorDate { get; set; }

		public int? MaxConnections { get; set; }

		public string[]? AllowedUpdates { get; set; }
	}
}
