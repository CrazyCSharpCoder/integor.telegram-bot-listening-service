using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningDto
{
	public class TelegramUserInfoDto
	{
		public long Id { get; set; }
		public bool IsBot { get; set; }

		public string FirstName { get; set; } = null!;
		public string? LastName { get; set; }

		public string? Username { get; set; }

		public string? LanguageCode { get; set; }
		public bool? IsPremium { get; set; }

		public DateTime AggregatedDate { get; set; }
	}
}
