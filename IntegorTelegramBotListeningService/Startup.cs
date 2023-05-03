using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Microsoft.AspNetCore.Builder;

using Microsoft.EntityFrameworkCore;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.Bots;
using IntegorTelegramBotListeningShared.Configuration;

using IntegorTelegramBotListeningShared.ApiRetranslation;
using IntegorTelegramBotListeningShared.ApiRetranslation.ApiContent;

using IntegorTelegramBotListeningShared.EventsAggregation;

using IntegorTelegramBotListeningServices;
using IntegorTelegramBotListeningServices.ApiRetranslation;
using IntegorTelegramBotListeningServices.ApiRetranslation.ApiContent;

using IntegorTelegramBotListeningServices.EntityFramework;
using IntegorTelegramBotListeningServices.Bots;
using IntegorTelegramBotListeningServices.EventsAggregation;

namespace IntegorTelegramBotListeningService
{
    using Filters;
    using Helpers;
    using Mapper.Profiles;

    public class Startup
	{
		public IConfiguration Configuration { get; }

		private IConfiguration _telegramBotListeningServiceConfiguration;
		private IConfiguration _telegramBotApiConfiguration;

        public Startup(IConfiguration configuration)
        {
			Configuration = configuration;

			_telegramBotListeningServiceConfiguration = new ConfigurationBuilder()
				.AddJsonFile("service_config.json")
				.Build();

			_telegramBotApiConfiguration = new ConfigurationBuilder()
				.AddJsonFile("telegram_bot_api_config.json")
				.Build();
        }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IJsonSerializerOptionsProvider, StandardJsonSerializerOptionsProvider>();

			services.AddControllers().AddJsonOptions(options =>
			{
				IJsonSerializerOptionsProvider optionsProvider = services
					.BuildServiceProvider()
					.GetRequiredService<IJsonSerializerOptionsProvider>();

				optionsProvider.AssignJsonSerizalizerOptions(options.JsonSerializerOptions);
			});

			services.Configure<TelegramBotListeningServiceConfiguration>(_telegramBotListeningServiceConfiguration);
			services.Configure<TelegramBotApiConfiguration>(_telegramBotApiConfiguration);

			services.AddSingleton<ITelegramBotApiUriBuilder, StandardTelegramBotApiUriBuilder>();

			services.AddSingleton<IMultipartContentTypesTransformer, StandardMultipartContentTypesTransformer>();
			services.AddSingleton<IBotApiHttpContentFactory, StandardBotApiHttpContentFactory>();
			services.AddSingleton<ITelegramBotApiGate, StandardTelegramBotApiGate>();
			services.AddSingleton<IHttpResponseMessageToHttpResponseAssigner, StandardHttpResponseMessageToHttpResponseAssigner>();

			services.AddScoped<ITelegramBotEventsAggregator, TelegramBotUpdatesAggregator>();

			services.AddDbContext<IntegorTelegramBotListeningDataContext>(
				options =>
				{
					options.UseNpgsql(Configuration.GetConnectionString("IntegorTelegramBotEventsDatabase"));
					options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
				});

			services.AddScoped<IBotsManagementService, EntityFrameworkBotsManagementService>();
			services.AddScoped<IBotWebhookManagementService, EntityFrameworkBotWebhookManagementService>();

			services.AddScoped<IChatsAggregationService, EntityFrameworkChatsAggregationService>();
			services.AddScoped<IUsersAggregationService, EntityFrameworkUsersAggregationService>();
			services.AddScoped<IMessagesAggregationService, EntityFrameworkMessagesAggregationService>();

			services.AddAutoMapper(
				typeof(TelegramBotMapperProfile),
				typeof(TelegramUserMapperProfile),
				typeof(TelegramChatMapperProfile),
				typeof(TelegramMessageMapperProfile));

			services.AddSingleton<HttpRequestHelper>();
			services.AddScoped<EntityFrameworkTransactionFilter>();
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
