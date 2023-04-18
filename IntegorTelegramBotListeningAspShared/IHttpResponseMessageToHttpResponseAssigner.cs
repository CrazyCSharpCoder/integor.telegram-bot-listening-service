using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

using Microsoft.AspNetCore.Http;

namespace IntegorTelegramBotListeningAspShared
{
	public interface IHttpResponseMessageToHttpResponseAssigner
	{
		Task AssignAsync(HttpResponse httpResponse, HttpResponseMessage responseMessage);
	}
}
