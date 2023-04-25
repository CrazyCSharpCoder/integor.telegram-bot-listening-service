using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Microsoft.AspNetCore.Builder;

using Microsoft.EntityFrameworkCore;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.Configuration;

using IntegorTelegramBotListeningShared.ApiRetranslation;
using IntegorTelegramBotListeningShared.ApiRetranslation.ApiContent;

using IntegorTelegramBotListeningShared.EventsAggregation;

using IntegorTelegramBotListeningServices;
using IntegorTelegramBotListeningServices.ApiRetranslation;
using IntegorTelegramBotListeningServices.ApiRetranslation.ApiContent;

using IntegorTelegramBotListeningServices.DataContext;
using IntegorTelegramBotListeningServices.EventsAggregation;

namespace IntegorTelegramBotListeningService
{
	using Mapper.Profiles;

    public class Startup
	{
		public IConfiguration Configuration { get; }

		private IConfiguration _telegramBotApiConfiguration;

        public Startup(IConfiguration configuration)
        {
			Configuration = configuration;

			_telegramBotApiConfiguration = new ConfigurationBuilder()
				.AddJsonFile("telegram_bot_api_config.json")
				.Build();
        }

		public void ConfigureServices(IServiceCollection services)
		{
			//services.AddScoped<TelegramBotApiRetranslator>();

			services.AddControllers();

			services.Configure<TelegramBotApiConfiguration>(_telegramBotApiConfiguration);

			services.AddSingleton<ITelegramBotApiUriBuilder, StandardTelegramBotApiUriBuilder>();

			services.AddSingleton<IBotApiHttpContentFactory, StandardBotApiHttpContentParser>();
			services.AddSingleton<ITelegramBotApiGate, StandardTelegramBotApiGate>();
			services.AddSingleton<IHttpResponseMessageToHttpResponseAssigner, StandardHttpResponseMessageToHttpResponseAssigner>();

			services.AddSingleton<ITelegramBotEventsAggregator, TelegramBotUpdatesAggregator>();

			services.AddDbContext<IntegorTelegramBotListeningDataContext>(
				options =>
				{
					options.UseNpgsql(Configuration.GetConnectionString("IntegorTelegramBotEventsDatabase"));
					options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
				});

			services.AddScoped<IBotsManagementService, EntityFrameworkBotsManagementService>();
			services.AddScoped<IUsersAggregationService, EntityFrameworkUsersAggregationService>();
			services.AddScoped<IMessagesAggregationService, EntityFrameworkMessagesAggregationService>();

			services.AddAutoMapper(
				typeof(TelegramBotMapperProfile),
				typeof(TelegramUserMapperProfile),
				typeof(TelegramMessageMapperProfile));
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
