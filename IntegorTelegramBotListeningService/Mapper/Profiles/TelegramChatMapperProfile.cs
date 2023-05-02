using AutoMapper;

using IntegorTelegramBotListeningDto;
using IntegorTelegramBotListeningServices.EntityFramework.Model;

namespace IntegorTelegramBotListeningService.Mapper.Profiles
{
	public class TelegramChatMapperProfile : Profile
	{
        public TelegramChatMapperProfile()
        {
			CreateMap<TelegramChatInfoDto, EfTelegramChat>().ReverseMap();
        }
    }
}
