using System.Collections.Generic;
using System.Text.Json.Serialization;

using IntegorTelegramBotListeningDto;

namespace IntegorTelegramBotListeningService.Dto
{
	public class BotMessagesDto : BotStatisticsDto
	{
		[JsonPropertyOrder(int.MaxValue)]
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
