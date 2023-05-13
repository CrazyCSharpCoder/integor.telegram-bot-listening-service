using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningShared.Json.Converters
{
	public class UnixDateTimeJsonConverter : JsonConverter<DateTime>
	{
		private DateTime _unixTimeStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			int unixSeconds = reader.GetInt32();
			return _unixTimeStart.AddSeconds(unixSeconds);
		}

		public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
		{
			int unixSeconds = (value - _unixTimeStart).Seconds;
			writer.WriteNumberValue(unixSeconds);
		}
	}
}
