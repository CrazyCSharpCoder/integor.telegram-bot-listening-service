using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Microsoft.AspNetCore.Builder;

using Microsoft.EntityFrameworkCore;

using IntegorResponseDecoration.Parsing;
using IntegorAspHelpers.Middleware.WebApiResponse;

using IntegorServiceConfiguration;
using IntegorServiceConfiguration.IntegorServicesInteraction;

using ExternalServicesConfiguration;

using IntegorTelegramBotListeningDto;

using IntegorTelegramBotListeningShared;
using IntegorTelegramBotListeningShared.Bots;
using IntegorTelegramBotListeningShared.Configuration;

using IntegorTelegramBotListeningShared.ApiRetranslation;
using IntegorTelegramBotListeningShared.ApiRetranslation.ApiContent;

using IntegorTelegramBotListeningShared.ApiAggregation.Aggregators;

using IntegorTelegramBotListeningShared.DataDeserialization;

using IntegorTelegramBotListeningServices;
using IntegorTelegramBotListeningServices.Bots;

using IntegorTelegramBotListeningServices.ApiRetranslation;
using IntegorTelegramBotListeningServices.ApiRetranslation.ApiContent;
using IntegorTelegramBotListeningServices.ApiAggregation.Aggregators;

using IntegorTelegramBotListeningServices.ObjectParsers;
using IntegorTelegramBotListeningServices.DataDeserialization;

using IntegorTelegramBotListeningServices.EntityFramework;

using IntegorTelegramBotListeningServices.MultipartNamesEncoding;
using IntegorTelegramBotListeningServices.MultipartNamesEncoding.Implementations;

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
			// Configuring errors handling
			services.AddExceptionConverting();
			services.AddDatabaseExceptionConverters();

			services.AddPrimaryTypesErrorConverters();
			services.AddResponseErrorsObjectCompiler();

			// Configuring response decorators
			services.AddErrorResponseDecorator();

			// Configuring decorated object parsers
			services.AddIntegorServicesJsonErrorsParsing();
			services.AddSingleton<IDecoratedObjectParser<TelegramBotInfoDto, JsonElement>, JsonTelegramBotParser>();
			services.AddSingleton<IDecoratedObjectParser<TelegramMessageInfoDto, JsonElement>, JsonTelegramMessageParser>();

			// Configuring infrastructure
			services.AddSingleton<IJsonSerializerOptionsProvider, StandardJsonSerializerOptionsProvider>();

			services.AddControllers().AddJsonOptions(options =>
			{
				IJsonSerializerOptionsProvider optionsProvider = services
					.BuildServiceProvider()
					.GetRequiredService<IJsonSerializerOptionsProvider>();

				optionsProvider.AssignJsonSerizalizerOptions(options.JsonSerializerOptions);
			});

			services.AddHttpContextAccessor();
			services.AddDefaultStatusCodeResponseBodyFactory();

			services.AddCors(options =>
			{
				options.AddDefaultPolicy(builder => builder
					.AllowAnyOrigin()
					.AllowAnyHeader()
					.AllowAnyMethod());
			});

			services.AddAutoMapper(typeof(WebhookMapperProfile));

			// Configuring options
			services.Configure<TelegramBotListeningServiceConfiguration>(_telegramBotListeningServiceConfiguration);
			services.Configure<TelegramBotApiConfiguration>(_telegramBotApiConfiguration);
			services.Configure<IntegorDataServiceConfiguration>(_dataServiceConfiguration);

			services.AddSingleton<ITelegramBotApiUriBuilder, StandardTelegramBotApiUriBuilder>();

			// Configuring api retranslation services
			services.AddSingleton<IMultipartContentTypesTransformer, StandardMultipartContentTypesTransformer>();
			services.AddSingleton<IBotApiHttpContentFactory, StandardBotApiHttpContentFactory>();
			services.AddSingleton<ITelegramBotApiGate, StandardTelegramBotApiGate>();
			services.AddSingleton<IWebhookInvoker, StandardWebhookInvoker>();
			services.AddSingleton<IHttpResponseMessageToHttpResponseAssigner, StandardHttpResponseMessageToHttpResponseAssigner>();
			services.AddSingleton<ITelegramWebhookApi, StandardTelegramWebhookApi>();

			services.AddDbContext<IntegorTelegramBotListeningDataContext>(
				options =>
				{
					options.UseNpgsql(Configuration.GetConnectionString("IntegorTelegramBotEventsDatabase"));
					options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
				});

			services.AddMultipartFileNamesEncoding(typeof(CyrillicMultipartFileNameEncoder));

			// Configuring data logic services
			services.AddScoped<IBotInfoAccessor, DataServiceBotInfoAccessor>();
			services.AddScoped<IBotWebhookManagementService, EntityFrameworkBotWebhookManagementService>();

			// Configuring aggregation services
			services.AddScoped<IWebhookUpdateDeserializer, StandardWebhookUpdateDeserializer>();
			services.AddScoped<ILongPollingUpdatesDeserializer, StandardLongPollingUpdatesDeserializer>();
			services.AddScoped<IWebhookInfoDeserializer, StandardWebhookInfoDeserializer>();

			services.AddScoped<ITelegramBotLongPollingAggregator, StandardTelegramBotLongPollingAggregator>();
			services.AddScoped<ITelegramBotWebhookAggregator, StandardTelegramBotWebhookAggregator>();
			services.AddScoped<ITelegramBotApiAggregator, StandardTelegramBotApiAggregator>();

			// Configuring extras
			services.AddSingleton<HttpRequestHelper>();
			services.AddScoped<EntityFrameworkTransactionFilter>();
		}

        public void Configure(IApplicationBuilder app)
		{
			app.UseWebApiExceptionsHandling();
			app.UseWebApiStatusCodesHandling();

			app.UseRouting();
			app.UseCors();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
