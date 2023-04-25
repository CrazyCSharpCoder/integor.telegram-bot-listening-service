using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.Dto;
using IntegorTelegramBotListeningShared.EventsAggregation;

using AutoMapper;

namespace IntegorTelegramBotListeningService.Controllers
{
	using Dto;

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

			return Ok(addedBot);
		}

		[HttpPut("{botId}")]
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

			return Ok(updateResult);
		}

		[HttpGet("{botToken}")]
		public async Task<IActionResult> GetBotAsync(string botToken)
		{
			TelegramBotInfoDto? bot = await _botsManagement.GetByTokenAsync(botToken);

			if (bot == null)
				// TODO replace with json
				return NotFound();

			int messagesCount = await _messagesAggregator.GetMessagesCountAsync(bot.Id);

			return Ok(new BotStatisticsDto(bot, messagesCount));
		}
	}
}
