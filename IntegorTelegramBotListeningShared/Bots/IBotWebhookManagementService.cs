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
		Task<TelegramBotWebhookInfo?> GetByIdAsync(int webhookId);
		Task<TelegramBotWebhookInfo?> GetByTokenCacheAsync(string botToken);

		Task<TelegramBotWebhookInfo> SetAsync(TelegramBotWebhookInfo webhook);
		Task<TelegramBotWebhookInfo?> DeleteAsync(int webhookId);

		Task<TelegramBotWebhookInfo> UpdateTokenCacheAsync(int webhookId, string newToken);
	}
}
