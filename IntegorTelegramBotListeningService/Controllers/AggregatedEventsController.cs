using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.Dto;
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

			int totalEvents = await _messagesAggregator.GetMessagesCountAsync(bot.Id);
			if (totalEvents > startIndex)
				// TODO replace with json
				return new ContentResult()
				{
					StatusCode = StatusCodes.Status400BadRequest,
					Content = "Start index exceeds count of messages"
				};

			IEnumerable<TelegramMessageInfoDto> messages =
				await _messagesAggregator.GetMessagesAsync(bot.Id, startIndex, count);

			return Ok(new BotMessagesDto(bot, messages, totalEvents));
		}
	}
}
