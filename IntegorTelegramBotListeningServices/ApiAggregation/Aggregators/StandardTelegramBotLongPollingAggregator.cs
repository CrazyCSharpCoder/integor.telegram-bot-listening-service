using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using AutoMapper;

using IntegorTelegramBotListeningDto;

using IntegorTelegramBotListeningShared.EventsAggregation;
using IntegorTelegramBotListeningShared.ApiAggregation.Aggregators;

namespace IntegorTelegramBotListeningServices.ApiAggregation.Aggregators
{
	using Internal.Aggregators;

    public class StandardTelegramBotLongPollingAggregator : ITelegramBotLongPollingAggregator
    {
		private IMessagesAggregationService _messagesAggregator;
		private EventsAggregationHelper _aggregationHelper;

        public StandardTelegramBotLongPollingAggregator(
            IChatsAggregationService chatsAggregator,
            IUsersAggregationService usersAggregator,
            IMessagesAggregationService messagesAggregator,

            IMapper mapper)
        {
			_messagesAggregator = messagesAggregator;
			_aggregationHelper = new EventsAggregationHelper(
				chatsAggregator, usersAggregator, messagesAggregator, mapper);
		}

        public async Task AggregateAsync(IEnumerable<TelegramUpdateDto> updates, int botId)
        {
			IEnumerable<TelegramMessageInfoDto> messages = updates
				.Select(update => update.Message)
				.Where(message => message != null)!;

			IEnumerable<Task> aggregateTasks = messages.Select(
				message => AggregateIfNewAsync(message, botId));

			await Task.WhenAll(aggregateTasks);
        }

		private async Task AggregateIfNewAsync(TelegramMessageInfoDto message, int botId)
		{
			if (!await MessageExistsAsync(message))
				await _aggregationHelper.AggregateMessageAsync(message, botId);
		}

		private async Task<bool> MessageExistsAsync(TelegramMessageInfoDto message)
			=> await _messagesAggregator.GetAsync(message!.Chat.Id, message.MessageId) != null;

	}
}
