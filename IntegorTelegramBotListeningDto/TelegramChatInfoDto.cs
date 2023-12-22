using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningDto
{
	public class TelegramChatInfoDto
	{
		public long Id { get; set; }

		public string Type { get; set; } = null!;
		public string? Title { get; set; }

		public string? Username { get; set; }

		public string? FirstName { get; set; }
		public string? LastName { get; set; }

		public bool? IsForum { get; set; }
	}
}
