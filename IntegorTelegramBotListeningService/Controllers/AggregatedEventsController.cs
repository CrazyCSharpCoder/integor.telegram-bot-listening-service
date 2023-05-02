using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using IntegorTelegramBotListeningDto;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.EventsAggregation;

namespace IntegorTelegramBotListeningService.Controllers
{
	using Dto;

	[ApiController]
	[Route("events")]
	public class AggregatedEventsController : ControllerBase
	{
		private IBotsManagementService _botsManagement;
		private IMessagesAggregationService _messagesAggregator;

		public AggregatedEventsController(
			IBotsManagementService botsManagement,
			IMessagesAggregationService messagesAggregator)
        {
			_botsManagement = botsManagement;
			_messagesAggregator = messagesAggregator;
        }

        [HttpGet("{botToken}")]
		public async Task<IActionResult> GetEventsAsync(
			string botToken, [FromQuery] int startIndex, [FromQuery] int count)
		{
			TelegramBotInfoDto? bot = await _botsManagement.GetByTokenAsync(botToken);

			if (bot == null)
				// TODO replace with json
				return NotFound();

			int totalEvents = await _messagesAggregator.GetBotMessagesCountAsync(bot.Id);

			if (startIndex == 0 && totalEvents == 0)
				return Ok(new BotMessagesDto(bot, new TelegramMessageInfoDto[0], totalEvents));

			if (startIndex >= totalEvents)
				// TODO replace with json
				return new ContentResult()
				{
					StatusCode = StatusCodes.Status400BadRequest,
					Content = "Start index must be between 0 and count of messages - 1"
				};

			IEnumerable<TelegramMessageInfoDto> messages =
				await _messagesAggregator.GetBotMessagesAsync(bot.Id, startIndex, count);

			return Ok(new BotMessagesDto(bot, messages, totalEvents));
		}
	}
}
