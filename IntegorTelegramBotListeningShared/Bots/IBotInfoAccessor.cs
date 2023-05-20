using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntegorTelegramBotListeningDto;

namespace IntegorTelegramBotListeningShared.Bots
{
    public interface IBotInfoAccessor
    {
        Task<TelegramBotInfoDto?> GetByTokenAsync(string botToken);
    }
}
