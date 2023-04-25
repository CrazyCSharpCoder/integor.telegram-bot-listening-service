using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegorTelegramBotListeningModel
{
	public class TelegramMessage
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
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
		public virtual TelegramUser? From { get; set; }

		/// <summary>
		/// Id of bot from chat with which the message is aggregated
		/// </summary>
		public int RelatedBotId { get; set; }
		public virtual TelegramBot? RelatedBot { get; set; }

		/// <summary>
		/// For replies, if of the original message
		/// </summary>
		public long? ReplyToMessageId { get; set; }
		public TelegramMessage? ReplyToMessage { get; set; }
	}
}