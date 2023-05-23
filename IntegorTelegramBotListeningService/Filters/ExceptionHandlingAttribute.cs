using System;
using System.Net.Sockets;

using Microsoft.EntityFrameworkCore;
using Npgsql;

using IntegorErrorsHandling.Filters;
using IntegorErrorsHandling.Converters;

namespace IntegorTelegramBotListeningService.Filters
{
	public class ExceptionHandlingAttribute : ExtensibleExceptionHandlingLazyFilterFactory
	{
        public ExceptionHandlingAttribute() : base(
			typeof(IExceptionErrorConverter<Exception>),
			typeof(IExceptionErrorConverter<InvalidOperationException>),
			typeof(IExceptionErrorConverter<ObjectDisposedException>),
			typeof(IExceptionErrorConverter<SocketException>),
			typeof(IExceptionErrorConverter<PostgresException>),
			typeof(IExceptionErrorConverter<DbUpdateException>))
        {
        }
    }
}
