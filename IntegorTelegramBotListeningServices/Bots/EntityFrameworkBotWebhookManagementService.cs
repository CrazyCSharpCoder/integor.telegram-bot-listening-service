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

		public Task<TelegramBotWebhookInfo?> GetByIdAsync(int webhookId)
		{
			return _context.Webhooks.GetByIdAsync(webhookId);
		}

		public Task<TelegramBotWebhookInfo?> GetByTokenCacheAsync(string botToken)
		{
			return _context.Webhooks.FirstOrDefaultAsync(
				webhook => webhook.BotTokenCache == botToken);
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

		public async Task<TelegramBotWebhookInfo?> DeleteAsync(int webhookId)
		{
			TelegramBotWebhookInfo? oldWebhook =
				await _context.Webhooks.GetReruiredByIdAsync(webhookId);

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

			webhook.BotTokenCache = newToken;

			_context.Webhooks.Update(webhook);
			await _context.SaveChangesAsync();

			return webhook;
		}
	}
}
