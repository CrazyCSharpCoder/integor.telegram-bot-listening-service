using AutoMapper;

using IntegorTelegramBotListeningModel;
using IntegorTelegramBotListeningDto;

using IntegorTelegramBotListeningServices.EntityFramework.Model;

namespace IntegorTelegramBotListeningService.Mapper.Profiles
{
	public class TelegramMessageMapperProfile : Profile
	{
        public TelegramMessageMapperProfile()
        {
			CreateMap<TelegramMessage, EfTelegramMessage>();

			CreateMap<EfTelegramMessage, TelegramMessageInfoDto>();
			CreateMap<TelegramMessageInfoDto, TelegramMessage>()
				.ForMember(
					dest => dest.ReplyToMessageChatId,
					options => options.MapFrom(source => source.ReplyToMessage!.Chat.Id))
				.ForMember(
					dest => dest.ReplyToMessageId,
					options => options.MapFrom(source => source.ReplyToMessage!.MessageId));
		}
	}
}
