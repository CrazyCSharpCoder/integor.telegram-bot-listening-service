﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningShared
{
	public interface IJsonSerializerOptionsProvider
	{
		JsonSerializerOptions GetJsonSerializerOptions();
		void AssignJsonSerizalizerOptions(JsonSerializerOptions target);
	}
}
