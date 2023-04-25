using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using IntegorTelegramBotListeningModel;

namespace IntegorTelegramBotListeningServices.DataContext
{
    public class IntegorTelegramBotListeningDataContext : DbContext
    {
		public DbSet<TelegramBot> Bots { get; set; } = null!;
		public DbSet<TelegramUser> Users { get; set; } = null!;
		public DbSet<TelegramMessage> Messages { get; set; } = null!;

        public IntegorTelegramBotListeningDataContext(DbContextOptions options)
			: base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}
