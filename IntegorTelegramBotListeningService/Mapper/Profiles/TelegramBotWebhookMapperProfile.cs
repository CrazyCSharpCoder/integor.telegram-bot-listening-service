using AutoMapper;

using IntegorTelegramBotListeningModel;
using IntegorTelegramBotListeningServices.EntityFramework.Model;

namespace IntegorTelegramBotListeningService.Mapper.Profiles
{
	public class TelegramBotWebhookMapperProfile : Profile
	{
        public TelegramBotWebhookMapperProfile()
        {
			CreateMap<TelegramBotWebhookInfo, EfTelegramBotWebhookInfo>();
        }
    }
}
