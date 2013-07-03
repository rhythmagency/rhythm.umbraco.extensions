using System;
using System.Linq;
using System.Text;

namespace Rhythm.Extensions.Utilities {
	/// <summary>
	/// Simple password generator class.
	/// </summary>
	public static class PasswordGenerator {
		private static readonly string[] Vowels = {"a", "ai", "au", "e", "ea", "ee", "i", "ia", "io", "o", "oa", "oi", "oo", "ou", "u"};
		private static readonly string[] Consonants = {"b", "c", "ch", "cl", "d", "f", "ff", "g", "gh", "gl", "j", "k", "l", "ll", "m", "mn", "n", "p", "ph", "ps", "r", "rh", "s", "sc", "sh", "sk", "st", "t", "th", "v", "w", "x", "y", "z"};
		private static readonly Random Randomizer = new Random();

		/// <summary>
		/// Generates a pronounceable password.
		/// </summary>
		/// <param name="length">How many characters the password should be.</param>
		/// <returns>Returns a pronounceable password that is the specified length.</returns>
		public static string Generate(int length = 15) {
			var sb = new StringBuilder();

			var vowel = Randomizer.Next(2) == 0;

			for (var i = 0; i < length; i++) {
				var elements = vowel ? Vowels : Consonants;

				sb.Append(elements[Randomizer.Next(elements.Count())]);

				vowel = !vowel;
			}

			return sb.ToString().Substring(0, length);
		}
	}
}
