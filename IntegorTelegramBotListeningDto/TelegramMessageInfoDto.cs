using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningDto
{
	public class TelegramMessageInfoDto
	{
		public long MessageId { get; set; }

		public string? Text { get; set; } = null!;
		public DateTime Date { get; set; }

		public TelegramChatInfoDto Chat { get; set; } = null!;

		public TelegramUserInfoDto? From { get; set; }
		public TelegramMessageInfoDto? ReplyToMessage { get; set; }
	}
}
