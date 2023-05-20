using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegorTelegramBotListeningModel
{
	public class TelegramBotWebhookInfo
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }
		public string Url { get; set; } = null!;

		public string BotTokenCache { get; set; } = null!;
	}
}
