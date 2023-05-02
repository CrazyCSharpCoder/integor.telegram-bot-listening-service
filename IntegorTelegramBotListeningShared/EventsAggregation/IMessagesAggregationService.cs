using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntegorTelegramBotListeningModel;
using IntegorTelegramBotListeningDto;

namespace IntegorTelegramBotListeningShared.EventsAggregation
{
	public interface IMessagesAggregationService
	{
		Task<TelegramMessageInfoDto> AddAsync(TelegramMessage message);

		// TODO add method for fixing if a message is edited
		// and saving changes in the message history

		Task<TelegramMessageInfoDto?> GetAsync(long chatId, long messageId);

		Task<int> GetBotMessagesCountAsync(int botId);
		Task<IEnumerable<TelegramMessageInfoDto>> GetBotMessagesAsync(int botId, int startIndex, int count);
	}
}
