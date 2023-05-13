using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace IntegorTelegramBotListeningModel
{
	public class TelegramBotWebhookInfo
	{
		[Key]
		public int Id { get; set; }
		public string Url { get; set; } = null!;

		public int BotId { get; set; }
	}
}
