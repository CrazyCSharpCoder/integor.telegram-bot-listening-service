using AutoMapper;

using IntegorTelegramBotListeningModel;
using IntegorTelegramBotListeningShared.Dto;

namespace IntegorTelegramBotListeningService.Mapper.Profiles
{
	public class TelegramUserMapperProfile : Profile
	{
        public TelegramUserMapperProfile()
        {
			CreateMap<TelegramUserInfoDto, TelegramUser>().ReverseMap();
        }
    }
}
