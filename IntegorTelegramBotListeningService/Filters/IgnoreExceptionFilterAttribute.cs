using System;

using Microsoft.AspNetCore.Mvc.Filters;

namespace IntegorTelegramBotListeningService.Filters
{
	public class IgnoreExceptionFilterAttribute : Attribute, IExceptionFilter
	{
		public void OnException(ExceptionContext context)
		{
		}
	}
}
