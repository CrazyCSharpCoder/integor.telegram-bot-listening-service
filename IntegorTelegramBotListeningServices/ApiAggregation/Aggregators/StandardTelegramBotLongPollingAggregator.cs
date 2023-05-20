using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using IntegorTelegramBotListeningDto;

using IntegorTelegramBotListeningShared.ApiAggregation.Aggregators;

namespace IntegorTelegramBotListeningServices.ApiAggregation.Aggregators
{
    public class StandardTelegramBotLongPollingAggregator : ITelegramBotLongPollingAggregator
    {
        public StandardTelegramBotLongPollingAggregator()
        {
		}

		public Task AggregateAsync(IEnumerable<TelegramUpdateDto> data, int botId)
		{
			// TODO implement
			throw new NotImplementedException();
		}

	}
}
