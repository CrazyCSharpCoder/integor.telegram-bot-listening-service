using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningShared.Dto.Input
{
	public class InputMessageDto
	{
		public long MessageId { get; set; }
		public DateTime Date { get; set; }

		public string? Text { get; set; } = null!;

		public long? FromId { get; set; }
		public long? ReplyToMessageId { get; set; }

		public int RelatedBotId { get; set; }
	}
}
