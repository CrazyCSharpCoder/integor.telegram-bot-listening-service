using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntegorTelegramBotListeningModel;

namespace IntegorTelegramBotListeningServices.DataContext.Internal
{
	internal static class TelegramMessagesExtensions
	{
		public static IQueryable<TelegramMessage> GetMessagesOfBot(
			this IQueryable<TelegramMessage> messages, int botId)
			=> messages.Where(msg => msg.RelatedBotId == botId);
	}
}
