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
			EfTelegramUser? user = await _db.Users
				.FirstOrDefaultAsync(user => user.Id == id);

			if (user == null)
				return null;

			return _mapper.Map<EfTelegramUser, TelegramUserInfoDto>(user);
		}
		
		public async Task<TelegramUserInfoDto> AddOrUpdateAsync(TelegramUserInfoDto user)
		{
			EfTelegramUser? aggregatedUser = await _db.Users
				.FirstOrDefaultAsync(dbUser => dbUser.Id == user.Id);

			EfTelegramUser editedUser =
				_mapper.Map<TelegramUserInfoDto, EfTelegramUser>(user);

			if (aggregatedUser == null)
				await _db.Users.AddAsync(editedUser);
			else
				_db.Users.Update(editedUser);

			return _mapper.Map<EfTelegramUser, TelegramUserInfoDto>(editedUser);
		}
	}
}
