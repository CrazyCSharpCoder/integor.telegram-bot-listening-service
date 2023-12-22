using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningServices.MultipartNamesEncoding
{
	public interface IMultipartNameEncoder
	{
		bool EncodingRequired(string fileName);
		string Encode(string fileName);
	}
}
