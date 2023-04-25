using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningShared.ApiRetranslation
{
    public interface ITelegramBotApiUriBuilder
    {
        string CreateUri(string botToken, string apiMethod, Dictionary<string, string>? queryParameters);
    }
}
