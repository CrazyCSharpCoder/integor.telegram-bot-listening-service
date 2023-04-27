using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IntegorTelegramBotListeningShared
{
    public interface ITelegramBotEventsAggregator
    {
		Task<bool> AllowAggregationAsync(string botToken, string apiMethod, string? mediaType);
        Task AggregateAsync(Stream body);
    }
}
