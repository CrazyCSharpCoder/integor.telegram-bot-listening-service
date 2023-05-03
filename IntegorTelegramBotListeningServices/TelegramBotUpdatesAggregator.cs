using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

using AutoMapper;

using IntegorTelegramBotListeningModel;
using IntegorTelegramBotListeningDto;

using IntegorTelegramBotListeningShared;

using IntegorTelegramBotListeningShared.Json.Converters;
using IntegorTelegramBotListeningShared.Json.NamingPolicies;

using IntegorTelegramBotListeningShared.Bots;
using IntegorTelegramBotListeningShared.EventsAggregation;

namespace IntegorTelegramBotListeningServices
{
    using JsonDeserialization.Internal;

    public class TelegramBotUpdatesAggregator : ITelegramBotEventsAggregator
	{
		private const string _getUpdatesMethod = "getUpdates";

		private string _botToken = null!;

		private IJsonSerializerOptionsProvider _jsonOptionsProvider;

		private IBotsManagementService _botsManagement;

		private IChatsAggregationService _chatsAggregator;
		private IUsersAggregationService _usersAggregator;
		private IMessagesAggregationService _messagesAggregator;

		private IMapper _mapper;

		public TelegramBotUpdatesAggregator(
			IJsonSerializerOptionsProvider jsonOptionsProvider,

			IBotsManagementService botsManagement,

			IChatsAggregationService chatsAggregator,
			IUsersAggregationService usersAggregator,
			IMessagesAggregationService messagesAggregator,
			
			IMapper mapper)
        {
			_jsonOptionsProvider = jsonOptionsProvider;

			_botsManagement = botsManagement;

			_chatsAggregator = chatsAggregator;
			_usersAggregator = usersAggregator;
			_messagesAggregator = messagesAggregator;

			_mapper = mapper;
        }

        public async Task<bool> AllowAggregationAsync(string botToken, string apiMethod, string? mediaType)
		{
			if (apiMethod.ToLower() != _getUpdatesMethod.ToLower())
				return false;

			if (mediaType != "application/json")
				return false;

			if (await _botsManagement.GetByTokenAsync(botToken) == null)
				return false;

			_botToken = botToken;

			return true;
		}

		public async Task AggregateAsync(Stream body)
		{
			// Если бот не зарегистрирован, нельзя агрегировать его сообщения
			TelegramBotInfoDto? bot = await _botsManagement.GetByTokenAsync(_botToken);

			if (bot == null)
				return;

			// Десериализация json'а с обновлениями
			JsonSerializerOptions serializerOptions = _jsonOptionsProvider.CreateJsonSerializerOptions();
			JsonElement jsonBody = await JsonSerializer.DeserializeAsync<JsonElement>(body, serializerOptions);

			if (!jsonBody.GetProperty("ok").GetBoolean())
				return;

			JsonElement.ArrayEnumerator updates = jsonBody.GetProperty("result").EnumerateArray();

			IEnumerable<JsonElement.ObjectEnumerator> updateProps = updates
				.Select(update => update.EnumerateObject());

			IEnumerable<TelegramMessageInfoDto> messages = updateProps
				.Select(updateProps => JsonElementHelpers.TryGetPropertyCaseInsensitive(updateProps, "message"))
				.Where(messagePropCheck => messagePropCheck != null)

				.Select(messageProp => JsonElementHelpers
					.TryDeserializeJson<TelegramMessageInfoDto>(
						((JsonProperty)messageProp!).Value, serializerOptions))
				.Where(messageCheck => messageCheck != null)!;

			// Агрегирование сообщений
			foreach (TelegramMessageInfoDto message in messages)
				await AggregateMessageAsync(bot.Id, message);
		}

		private async Task AggregateMessageAsync(int botId, TelegramMessageInfoDto message)
		{
			TelegramMessageInfoDto? replyToMessage = message.ReplyToMessage;

			if (replyToMessage != null && await _messagesAggregator.GetAsync(replyToMessage.Chat.Id, replyToMessage.MessageId) == null)
				await AggregateMessageAsync(botId, replyToMessage);

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
