using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningShared.ApiAggregation.Aggregators
{
    public interface ITelegramBotApiAggregator : ITelegramApiAggregator<Stream>
    {
        Task<bool> AllowAggregationAsync(string apiMethod);
    }
}
