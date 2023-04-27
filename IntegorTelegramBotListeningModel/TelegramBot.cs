using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace IntegorTelegramBotListeningModel
{
	public class TelegramBot
	{
		[Key]
		public int Id { get; set; }

		public string Title { get; set; } = null!;
		public string Token { get; set; } = null!;

		public DateTime CreatedDate { get; set; }
		public DateTime UpdatedDate { get; set; }
	}
}
