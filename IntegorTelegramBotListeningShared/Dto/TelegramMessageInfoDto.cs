using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningShared.Dto
{
	public class TelegramMessageInfoDto
	{
		public long MessageId { get; set; }

		public string? Text { get; set; } = null!;
		public DateTime Date { get; set; }

		public TelegramUserInfoDto? From { get; set; }
		public TelegramMessageInfoDto? ReplyToMessage { get; set; }
	}
}
