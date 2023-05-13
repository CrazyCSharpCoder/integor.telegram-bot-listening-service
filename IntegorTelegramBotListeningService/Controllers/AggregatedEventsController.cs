using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using IntegorTelegramBotListeningDto;

using IntegorTelegramBotListeningShared.Bots;
using IntegorTelegramBotListeningShared.EventsAggregation;

namespace IntegorTelegramBotListeningService.Controllers
{
    using Dto;
    using Settings;

    [ApiController]
	[Route("events")]
	public class AggregatedEventsController : ControllerBase
	{
		private const string _eventsCountOutOfRangeErrorMessage =
			"Maximum size of {0} parameter is {1}";

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
		public async Task<IActionResult> GetEventsByTokenAsync(
			string botToken, [FromQuery] int startIndex,
			[FromQuery]
			[Range(0, PaginationSettings.MaxBotEventsCount,
				ErrorMessage = _eventsCountOutOfRangeErrorMessage)] int count)
		{
			TelegramBotInfoDto? bot = await _botsManagement.GetByTokenAsync(botToken);
			return await ProcessForBotAsync(bot, startIndex, count);
		}

		[HttpGet("{botId:int}")]
		public async Task<IActionResult> GetEventsByIdAsync(
			int botId, [FromQuery] int startIndex,
			// TODO add error message
			[FromQuery]
			[Range(0, PaginationSettings.MaxBotEventsCount,
				ErrorMessage = _eventsCountOutOfRangeErrorMessage)] int count)
		{
			TelegramBotInfoDto? bot = await _botsManagement.GetByIdAsync(botId);
			return await ProcessForBotAsync(bot, startIndex, count);
		}

		private async Task<IActionResult> ProcessForBotAsync(
			TelegramBotInfoDto? bot, int startIndex, int count)
		{
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
