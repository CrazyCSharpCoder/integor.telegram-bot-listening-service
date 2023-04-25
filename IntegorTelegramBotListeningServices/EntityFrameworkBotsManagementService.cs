using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using IntegorTelegramBotListeningModel;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.Dto;

namespace IntegorTelegramBotListeningServices
{
	using DataContext;

	public class EntityFrameworkBotsManagementService : IBotsManagementService
	{
		private IntegorTelegramBotListeningDataContext _db;
		private IMapper _mapper;

		public EntityFrameworkBotsManagementService(
			IntegorTelegramBotListeningDataContext db,
			IMapper mapper)
        {
			_db = db;
			_mapper = mapper;
        }

        public async Task<TelegramBotInfoDto> AddAsync(TelegramBotInfoDto bot)
		{
			TelegramBot addedBotModel = _mapper.Map<TelegramBotInfoDto, TelegramBot>(bot);

			await _db.Bots.AddAsync(addedBotModel);
			await _db.SaveChangesAsync();

			return _mapper.Map<TelegramBot, TelegramBotInfoDto>(addedBotModel);
		}

		public async Task<TelegramBotInfoDto?> GetByIdAsync(int id)
		{
			TelegramBot? bot = await _db.Bots.FirstOrDefaultAsync(bot => bot.Id == id);

			if (bot == null)
				return null;

			return _mapper.Map<TelegramBot, TelegramBotInfoDto>(bot);
		}

		public async Task<TelegramBotInfoDto?> GetByTokenAsync(string botToken)
		{
			TelegramBot? bot = await _db.Bots.FirstOrDefaultAsync(bot => bot.Token == botToken);

			if (bot == null)
				return null;

			return _mapper.Map<TelegramBot, TelegramBotInfoDto>(bot);
		}

		public async Task<TelegramBotInfoDto> UpdateAsync(TelegramBotInfoDto bot)
		{
			TelegramBot updatedBotModel = _mapper.Map<TelegramBotInfoDto, TelegramBot>(bot);

			_db.Bots.Update(updatedBotModel);
			await _db.SaveChangesAsync();

			return _mapper.Map<TelegramBot, TelegramBotInfoDto>(updatedBotModel);
		}
	}
}
