using AutoMapper;

using IntegorTelegramBotListeningModel;
using IntegorTelegramBotListeningShared.Dto;
using IntegorTelegramBotListeningService.Dto;

namespace IntegorTelegramBotListeningService.Mapper.Profiles
{
	public class TelegramBotMapperProfile : Profile
	{
        public TelegramBotMapperProfile()
        {
			CreateMap<TelegramBotInfoDto, TelegramBot>().ReverseMap();
			CreateMap<TelegramBotInputDto, TelegramBotInfoDto>();
		}
    }
}
