using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntegorTelegramBotListeningDto;

namespace IntegorTelegramBotListeningShared.EventsAggregation
{
	public interface IUsersAggregationService
	{
		Task<TelegramUserInfoDto?> GetAsync(long id);
		Task<TelegramUserInfoDto> AddOrUpdateAsync(TelegramUserInfoDto user);
	}
}
