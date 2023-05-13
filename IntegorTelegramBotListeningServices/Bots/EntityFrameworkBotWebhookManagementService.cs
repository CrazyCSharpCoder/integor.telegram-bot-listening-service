using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using IntegorTelegramBotListeningShared.Bots;

namespace IntegorTelegramBotListeningServices.Bots
{
	using EntityFramework;
	using EntityFramework.Model;
	using IntegorTelegramBotListeningModel;

	public class EntityFrameworkBotWebhookManagementService : IBotWebhookManagementService
	{
		private IntegorTelegramBotListeningDataContext _db;
		private IMapper _mapper;

		public EntityFrameworkBotWebhookManagementService(
			IntegorTelegramBotListeningDataContext db,
			IMapper mapper)
        {
			_db = db;
			_mapper = mapper;
        }

		public async Task<TelegramBotWebhookInfo?> GetAsync(int botId)
		{
			return await GetEfModelAsync(botId);
		}

		public async Task<TelegramBotWebhookInfo> SetAsync(TelegramBotWebhookInfo webhook)
		{
			EfTelegramBotWebhookInfo? oldWebhook = await GetEfModelAsync(webhook.BotId);

			if (oldWebhook != null)
				_db.Webhooks.Remove(oldWebhook);

			EfTelegramBotWebhookInfo addedWebhookModel =
				_mapper.Map<TelegramBotWebhookInfo, EfTelegramBotWebhookInfo>(webhook);

			await _db.Webhooks.AddAsync(addedWebhookModel);

			return addedWebhookModel;
		}

		public async Task<TelegramBotWebhookInfo> DeleteAsync(int botId)
		{
			EfTelegramBotWebhookInfo? oldWebhook = await GetEfModelAsync(botId);

			if (oldWebhook == null)
				throw new InvalidOperationException("Bot with specified id does not exist");

			_db.Webhooks.Remove(oldWebhook);
			return oldWebhook;
		}

		private async Task<EfTelegramBotWebhookInfo?> GetEfModelAsync(int botId)
		{
			return await _db.Webhooks.FirstOrDefaultAsync(webhook => webhook.BotId == botId);
		}
	}
}
