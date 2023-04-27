using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningModel
{
	public class TelegramMessage
	{
		public long MessageId { get; set; }

		/// <summary>
		/// Time when the massage was sent
		/// </summary>
		public DateTime Date { get; set; }

		public string? Text { get; set; } = null!;

		/// <summary>
		/// User id the message comes from
		/// </summary>
		public long? FromId { get; set; }

		/// <summary>
		/// Id of bot from chat with which the message is aggregated
		/// </summary>
		public int RelatedBotId { get; set; }

		/// <summary>
		/// For replies, if of the original message
		/// </summary>
		public long? ReplyToMessageId { get; set; }
		public long? ReplyToMessageChatId { get; set; }

		public long ChatId { get; set; }
	}
}