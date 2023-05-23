using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntegorTelegramBotListeningModel;

namespace IntegorTelegramBotListeningShared.Bots
{
	public interface IBotWebhookManagementService
	{
		Task<TelegramBotWebhookInfo?> GetByBotTokenAsync(string botToken);

		Task<TelegramBotWebhookInfo> SetAsync(TelegramBotWebhookInfo webhook);
		Task<TelegramBotWebhookInfo?> DeleteAsync(string botToken);

		Task<TelegramBotWebhookInfo> UpdateTokenCacheAsync(int webhookId, string newToken);
	}
}
