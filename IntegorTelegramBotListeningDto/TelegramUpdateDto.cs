using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningDto
{
	public class TelegramUpdateDto
	{
		public long UpdateId { get; set; }
		public TelegramMessageInfoDto? Message { get; set; }
	}
}
