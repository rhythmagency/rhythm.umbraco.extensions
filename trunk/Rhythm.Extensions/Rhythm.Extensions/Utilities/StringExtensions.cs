using System.Security.Cryptography;
using System.Text;

namespace Rhythm.Extensions.Utilities {
	public static class StringExtensions {
// ReSharper disable InconsistentNaming
		public static string ToMD5Hash(this string input) {
// ReSharper restore InconsistentNaming
			var md5 = MD5.Create();
			var inputBytes = Encoding.ASCII.GetBytes(input);
			var hash = md5.ComputeHash(inputBytes);
			var sb = new StringBuilder();

			for (var i = 0; i < hash.Length; i++) {
				sb.Append(hash[i].ToString("X2"));
			}

			return sb.ToString();
		}
	}
}
