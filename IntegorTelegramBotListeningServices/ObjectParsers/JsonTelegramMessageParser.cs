using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using IntegorTelegramBotListeningDto;
using IntegorResponseDecoration.Parsing;

namespace IntegorTelegramBotListeningServices.ObjectParsers
{
	public class JsonTelegramMessageParser
		: IDecoratedObjectParser<TelegramMessageInfoDto, JsonElement>
	{
		public DecoratedObjectParsingResult<TelegramMessageInfoDto> ParseDecorated(JsonElement decoratedObject)
		{
			JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = true
			};

			TelegramMessageInfoDto message =
				decoratedObject.Deserialize<TelegramMessageInfoDto>(jsonOptions)!;

			return new DecoratedObjectParsingResult<TelegramMessageInfoDto>(message);
		}
	}
}
