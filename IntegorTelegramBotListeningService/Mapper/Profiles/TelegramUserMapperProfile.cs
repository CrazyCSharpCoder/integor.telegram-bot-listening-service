using AutoMapper;

using IntegorTelegramBotListeningDto;
using IntegorTelegramBotListeningServices.EntityFramework.Model;

namespace IntegorTelegramBotListeningService.Mapper.Profiles
{
	public class TelegramUserMapperProfile : Profile
	{
        public TelegramUserMapperProfile()
        {
			CreateMap<TelegramUserInfoDto, EfTelegramUser>().ReverseMap();
        }
    }
}
