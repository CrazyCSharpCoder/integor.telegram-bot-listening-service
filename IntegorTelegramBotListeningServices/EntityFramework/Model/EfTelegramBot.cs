using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntegorTelegramBotListeningModel;

namespace IntegorTelegramBotListeningServices.EntityFramework.Model
{
	public class EfTelegramBot : TelegramBot
	{
		public virtual ICollection<EfTelegramMessage> RelatedMessages { get; set; } = null!;
	}
}
