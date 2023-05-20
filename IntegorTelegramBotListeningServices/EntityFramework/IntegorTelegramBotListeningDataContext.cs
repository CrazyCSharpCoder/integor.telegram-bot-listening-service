using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using IntegorTelegramBotListeningModel;

namespace IntegorTelegramBotListeningServices.EntityFramework
{
	public class IntegorTelegramBotListeningDataContext : DbContext
	{
		public DbSet<TelegramBotWebhookInfo> Webhooks { get; set; } = null!;

		public IntegorTelegramBotListeningDataContext(DbContextOptions options)
			: base(options)
		{
		}
	}
}
