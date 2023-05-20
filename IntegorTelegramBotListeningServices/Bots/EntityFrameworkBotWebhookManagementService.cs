using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public Task<TelegramBotWebhookInfo?> GetAsync(int webhookId)
		{
			return _context.Webhooks.GetByIdAsync(webhookId);
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
	}
}
