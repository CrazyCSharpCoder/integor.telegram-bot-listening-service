using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.ApiContent;
using IntegorTelegramBotListeningShared.Configuration;

using IntegorTelegramBotListeningServices;
using IntegorTelegramBotListeningServices.ApiContent;

namespace IntegorTelegramBotListeningService
{
	using ApiRetranslation;

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
			services.AddScoped<TelegramBotApiRetranslator>();

			services.AddControllers();

			services.Configure<TelegramBotApiConfiguration>(_telegramBotApiConfiguration);

			services.AddSingleton<IBotApiHttpContentFactory, StandardBotApiHttpContentParser>();
			services.AddSingleton<IHttpResponseMessageToHttpResponseAssigner, StandardHttpResponseMessageToHttpResponseAssigner>();
		}

        public void Configure(IApplicationBuilder app)
		{
			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			//RouteHandler routeHandler = new RouteHandler(async context =>
			//{
			//	await context.RequestServices.GetRequiredService<TelegramBotApiRetranslator>().Invoke(context);
			//});

			//IRouter router = new RouteBuilder(app, routeHandler)
			//	.MapRoute("telegramBotApi", "bot{token}/{apiMethod}")
			//	.MapRoute("non-declared", "{*any}")
			//	.Build();

			//app.UseRouter(router);
		}
	}
}
