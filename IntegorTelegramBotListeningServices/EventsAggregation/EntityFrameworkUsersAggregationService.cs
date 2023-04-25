using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using IntegorTelegramBotListeningModel;

using IntegorTelegramBotListeningShared.Dto;
using IntegorTelegramBotListeningShared.EventsAggregation;

namespace IntegorTelegramBotListeningServices.EventsAggregation
{
	using DataContext;

	public class EntityFrameworkUsersAggregationService : IUsersAggregationService
	{
		private IntegorTelegramBotListeningDataContext _db;
		private IMapper _mapper;

		public EntityFrameworkUsersAggregationService(
			IntegorTelegramBotListeningDataContext db, IMapper mapper)
        {
			_db = db;
			_mapper = mapper;
        }

		public async Task<TelegramUserInfoDto?> GetAsync(long id)
		{
			TelegramUser? user = await _db.Users
				.FirstOrDefaultAsync(user => user.Id == id);

			if (user == null)
				return null;

			return _mapper.Map<TelegramUser, TelegramUserInfoDto>(user);
		}
		
		public async Task<TelegramUserInfoDto> AddOrUpdateAsync(TelegramUserInfoDto user)
		{
			TelegramUser? aggregatedUser = await _db.Users
				.FirstOrDefaultAsync(user => user.Id == user.Id);

			TelegramUser editedUser =
				_mapper.Map<TelegramUserInfoDto, TelegramUser>(user);

			if (aggregatedUser == null)
				await _db.Users.AddAsync(editedUser);
			else
				_db.Users.Update(editedUser);

			await _db.SaveChangesAsync();

			return _mapper.Map<TelegramUser, TelegramUserInfoDto>(editedUser);
		}
	}
}
