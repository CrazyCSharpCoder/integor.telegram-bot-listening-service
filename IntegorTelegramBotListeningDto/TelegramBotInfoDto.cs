using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningDto
{
	public class TelegramBotInfoDto
	{
		public long Id { get; set; }

		public string Title { get; set; } = null!;
		public string Token { get; set; } = null!;
	}
}
