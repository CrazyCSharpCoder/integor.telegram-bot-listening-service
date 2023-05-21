using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using IntegorTelegramBotListeningModel;
using IntegorTelegramBotListeningShared.Bots;

namespace IntegorTelegramBotListeningServices.Bots
{
	using EntityFramework;
	using EntityFramework.Internal;

	public class EntityFrameworkBotWebhookManagementService : IBotWebhookManagementService
	{
		private IntegorTelegramBotListeningDataContext _context;

		public EntityFrameworkBotWebhookManagementService(
			IntegorTelegramBotListeningDataContext context)
        {
			_context = context;
        }

		public Task<TelegramBotWebhookInfo?> GetByBotTokenAsync(string botToken)
		{
			return _context.Webhooks.FirstOrDefaultAsync(
				webhook => webhook.BotToken == botToken);
		}

		public async Task<TelegramBotWebhookInfo> SetAsync(TelegramBotWebhookInfo webhook)
		{
			TelegramBotWebhookInfo? oldWebhook =
				await _context.Webhooks.GetByIdAsync(webhook.Id);

			if (oldWebhook != null)
				_context.Webhooks.Remove(oldWebhook);

			await _context.Webhooks.AddAsync(webhook);
			await _context.SaveChangesAsync();

			return webhook;
		}

		public async Task<TelegramBotWebhookInfo?> DeleteAsync(string botToken)
		{
			TelegramBotWebhookInfo? oldWebhook =
				await _context.Webhooks.GetReruiredByBotTokenAsync(botToken);

			if (oldWebhook == null)
				return null;

			_context.Webhooks.Remove(oldWebhook);
			await _context.SaveChangesAsync();

			return oldWebhook;
		}

		public async Task<TelegramBotWebhookInfo> UpdateTokenCacheAsync(int webhookId, string newToken)
		{
			TelegramBotWebhookInfo webhook =
				await _context.Webhooks.GetReruiredByIdAsync(webhookId);

			webhook.BotToken = newToken;

			_context.Webhooks.Update(webhook);
			await _context.SaveChangesAsync();

			return webhook;
		}
	}
}
