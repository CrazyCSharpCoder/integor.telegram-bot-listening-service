using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

using Microsoft.Extensions.Options;

using ExternalServicesConfiguration;

using IntegorTelegramBotListeningDto;
using IntegorTelegramBotListeningShared.Bots;

namespace IntegorTelegramBotListeningServices.Bots
{
    public class EntityFrameworkBotsManagementService : IBotInfoAccessor
    {
		private IntegorDataServiceConfiguration _dataServiceConfiguration;

		public EntityFrameworkBotsManagementService(
			IOptions<IntegorDataServiceConfiguration> dataServiceOptions)
        {
			_dataServiceConfiguration = dataServiceOptions.Value;
		}

		public async Task<TelegramBotInfoDto?> GetByTokenAsync(string botToken)
		{
			Uri uri = new Uri(_dataServiceConfiguration.Url);
			uri = new Uri(uri, $"bot/{botToken}");

			using HttpRequestMessage request =
				new HttpRequestMessage(HttpMethod.Get, uri.AbsoluteUri);

			using HttpClient client = new HttpClient();
			using HttpResponseMessage response = await client.SendAsync(request);

			using Stream body = await response.Content.ReadAsStreamAsync();

			JsonElement jsonBody = await JsonSerializer.DeserializeAsync<JsonElement>(body);

			JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = true
			};

			return jsonBody.GetProperty("bot").Deserialize<TelegramBotInfoDto>(jsonOptions);
		}
	}
}
