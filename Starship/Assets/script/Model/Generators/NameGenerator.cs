using System;
using System.Text;

namespace Model
{
	namespace Generators
	{
		public static class NameGenerator
		{
			public static string GetStarName(int id)
			{
				var name = new StringBuilder();
				var random = new System.Random(id);

				var length = random.Next(1,5);
				CreateAlphaNumSequence(name, length, random);
				name.Append('-');
				length = 5-length + random.Next(3);
				CreateAlphaNumSequence(name, length, random);

				if (random.Next(10) == 0)
				{
					name.Append('-');
					CreateAlphaNumSequence(name, random.Next(1,4), random);
				}

				return name.ToString();
			}

			public static string GetPlanetName(int starId, int index)
			{
				return GetStarName(starId) + " " + ToRoman(index + 1);
			}

			private static void CreateAlphaNumSequence(StringBuilder builder, int length, System.Random random)
			{
				for (int i = 0; i < length; ++i)
				{
					if (random.Next()%3 == 0)
						builder.Append((char)random.Next('0','9'+1));
					else
						builder.Append((char)random.Next('A', 'Z'+1));
				}
			}

			private static string ToRoman(int number)
			{
				if ((number < 0) || (number > 3999))
					throw new ArgumentOutOfRangeException();
				if (number < 1) return string.Empty;            
				if (number >= 1000) return "M" + ToRoman(number - 1000);
				if (number >= 900) return "CM" + ToRoman(number - 900);
				if (number >= 500) return "D" + ToRoman(number - 500);
				if (number >= 400) return "CD" + ToRoman(number - 400);
				if (number >= 100) return "C" + ToRoman(number - 100);            
				if (number >= 90) return "XC" + ToRoman(number - 90);
				if (number >= 50) return "L" + ToRoman(number - 50);
				if (number >= 40) return "XL" + ToRoman(number - 40);
				if (number >= 10) return "X" + ToRoman(number - 10);
				if (number >= 9) return "IX" + ToRoman(number - 9);
				if (number >= 5) return "V" + ToRoman(number - 5);
				if (number >= 4) return "IV" + ToRoman(number - 4);
				if (number >= 1) return "I" + ToRoman(number - 1);
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}