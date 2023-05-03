using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using IntegorTelegramBotListeningDto;

using IntegorTelegramBotListeningShared.EventsAggregation;
using IntegorTelegramBotListeningShared.ApiAggregation.Aggregators;

namespace IntegorTelegramBotListeningServices.ApiAggregation.Aggregators
{
	using Internal.Aggregators;

	public class StandardTelegramBotWebhookAggregator : ITelegramBotWebhookAggregator
	{
		private EventsAggregationHelper _aggregationHelper;

		public StandardTelegramBotWebhookAggregator(
			IChatsAggregationService chatsAggregator,
			IUsersAggregationService usersAggregator,
			IMessagesAggregationService messagesAggregator,

			IMapper mapper)
        {
			_aggregationHelper = new EventsAggregationHelper(
				chatsAggregator, usersAggregator, messagesAggregator, mapper);
        }

        public async Task AggregateAsync(TelegramUpdateDto update, int botId)
		{
			if (update.Message == null)
				return;

			await _aggregationHelper.AggregateMessageAsync(update.Message, botId);
		}
	}
}
