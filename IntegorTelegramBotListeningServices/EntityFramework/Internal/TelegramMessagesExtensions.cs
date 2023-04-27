using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningServices.EntityFramework.Internal
{
	using Model;

	internal static class TelegramMessagesExtensions
	{
		public static IQueryable<EfTelegramMessage> GetMessagesOfBot(
			this IQueryable<EfTelegramMessage> messages, int botId)
			=> messages.Where(msg => msg.RelatedBotId == botId);
	}
}
