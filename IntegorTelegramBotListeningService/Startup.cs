using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Microsoft.AspNetCore.Builder;

using Microsoft.EntityFrameworkCore;

using ExternalServicesConfiguration;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.Bots;
using IntegorTelegramBotListeningShared.Configuration;

using IntegorTelegramBotListeningShared.ApiRetranslation;
using IntegorTelegramBotListeningShared.ApiRetranslation.ApiContent;

using IntegorTelegramBotListeningShared.ApiAggregation.Aggregators;
using IntegorTelegramBotListeningShared.ApiAggregation.DataDeserialization;

using IntegorTelegramBotListeningServices;
using IntegorTelegramBotListeningServices.ApiRetranslation;
using IntegorTelegramBotListeningServices.ApiRetranslation.ApiContent;

using IntegorTelegramBotListeningServices.EntityFramework;
using IntegorTelegramBotListeningServices.Bots;

using IntegorTelegramBotListeningServices.ApiAggregation.Aggregators;
using IntegorTelegramBotListeningServices.ApiAggregation.DataDeserialization;

namespace IntegorTelegramBotListeningService
{
    using Filters;
    using Helpers;

    public class Startup
	{
		public IConfiguration Configuration { get; }

		private IConfiguration _telegramBotListeningServiceConfiguration;

		private IConfiguration _telegramBotApiConfiguration;
		private IConfiguration _dataServiceConfiguration;

        public Startup(IConfiguration configuration)
        {
			Configuration = configuration;

			_telegramBotListeningServiceConfiguration = new ConfigurationBuilder()
				.AddJsonFile("serviceConfig.json")
				.Build();

			_telegramBotApiConfiguration = new ConfigurationBuilder()
				.AddJsonFile("ExternalServices/telegramBotApiConfig.json")
				.Build();

			_dataServiceConfiguration = new ConfigurationBuilder()
				.AddJsonFile("ExternalServices/integorDataServiceConfig.json")
				.Build();
        }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IJsonSerializerOptionsProvider, StandardJsonSerializerOptionsProvider>();

			services.AddCors(options =>
			{
				options.AddDefaultPolicy(builder => builder
					.AllowAnyOrigin()
					.AllowAnyHeader()
					.AllowAnyMethod());
			});

			services.AddControllers().AddJsonOptions(options =>
			{
				IJsonSerializerOptionsProvider optionsProvider = services
					.BuildServiceProvider()
					.GetRequiredService<IJsonSerializerOptionsProvider>();

				optionsProvider.AssignJsonSerizalizerOptions(options.JsonSerializerOptions);
			});

			services.Configure<TelegramBotListeningServiceConfiguration>(_telegramBotListeningServiceConfiguration);
			services.Configure<TelegramBotApiConfiguration>(_telegramBotApiConfiguration);
			services.Configure<IntegorDataServiceConfiguration>(_dataServiceConfiguration);

			services.AddSingleton<ITelegramBotApiUriBuilder, StandardTelegramBotApiUriBuilder>();

			services.AddSingleton<IMultipartContentTypesTransformer, StandardMultipartContentTypesTransformer>();
			services.AddSingleton<IBotApiHttpContentFactory, StandardBotApiHttpContentFactory>();
			services.AddSingleton<ITelegramBotApiGate, StandardTelegramBotApiGate>();
			services.AddSingleton<IWebhookInvoker, StandardWebhookInvoker>();
			services.AddSingleton<IHttpResponseMessageToHttpResponseAssigner, StandardHttpResponseMessageToHttpResponseAssigner>();

			services.AddScoped<ITelegramBotLongPollingAggregator, StandardTelegramBotLongPollingAggregator>();

			services.AddDbContext<IntegorTelegramBotListeningDataContext>(
				options =>
				{
					options.UseNpgsql(Configuration.GetConnectionString("IntegorTelegramBotEventsDatabase"));
					options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
				});

			services.AddScoped<IBotInfoAccessor, EntityFrameworkBotsManagementService>();
			services.AddScoped<IBotWebhookManagementService, EntityFrameworkBotWebhookManagementService>();

			services.AddScoped<IWebhookUpdateDeserializer, StandardWebhookUpdateDeserializer>();
			services.AddScoped<ILongPollingUpdatesDeserializer, StandardLongPollingUpdatesDeserializer>();

			services.AddScoped<ITelegramBotLongPollingAggregator, StandardTelegramBotLongPollingAggregator>();
			services.AddScoped<ITelegramBotWebhookAggregator, StandardTelegramBotWebhookAggregator>();

			services.AddSingleton<HttpRequestHelper>();
			services.AddScoped<EntityFrameworkTransactionFilter>();
		}

        public void Configure(IApplicationBuilder app)
		{
			app.UseRouting();
			app.UseCors();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
