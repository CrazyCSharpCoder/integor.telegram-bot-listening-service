using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using IntegorTelegramBotListeningModel;
using IntegorTelegramBotListeningDto;

using IntegorTelegramBotListeningShared.EventsAggregation;

namespace IntegorTelegramBotListeningServices.EventsAggregation
{
	using EntityFramework;
	using EntityFramework.Model;
	using EntityFramework.Internal;

	public class EntityFrameworkMessagesAggregationService : IMessagesAggregationService
	{
		private IntegorTelegramBotListeningDataContext _db;
		private IMapper _mapper;

		public EntityFrameworkMessagesAggregationService(
			IntegorTelegramBotListeningDataContext db,
			IMapper mapper)
        {
			_db = db;
			_mapper = mapper;
        }

		public async Task<TelegramMessageInfoDto?> GetAsync(long chatId, long messageId)
		{
			EfTelegramMessage? message = await _db.Messages
				.Include(msg => msg.ReplyToMessage)
				.FirstOrDefaultAsync(msg => msg.ChatId == chatId && msg.MessageId == messageId);

			if (message == null)
				return null;

			return _mapper.Map<EfTelegramMessage, TelegramMessageInfoDto>(message);
		}

		public async Task<TelegramMessageInfoDto> AddAsync(TelegramMessage message)
		{
			EfTelegramMessage messageModel = _mapper.Map<TelegramMessage, EfTelegramMessage>(message);

			await _db.Messages.AddAsync(messageModel);

			return _mapper.Map<EfTelegramMessage, TelegramMessageInfoDto>(messageModel);
		}

		public async Task<IEnumerable<TelegramMessageInfoDto>> GetBotMessagesAsync(int botId, int startIndex, int count)
		{
			IEnumerable<EfTelegramMessage> messages = await _db.Messages
				.GetMessagesOfBot(botId)
				.Skip(startIndex).Take(count)

				.Include(msg => msg.From)
				.Include(msg => msg.Chat)
				.Include(msg => msg.ReplyToMessage)

				.OrderBy(msg => msg.Date)
				.ToArrayAsync();

			return messages.Select(msg => _mapper.Map<EfTelegramMessage, TelegramMessageInfoDto>(msg));
		}

		public async Task<int> GetBotMessagesCountAsync(int botId)
		{
			return await _db.Messages
				.GetMessagesOfBot(botId)
				.CountAsync();
		}
	}
}
