using System.Collections.Generic;

using IntegorTelegramBotListeningShared.Dto;

namespace IntegorTelegramBotListeningService.Dto
{
	public class BotMessagesDto : BotStatisticsDto
	{
		public IEnumerable<TelegramMessageInfoDto> Messages { get; set; } = null!;

        public BotMessagesDto()
        {
        }

        public BotMessagesDto(
			TelegramBotInfoDto bot, IEnumerable<TelegramMessageInfoDto> messages, int totalEvents)
			: base(bot, totalEvents)
        {
			Messages = messages;
        }
    }
}
