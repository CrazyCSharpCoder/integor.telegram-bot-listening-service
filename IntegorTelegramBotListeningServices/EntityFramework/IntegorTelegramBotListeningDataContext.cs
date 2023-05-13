using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace IntegorTelegramBotListeningServices.EntityFramework
{
	using Model;

	public class IntegorTelegramBotListeningDataContext : DbContext
	{
		public DbSet<EfTelegramBot> Bots { get; set; } = null!;
		public DbSet<EfTelegramBotWebhookInfo> Webhooks { get; set; } = null!;

		public DbSet<EfTelegramUser> Users { get; set; } = null!;
		public DbSet<EfTelegramChat> Chats { get; set; } = null!;
		public DbSet<EfTelegramMessage> Messages { get; set; } = null!;

		public IntegorTelegramBotListeningDataContext(DbContextOptions options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<EfTelegramBotWebhookInfo>(webhook =>
			{
				webhook.HasOne(webhook => webhook.Bot)
					.WithOne(bot => bot.WebhookInfo)
					.HasForeignKey<EfTelegramBotWebhookInfo>(webhook => webhook.BotId);
			});

			modelBuilder.Entity<EfTelegramMessage>(message =>
			{
				message.HasKey(message => new { message.MessageId, message.ChatId });

				message.HasOne(message => message.ReplyToMessage)
					.WithMany()
					.HasForeignKey(message => new
						{
							message.ReplyToMessageId,
							message.ReplyToMessageChatId
						});
			});
		}
	}
}
