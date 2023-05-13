using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using IntegorTelegramBotListeningDto;

using IntegorTelegramBotListeningShared.Bots;
using IntegorTelegramBotListeningShared.EventsAggregation;

using AutoMapper;

namespace IntegorTelegramBotListeningService.Controllers
{
    using Dto;
    using Filters;

	[ApiController]
	[Route("bots")]
	public class BotsManagementController : ControllerBase
	{
		private IBotsManagementService _botsManagement;
		private IMessagesAggregationService _messagesAggregator;

		private IMapper _mapper;

		public BotsManagementController(
			IBotsManagementService botsManagement,
			IMessagesAggregationService messagesAggregator,

			IMapper mapper)
        {
			_botsManagement = botsManagement;
			_messagesAggregator = messagesAggregator;

			_mapper = mapper;
        }

        [HttpPost]
		[ServiceFilter(typeof(EntityFrameworkTransactionFilter))]
		public async Task<IActionResult> RegisterBotAsync(
			[FromBody] TelegramBotInputDto bot)
		{
			if (await _botsManagement.GetByTokenAsync(bot.Token) != null)
				// TODO replace with json
				return new ContentResult()
				{
					Content = "Bot with the specified token already exists",
					StatusCode = StatusCodes.Status400BadRequest
				};

			TelegramBotInfoDto addedBot = _mapper.Map<TelegramBotInputDto, TelegramBotInfoDto>(bot);
			addedBot = await _botsManagement.AddAsync(addedBot);

			return Ok(new BotStatisticsDto(addedBot));
		}

		[HttpPut("{botId}")]
		[ServiceFilter(typeof(EntityFrameworkTransactionFilter))]
		public async Task<IActionResult> UpdateBotAsync(
			int botId, [FromBody] TelegramBotInputDto bot)
		{
			TelegramBotInfoDto? oldBot =
				await _botsManagement.GetByIdAsync(botId);

			if (oldBot == null)
				// TODO replace with json
				return new ContentResult()
				{
					Content = "Bot with the specified id does not exists",
					StatusCode = StatusCodes.Status400BadRequest
				};

			TelegramBotInfoDto? sameTokenBot = await _botsManagement.GetByTokenAsync(bot.Token);

			if (sameTokenBot != null && oldBot.Id != sameTokenBot.Id)
				// TODO replace with json
				return new ContentResult()
				{
					Content = "Bot this token is already registered",
					StatusCode = StatusCodes.Status400BadRequest
				};

			_mapper.Map(bot, oldBot);

			TelegramBotInfoDto updateResult = await _botsManagement.UpdateAsync(oldBot);
			int messagesCount = await _messagesAggregator.GetBotMessagesCountAsync(botId);

			return Ok(new BotStatisticsDto(updateResult, messagesCount));
		}

		[HttpGet("{botToken}")]
		public async Task<IActionResult> GetBotByTokenAsync(string botToken)
		{
			TelegramBotInfoDto? bot = await _botsManagement.GetByTokenAsync(botToken);

			if (bot == null)
				// TODO replace with json
				return NotFound();

			int messagesCount = await _messagesAggregator.GetBotMessagesCountAsync(bot.Id);

			return Ok(new BotStatisticsDto(bot, messagesCount));
		}

		[HttpGet("{botId:int}")]
		public async Task<IActionResult> GetBotByIdAsync(int botId)
		{
			TelegramBotInfoDto? bot = await _botsManagement.GetByIdAsync(botId);

			if (bot == null)
				// TODO replace with json
				return NotFound();

			int messagesCount = await _messagesAggregator.GetBotMessagesCountAsync(bot.Id);

			return Ok(new BotStatisticsDto(bot, messagesCount));
		}

		[HttpGet("all")]
		public async Task<IActionResult> GetAllBotsAsync()
		{
			IEnumerable<TelegramBotInfoDto> bots = await _botsManagement.GetAllAsync();
			List<BotStatisticsDto> resultDtos = new List<BotStatisticsDto>();

			foreach (TelegramBotInfoDto bot in bots)
			{
				int messagesCount = await _messagesAggregator.GetBotMessagesCountAsync(bot.Id);
				resultDtos.Add(new BotStatisticsDto(bot, messagesCount));
			}

			return Ok(resultDtos);
		}
	}
}
