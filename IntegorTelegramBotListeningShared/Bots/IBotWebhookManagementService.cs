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
		Task<TelegramBotWebhookInfo?> GetAsync(int botId);
		Task<TelegramBotWebhookInfo> SetAsync(TelegramBotWebhookInfo webhook);
		Task<TelegramBotWebhookInfo> DeleteAsync(int botId);
	}
}
