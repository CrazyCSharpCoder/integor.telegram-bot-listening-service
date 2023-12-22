using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using IntegorTelegramBotListeningModel;

namespace IntegorTelegramBotListeningServices.EntityFramework.Internal
{
	internal static class TelegramBotWebhookExtensions
	{
		public static Task<TelegramBotWebhookInfo?> GetByIdAsync(
			this IQueryable<TelegramBotWebhookInfo> webhooks, long webhookId)
			=> webhooks.FirstOrDefaultAsync(webhook => webhook.Id == webhookId);

		public static Task<TelegramBotWebhookInfo> GetReruiredByIdAsync(
			this IQueryable<TelegramBotWebhookInfo> webhooks, long webhookId)
			=> webhooks.FirstAsync(webhook => webhook.Id == webhookId);

		public static Task<TelegramBotWebhookInfo?> GetByBotTokenAsync(
			this IQueryable<TelegramBotWebhookInfo> webhooks, string botToken)
			=> webhooks.FirstOrDefaultAsync(webhook => webhook.BotToken == botToken);

		public static Task<TelegramBotWebhookInfo> GetReruiredByBotTokenAsync(
			this IQueryable<TelegramBotWebhookInfo> webhooks, string botToken)
			=> webhooks.FirstAsync(webhook => webhook.BotToken == botToken);
	}
}
