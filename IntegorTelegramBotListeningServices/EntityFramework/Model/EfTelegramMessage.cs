using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntegorTelegramBotListeningModel;

namespace IntegorTelegramBotListeningServices.EntityFramework.Model
{
	public class EfTelegramMessage : TelegramMessage
	{
		public virtual EfTelegramUser? From { get; set; } = null!;
		public virtual EfTelegramChat Chat { get; set; } = null!;

		public virtual EfTelegramBot? RelatedBot { get; set; } = null!;

		public virtual EfTelegramMessage? ReplyToMessage { get; set; } = null!;
	}
}