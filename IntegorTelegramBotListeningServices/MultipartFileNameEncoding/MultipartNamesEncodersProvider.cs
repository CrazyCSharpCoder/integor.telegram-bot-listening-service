using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace IntegorTelegramBotListeningServices.MultipartNamesEncoding
{
	public class MultipartNamesEncodersProvider : IEnumerable<IMultipartNameEncoder>
	{
		public Type[] FileNameConvertersOrder { get; }

		private IServiceProvider _serviceProvider;

		public MultipartNamesEncodersProvider(
			IServiceProvider serviceProvider, Type[] converterTypes)
        {
			_serviceProvider = serviceProvider;
			FileNameConvertersOrder = converterTypes;
        }

		public TConverter GetConverter<TConverter>()
			where TConverter : IMultipartNameEncoder
		{
			return _serviceProvider
				.GetRequiredService<TConverter>();
		}

		public IMultipartNameEncoder GetConverter(Type converterType)
		{
			object service = _serviceProvider
				.GetRequiredService(converterType);

			return (IMultipartNameEncoder)service;
		}

		public IEnumerator<IMultipartNameEncoder> GetEnumerator()
		{
			foreach (Type converterType in FileNameConvertersOrder)
				yield return GetConverter(converterType);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach (Type converterType in FileNameConvertersOrder)
				yield return GetConverter(converterType);
		}
	}
}
