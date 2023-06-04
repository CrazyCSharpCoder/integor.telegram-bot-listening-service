using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

using IntegorLogicShared.Database.Attributes;

namespace IntegorTelegramBotListeningModel
{
	public class TelegramBotWebhookInfo
	{
		[Key]
		public long Id { get; set; }
		public string Url { get; set; } = null!;

		public string BotToken { get; set; } = null!;

		[CreatedTime]
		public DateTime CreatedDate { get; set; }
	}
}
