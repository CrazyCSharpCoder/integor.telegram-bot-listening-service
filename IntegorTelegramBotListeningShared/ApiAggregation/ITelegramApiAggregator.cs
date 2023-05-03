using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningShared.ApiAggregation
{
	public interface ITelegramApiAggregator<T>
	{
		Task AggregateAsync(T data, int botId);
	}
}
