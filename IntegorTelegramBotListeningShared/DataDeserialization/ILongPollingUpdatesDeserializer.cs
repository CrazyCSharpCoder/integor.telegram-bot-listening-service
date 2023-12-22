using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntegorTelegramBotListeningDto;

namespace IntegorTelegramBotListeningShared.DataDeserialization
{
    public interface ILongPollingUpdatesDeserializer
        : ITelegramDataDeserializer<IEnumerable<TelegramUpdateDto>>
    {
    }
}
