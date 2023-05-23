using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using IntegorResponseDecoration.Parsing;
using IntegorTelegramBotListeningDto;

namespace IntegorTelegramBotListeningServices.ObjectParsers
{
	public class JsonTelegramBotParser
		: IDecoratedObjectParser<TelegramBotInfoDto, JsonElement>
	{
		private const string _botPropertyName = "bot";

		public DecoratedObjectParsingResult<TelegramBotInfoDto> ParseDecorated(JsonElement decoratedObject)
		{
			if (!decoratedObject.TryGetProperty(_botPropertyName, out JsonElement jsonBot))
				return new DecoratedObjectParsingResult<TelegramBotInfoDto>(false);

			JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = true
			};

			TelegramBotInfoDto parsedBot = jsonBot.Deserialize<TelegramBotInfoDto>(jsonOptions)!;

			return new DecoratedObjectParsingResult<TelegramBotInfoDto>(parsedBot);
		}
	}
}
