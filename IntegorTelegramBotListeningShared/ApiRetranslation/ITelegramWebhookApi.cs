using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;

using IntegorTelegramBotListeningDto.Webhook;
using IntegorTelegramBotListeningDto.Webhook.WebhookInfo;

namespace IntegorTelegramBotListeningShared.ApiRetranslation
{
    public interface ITelegramWebhookApi
    {
		Task<HttpResponseMessage> GetWebhookInfoAsync(string botToken, string httpMethod);
		Task<HttpResponseMessage> SetWebhookAsync(string botToken, string httpMethod, TelegramSetWebhookDto webhook);
        Task<HttpResponseMessage> DeleteWebhookAsync(string botToken, string httpMethod, DeleteWebhookDto deleteDto);
    }
}
