using IntegorTelegramBotListeningShared.Dto;

namespace IntegorTelegramBotListeningService.Dto
{
	public class BotStatisticsDto
	{
		public TelegramBotInfoDto Bot { get; set; } = null!;
		public int TotalEvents { get; set; }

        public BotStatisticsDto()
        {   
        }

        public BotStatisticsDto(TelegramBotInfoDto bot, int totalEvents = 0)
        {
			Bot = bot;
			TotalEvents = totalEvents;
        }
    }
}
