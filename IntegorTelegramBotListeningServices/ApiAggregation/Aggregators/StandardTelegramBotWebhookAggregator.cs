using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net.Http.Json;

using Microsoft.Extensions.Options;

using IntegorTelegramBotListeningDto;
using IntegorTelegramBotListeningShared.ApiAggregation.Aggregators;

namespace IntegorTelegramBotListeningServices.ApiAggregation.Aggregators
{
	using ExternalServicesConfiguration;

	public class StandardTelegramBotWebhookAggregator : ITelegramBotWebhookAggregator
	{
		private IntegorDataServiceConfiguration _dataServiceConfiguration;

		public StandardTelegramBotWebhookAggregator(
			IOptions<IntegorDataServiceConfiguration> dataServiceOptions)
        {
			_dataServiceConfiguration = dataServiceOptions.Value;
        }

        public async Task AggregateAsync(TelegramUpdateDto update, long botId)
		{
			Uri uri = new Uri(_dataServiceConfiguration.Url);
			uri = new Uri(uri, $"telegram-events/aggregate-single/{botId}");

			using HttpContent content = JsonContent.Create(update);
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
