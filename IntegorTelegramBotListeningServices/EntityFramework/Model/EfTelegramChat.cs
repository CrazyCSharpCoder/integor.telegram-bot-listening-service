using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntegorTelegramBotListeningModel;

namespace IntegorTelegramBotListeningServices.EntityFramework.Model
{
	public class EfTelegramChat : TelegramChat
	{
		public virtual ICollection<EfTelegramMessage> Messages { get; set; } = null!;
	}
}
