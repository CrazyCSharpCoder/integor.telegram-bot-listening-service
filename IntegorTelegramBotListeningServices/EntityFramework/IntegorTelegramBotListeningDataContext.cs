using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using IntegorLogicShared.Database.EntityFramework;

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

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TelegramBotWebhookInfo>(webhook =>
			{
				webhook.HasIndex(webhook => webhook.BotToken).IsUnique();
				webhook.Property(webhook => webhook.CreatedDate).HasColumnType("date");
			});
		}

		public override int SaveChanges(bool acceptAllChangesOnSuccess)
		{
			ProcessDateFields(ChangeTracker.Entries());

			return base.SaveChanges(acceptAllChangesOnSuccess);
		}

		public override Task<int> SaveChangesAsync(
			bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
		{
			ProcessDateFields(ChangeTracker.Entries());

			return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
		}

		private void ProcessDateFields(IEnumerable<EntityEntry> entries)
		{
			DateTimeInitializer dateTimeInitializer = new DateTimeInitializer();
			DateTimeUtcValidator utcValidator = new DateTimeUtcValidator();

			dateTimeInitializer.InitDateTimes(entries);
			utcValidator.SetToUtc(entries);
		}
	}
}
