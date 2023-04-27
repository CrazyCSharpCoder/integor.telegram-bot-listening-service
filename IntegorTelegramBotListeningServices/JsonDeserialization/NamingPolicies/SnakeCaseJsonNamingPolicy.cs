using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntegorTelegramBotListeningServices.JsonDeserialization.NamingPolicies
{
	public class SnakeCaseJsonNamingPolicy : JsonNamingPolicy
	{
		private enum CharacterClasses { UpperCase, LowerCase, Number, Underscore };

		private CharacterClasses GetCharacterClass(char character)
		{
			if (char.IsLower(character))
				return CharacterClasses.LowerCase;

			if (char.IsUpper(character))
				return CharacterClasses.UpperCase;

			if (char.IsNumber(character))
				return CharacterClasses.Number;

			return CharacterClasses.Underscore;
		}

		public override string ConvertName(string name)
		{
			if (name.Length == 1)
				return name;

			char lastChar = name.Last();

			List<char> newName = new List<char>() { char.ToLower(lastChar) };
			List<CharacterClasses> wordClasses = new List<CharacterClasses>()
			{
				GetCharacterClass(lastChar)
			};

			for (int i = name.Length - 2; i >= 0; i--)
			{
				char current = name[i];
				CharacterClasses currentClass = GetCharacterClass(current);

				if (NewWord(currentClass, wordClasses))
				{
					wordClasses = new List<CharacterClasses>() { currentClass };
					PrependList(newName, '_');
				}
				else
				{
					PrependList(wordClasses, currentClass);
				}

				PrependList(newName, char.ToLower(current));
			}

			return new string(newName.ToArray());
		}

		private bool NewWord(CharacterClasses newCharacterClass, List<CharacterClasses> wordClasses)
		{
			CharacterClasses firstWordClass = wordClasses.First();

			if (newCharacterClass == CharacterClasses.Underscore || firstWordClass == CharacterClasses.Underscore)
				return false;

			if (newCharacterClass != CharacterClasses.UpperCase)
				return newCharacterClass != firstWordClass;

			// Uppercase

			if (firstWordClass == CharacterClasses.LowerCase)
				return false;

			if (firstWordClass == newCharacterClass)
				return wordClasses.Count > 1 && wordClasses[1] == CharacterClasses.LowerCase;

			return newCharacterClass != firstWordClass;
		}

		private void PrependList<T>(List<T> list, T item) => list.Insert(0, item);
	}
}
