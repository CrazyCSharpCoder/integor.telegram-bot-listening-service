using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using IntegorTelegramBotListeningDto;
using IntegorTelegramBotListeningShared;

namespace IntegorTelegramBotListeningServices
{
	using EntityFramework;
	using EntityFramework.Model;

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
			EfTelegramBot addedBotModel = _mapper.Map<TelegramBotInfoDto, EfTelegramBot>(bot);

			await _db.Bots.AddAsync(addedBotModel);

			return _mapper.Map<EfTelegramBot, TelegramBotInfoDto>(addedBotModel);
		}

		public async Task<TelegramBotInfoDto?> GetByIdAsync(int id)
		{
			EfTelegramBot? bot = await _db.Bots.FirstOrDefaultAsync(bot => bot.Id == id);

			if (bot == null)
				return null;

			return _mapper.Map<EfTelegramBot, TelegramBotInfoDto>(bot);
		}

		public async Task<TelegramBotInfoDto?> GetByTokenAsync(string botToken)
		{
			EfTelegramBot? bot = await _db.Bots.FirstOrDefaultAsync(bot => bot.Token == botToken);

			if (bot == null)
				return null;

			return _mapper.Map<EfTelegramBot, TelegramBotInfoDto>(bot);
		}

		public async Task<TelegramBotInfoDto> UpdateAsync(TelegramBotInfoDto bot)
		{
			EfTelegramBot updatedBotModel = _mapper.Map<TelegramBotInfoDto, EfTelegramBot>(bot);

			_db.Bots.Update(updatedBotModel);

			return _mapper.Map<EfTelegramBot, TelegramBotInfoDto>(updatedBotModel);
		}
	}
}
