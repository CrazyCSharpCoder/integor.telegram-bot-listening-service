using AutoMapper;

using IntegorTelegramBotListeningModel;

using IntegorTelegramBotListeningShared.Dto;
using IntegorTelegramBotListeningShared.Dto.Input;

namespace IntegorTelegramBotListeningService.Mapper.Profiles
{
	public class TelegramMessageMapperProfile : Profile
	{
        public TelegramMessageMapperProfile()
        {
			CreateMap<InputMessageDto, TelegramMessage>();
			CreateMap<TelegramMessage, TelegramMessageInfoDto>();
		}
	}
}
