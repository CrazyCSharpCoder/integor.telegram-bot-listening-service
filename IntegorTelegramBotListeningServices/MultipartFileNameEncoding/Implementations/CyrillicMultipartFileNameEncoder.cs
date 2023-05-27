using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningServices.MultipartNamesEncoding.Implementations
{
	public class CyrillicMultipartFileNameEncoder : IMultipartNameEncoder
	{
		private const string _regex = "^[0-9a-zа-яё ,.!+-=()\\[\\]\\{\\}\\';%$№#`~]*$";

		private Dictionary<char, string> _cyrillicToLatin = new Dictionary<char, string>()
		{
			{ 'а', "a" }, { 'б', "b" }, { 'в', "v" }, { 'г', "g" }, { 'д', "d" },
			{ 'е', "ye" }, { 'ё', "yo" }, { 'ж', "zh" }, { 'з', "z" }, { 'и', "i" },

			{ 'й', "y" }, { 'к', "k" }, { 'л', "l" }, { 'м', "m" }, { 'н', "n" },
			{ 'о', "o" }, { 'п', "p" }, { 'р', "r" }, { 'с', "s" }, { 'т', "t" },

			{ 'у', "u" }, { 'ф', "f" }, { 'х', "kh" }, { 'ц', "ts" }, { 'ч', "ch" },
			{ 'ш', "sh" }, { 'щ', "shch" }, { 'ъ', "''" }, { 'ы', "y" }, { 'ь', "'" },

			{ 'э', "e" }, { 'ю', "yu" }, { 'я', "ya" }
		};

		public bool EncodingRequired(string fileName)
		{
			if (!fileName.Any(chr => chr >= 'а' && chr <= 'я'))
				return false;

			return Regex.IsMatch(fileName.ToLower(), _regex);
		}

		public string Encode(string fileName)
		{
			// Старый нерабочий вариант

			//Encoding encoding = Encoding.GetEncoding("windows-1251");

			//byte[] bytes = Encoding.UTF8.GetBytes(fileName);
			//return encoding.GetString(bytes);

			IEnumerable<string> codedSymbols =
				fileName.Select(chr => ConvertChar(chr));

			return string.Concat(codedSymbols);
		}

		private string ConvertChar(char chr)
		{
			string? matched = _cyrillicToLatin
				.GetValueOrDefault(char.ToLower(chr));

			// Если не кириллица, оставляем прежнее значение
			if (matched == null)
				return chr.ToString();

			// Если в нижнем регистре, не изменяем (так как
			// в таблице все сопоставления даны в нижнем регистре)
			if (char.IsLower(chr))
				return matched;

			// Если в верхнем регистре, и сопоставимое значение состоит
			// из одного символа, приводим к верхнему регистру
			if (matched.Length == 1)
				return matched.ToUpper();

			// Если в верхнем регистре, и сопоставимое значение состоит
			// из нескольких символов, приводим только первый символ
			// значения к верхнему регистру
			return char.ToUpper(matched.First()) + matched.Substring(1, matched.Length - 1);
		}
	}
}
