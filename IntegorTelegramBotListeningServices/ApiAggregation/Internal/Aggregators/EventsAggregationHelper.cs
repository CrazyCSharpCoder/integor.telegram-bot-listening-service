using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntegorTelegramBotListeningModel;
using IntegorTelegramBotListeningDto;

using IntegorTelegramBotListeningShared.EventsAggregation;

using AutoMapper;

namespace IntegorTelegramBotListeningServices.ApiAggregation.Internal.Aggregators
{
	internal class EventsAggregationHelper
	{
		private IChatsAggregationService _chatsAggregator;
		private IUsersAggregationService _usersAggregator;
		private IMessagesAggregationService _messagesAggregator;

		private IMapper _mapper;

		public EventsAggregationHelper(
			IChatsAggregationService chatsAggregator,
			IUsersAggregationService usersAggregator,
			IMessagesAggregationService messagesAggregator,

			IMapper mapper)
		{
			_chatsAggregator = chatsAggregator;
			_usersAggregator = usersAggregator;
			_messagesAggregator = messagesAggregator;

			_mapper = mapper;
		}

		public async Task AggregateMessageAsync(TelegramMessageInfoDto message, int botId)
		{
			TelegramMessageInfoDto? replyToMessage = message.ReplyToMessage;

			if (replyToMessage != null && await _messagesAggregator.GetAsync(replyToMessage.Chat.Id, replyToMessage.MessageId) == null)
				await AggregateMessageAsync(replyToMessage, botId);

			if (await _chatsAggregator.GetAsync(message.Chat.Id) == null)
				await _chatsAggregator.AddAsync(message.Chat);

			if (message.From != null)
				await _usersAggregator.AddOrUpdateAsync(message.From);

			TelegramMessage addedMessageModel =
				_mapper.Map<TelegramMessageInfoDto, TelegramMessage>(message);
			addedMessageModel.RelatedBotId = botId;

			await _messagesAggregator.AddAsync(addedMessageModel);
		}
	}
}
