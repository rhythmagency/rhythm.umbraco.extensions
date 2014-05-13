using System.Security.Cryptography;
using System.Text;

namespace Rhythm.Extensions.ExtensionMethods {
	public static class StringExtensionMethods {

		/// <summary>
		/// Creates an MD5 hash for a string.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>The MD5 hash.</returns>
		public static string ToMd5Hash(this string input) {
			var md5 = MD5.Create();
			var inputBytes = Encoding.ASCII.GetBytes(input);
			var hash = md5.ComputeHash(inputBytes);
			var sb = new StringBuilder();

			for (var i = 0; i < hash.Length; i++) {
				sb.Append(hash[i].ToString("X2"));
			}

			return sb.ToString();
		}

		/// <summary>
		/// Returns null of the input is only whitespace.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>
		/// The original string, or null if the original string is whitespace or an empty string.
		/// </returns>
		/// <remarks>
		/// This is useful when combined with the null coalescing operator.
		/// </remarks>
		public static string BlankToNull(this string input)
		{
			return string.IsNullOrWhiteSpace(input) ? null : input;
		}

	}
}