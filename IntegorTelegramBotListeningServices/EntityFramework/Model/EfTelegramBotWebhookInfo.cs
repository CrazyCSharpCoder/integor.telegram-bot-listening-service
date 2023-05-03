using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntegorTelegramBotListeningModel;

namespace IntegorTelegramBotListeningServices.EntityFramework.Model
{
	public class EfTelegramBotWebhookInfo : TelegramBotWebhookInfo
	{
		public virtual EfTelegramBot Bot { get; set; } = null!;
	}
}
