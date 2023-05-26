using AutoMapper;

using IntegorTelegramBotListeningModel;
using IntegorTelegramBotListeningDto.Webhook.WebhookInfo;

namespace IntegorTelegramBotListeningService.Mapper.Profiles
{
	public class WebhookMapperProfile : Profile
	{
        public WebhookMapperProfile()
        {
			CreateMap<TelegramBotWebhookInfo, WebhookMetaDto>().ReverseMap();
			CreateMap<TelegramApiWebhookInfoDto, TelegramApiWebhookPublicInfoDto>();
        }
    }
}
