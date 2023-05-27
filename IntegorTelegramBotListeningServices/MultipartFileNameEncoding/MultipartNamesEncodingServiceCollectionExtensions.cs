using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace IntegorTelegramBotListeningServices.MultipartNamesEncoding
{
	public static class MultipartNamesEncodingServiceCollectionExtensions
	{
		public static IServiceCollection AddMultipartFileNamesEncoding(
			this IServiceCollection services,
			ServiceLifetime lifetime, bool injectEncoders,
			params Type[] nameEncoderTypes)
		{
			if (injectEncoders)
				InjectFileNameEncoders(services, lifetime, nameEncoderTypes);

			Type providerType = typeof(MultipartNamesEncodersProvider);

			Func<IServiceProvider, object> providerFactory =
				serviceProvider => new MultipartNamesEncodersProvider(serviceProvider, nameEncoderTypes);

			ServiceDescriptor providerDescriptor =
				new ServiceDescriptor(providerType, providerFactory, lifetime);

			services.Add(providerDescriptor);

			return services;
		}

		public static IServiceCollection AddMultipartNamesEncoding(
			this IServiceCollection services,
			ServiceLifetime lifetime, params Type[] nameEncoderTypes)
		{
			return services.AddMultipartFileNamesEncoding(lifetime, true, nameEncoderTypes);
		}

		public static IServiceCollection AddMultipartFileNamesEncoding(
			this IServiceCollection services, params Type[] nameEncoderTypes)
		{
			return services.AddMultipartFileNamesEncoding(ServiceLifetime.Singleton, true, nameEncoderTypes);
		}


		private static void InjectFileNameEncoders(
			this IServiceCollection services,
			ServiceLifetime lifetime, Type[] nameEncoderTypes)
		{
			foreach (Type encoderType in nameEncoderTypes)
			{
				ServiceDescriptor encoderDescriptor =
					new ServiceDescriptor(encoderType, encoderType, lifetime);

				services.Add(encoderDescriptor);
			}
		}
	}
}
