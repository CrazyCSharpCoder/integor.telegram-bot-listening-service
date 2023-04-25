using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningShared.EventsAggregation
{
	using Dto;
	using Dto.Input;

	public interface IMessagesAggregationService
	{
		Task<TelegramMessageInfoDto> AddAsync(InputMessageDto message);

		// TODO add method for fixing if a message is edited
		// and saving changes in the message history

		Task<int> GetMessagesCountAsync(int botId);
		Task<IEnumerable<TelegramMessageInfoDto>> GetMessagesAsync(int botId, int startIndex, int count);
	}
}
