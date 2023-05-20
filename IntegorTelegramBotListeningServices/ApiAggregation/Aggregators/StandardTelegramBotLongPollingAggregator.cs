using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net.Http.Json;

using Microsoft.Extensions.Options;

using ExternalServicesConfiguration;

using IntegorTelegramBotListeningDto;
using IntegorTelegramBotListeningShared.ApiAggregation.Aggregators;

namespace IntegorTelegramBotListeningServices.ApiAggregation.Aggregators
{
    public class StandardTelegramBotLongPollingAggregator : ITelegramBotLongPollingAggregator
    {
		private IntegorDataServiceConfiguration _dataServiceConfiguration;

		public StandardTelegramBotLongPollingAggregator(
			IOptions<IntegorDataServiceConfiguration> dataServiceOptions)
		{
			_dataServiceConfiguration = dataServiceOptions.Value;
		}

		public async Task AggregateAsync(IEnumerable<TelegramUpdateDto> updates, int botId)
		{
			Uri uri = new Uri(_dataServiceConfiguration.Url);
			uri = new Uri(uri, $"telegram-events/aggregate-many/{botId}");

			using HttpContent content = JsonContent.Create(updates);
			using HttpRequestMessage request =
				new HttpRequestMessage(HttpMethod.Post, uri.AbsoluteUri)
				{
					Content = content
				};

			using HttpClient client = new HttpClient();
			await client.SendAsync(request);
		}
	}
}
