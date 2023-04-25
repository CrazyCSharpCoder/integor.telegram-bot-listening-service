using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegorTelegramBotListeningModel
{
	public class TelegramUser
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public long Id { get; set; }
		public bool IsBot { get; set; }

		public string FirstName { get; set; } = null!;
		public string? LastName { get; set; }

		// Не пометил как Unique, так как возможна следующая ситуация:

		// Пользователь А использовал бота, чьи сообщения агрегируются, и данные о его Username были
		// также сагрегированы в базу данных. В какой-то момент он перестал пользоваться ботом, и информация
		// о нём перестала обновляться. Далее пользователь принял решение изменить свой Username, но система
		// об этом не узнала, следовательно в данный момент она хранит неактуальную информацию о пользователе

		// Пользователь Б начал использовать бота уже после того, как пользователь А изменил свой Username,
		// и так совпало, что Username пользователя Б совпадает со старым Username пользователя А, когда
		// последний изспользовал бота с агрегируемыми сообщениями. Однако так как информация о пользователе А
		// не обновлялась, произошла коллизия Username'а пользователя Б и старого Username'а пользователя А

		// Подобные ситуации возможны, и, возможно, даже типичны, и они не должны приводить к ошибкам внутри
		// системы, поэтому проверка на уникальность данного значения не производится
		public string? Username { get; set; }

		public string? LanguageCode { get; set; }
		public bool? IsPremium { get; set; }

		// TODO по умолчанию присваивать дату создания
		public DateTime AggregatedDate { get; set; }

		public virtual ICollection<TelegramMessage> Messages { get; set; } = null!;
	}
}
