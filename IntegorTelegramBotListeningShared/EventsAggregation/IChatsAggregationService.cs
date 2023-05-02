using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntegorTelegramBotListeningDto;

namespace IntegorTelegramBotListeningShared.EventsAggregation
{
	public interface IChatsAggregationService
	{
		Task<TelegramChatInfoDto> AddAsync(TelegramChatInfoDto chat);
		Task<TelegramChatInfoDto?> GetAsync(long id);
	}
}
