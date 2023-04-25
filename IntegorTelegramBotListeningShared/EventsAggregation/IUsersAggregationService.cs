using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningShared.EventsAggregation
{
	using Dto;

	public interface IUsersAggregationService
	{
		Task<TelegramUserInfoDto?> GetAsync(long id);
		Task<TelegramUserInfoDto> AddOrUpdateAsync(TelegramUserInfoDto user);
	}
}
