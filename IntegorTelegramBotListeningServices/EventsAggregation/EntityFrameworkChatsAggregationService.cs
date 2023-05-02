using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using IntegorTelegramBotListeningDto;
using IntegorTelegramBotListeningShared.EventsAggregation;

namespace IntegorTelegramBotListeningServices.EventsAggregation
{
	using EntityFramework;
	using EntityFramework.Model;

	public class EntityFrameworkChatsAggregationService : IChatsAggregationService
	{
		private IntegorTelegramBotListeningDataContext _db;
		private IMapper _mapper;

		public EntityFrameworkChatsAggregationService(
			IntegorTelegramBotListeningDataContext db,
			IMapper mapper)
        {
			_db = db;
			_mapper = mapper;
        }

        public async Task<TelegramChatInfoDto> AddAsync(TelegramChatInfoDto chat)
		{
			EfTelegramChat addedChatModel = _mapper.Map<TelegramChatInfoDto, EfTelegramChat>(chat);
			await _db.AddAsync(addedChatModel);

			return _mapper.Map<EfTelegramChat, TelegramChatInfoDto>(addedChatModel);
		}

		public async Task<TelegramChatInfoDto?> GetAsync(long id)
		{
			EfTelegramChat? chat = await _db.Chats.FirstOrDefaultAsync(chat => chat.Id == id);

			if (chat == null)
				return null;

			return _mapper.Map<EfTelegramChat, TelegramChatInfoDto>(chat);
		}
	}
}
