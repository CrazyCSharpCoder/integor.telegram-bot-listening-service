using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using IntegorTelegramBotListeningModel;

using IntegorTelegramBotListeningShared.Dto;
using IntegorTelegramBotListeningShared.Dto.Input;
using IntegorTelegramBotListeningShared.EventsAggregation;

namespace IntegorTelegramBotListeningServices.EventsAggregation
{
	using DataContext;
	using DataContext.Internal;

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

        public async Task<TelegramMessageInfoDto> AddAsync(InputMessageDto message)
		{
			TelegramMessage messageModel = _mapper.Map<InputMessageDto, TelegramMessage>(message);

			await _db.Messages.AddAsync(messageModel);
			await _db.SaveChangesAsync();

			return _mapper.Map<TelegramMessage, TelegramMessageInfoDto>(messageModel);
		}

		public async Task<IEnumerable<TelegramMessageInfoDto>> GetMessagesAsync(int botId, int startIndex, int count)
		{
			IEnumerable<TelegramMessage> messages = await _db.Messages
				.GetMessagesOfBot(botId)
				.Skip(startIndex)
				.Take(count)
				.Include(msg => msg.From)
				.ToArrayAsync();

			return messages.Select(msg => _mapper.Map<TelegramMessage, TelegramMessageInfoDto>(msg));
		}

		public async Task<int> GetMessagesCountAsync(int botId)
		{
			return await _db.Messages
				.GetMessagesOfBot(botId)
				.CountAsync();
		}
	}
}
