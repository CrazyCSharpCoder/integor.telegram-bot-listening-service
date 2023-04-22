using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Microsoft.AspNetCore.Builder;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.ApiContent;
using IntegorTelegramBotListeningShared.Configuration;

using IntegorTelegramBotListeningServices;
using IntegorTelegramBotListeningServices.ApiContent;

namespace IntegorTelegramBotListeningService
{
	public class Startup
	{
		private IConfiguration _telegramBotApiConfiguration;

        public Startup()
        {
			_telegramBotApiConfiguration = new ConfigurationBuilder()
				.AddJsonFile("telegram_bot_api_config.json")
				.Build();
        }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			services.Configure<TelegramBotApiConfiguration>(_telegramBotApiConfiguration);

			services.AddSingleton<IBotApiHttpContentFactory, StandardBotApiHttpContentFactory>();
			services.AddSingleton<IHttpResponseMessageToHttpResponseAssigner, StandardHttpResponseMessageToHttpResponseAssigner>();
		}

        public void Configure(IApplicationBuilder app)
		{
			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
