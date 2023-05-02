using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Mvc.Filters;

using IntegorTelegramBotListeningServices.EntityFramework;

namespace IntegorTelegramBotListeningService.Filters
{
	public class EntityFrameworkTransactionFilter : IAsyncActionFilter
	{
		private IntegorTelegramBotListeningDataContext _db;

		public EntityFrameworkTransactionFilter(IntegorTelegramBotListeningDataContext db)
        {
			_db = db;
        }

        public async Task OnActionExecutionAsync(
			ActionExecutingContext context, ActionExecutionDelegate next)
		{
			IDbContextTransaction transaction = await _db.Database.BeginTransactionAsync();
			ActionExecutedContext executedContext = await next();

			bool hasError = executedContext.Exception != null && !executedContext.ExceptionHandled;

			if (!hasError && executedContext.ModelState.IsValid)
			{
				await _db.SaveChangesAsync();
				await transaction.CommitAsync();
			}
			else
			{
				await transaction.RollbackAsync();
			}
		}
	}
}
