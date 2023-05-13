using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntegorTelegramBotListeningDto;

namespace IntegorTelegramBotListeningShared.Bots
{
    public interface IBotsManagementService
    {
        Task<TelegramBotInfoDto> AddAsync(TelegramBotInfoDto bot);
        Task<TelegramBotInfoDto> UpdateAsync(TelegramBotInfoDto bot);

        Task<TelegramBotInfoDto?> GetByIdAsync(int id);
        Task<TelegramBotInfoDto?> GetByTokenAsync(string botToken);

		Task<IEnumerable<TelegramBotInfoDto>> GetAllAsync();
    }
}
