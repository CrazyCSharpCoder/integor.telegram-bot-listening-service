using AutoMapper;

using IntegorTelegramBotListeningDto;
using IntegorTelegramBotListeningServices.EntityFramework.Model;

namespace IntegorTelegramBotListeningService.Mapper.Profiles
{
	using Dto;

	public class TelegramBotMapperProfile : Profile
	{
        public TelegramBotMapperProfile()
        {
			CreateMap<TelegramBotInfoDto, EfTelegramBot>().ReverseMap();
			CreateMap<TelegramBotInputDto, TelegramBotInfoDto>();
		}
    }
}
