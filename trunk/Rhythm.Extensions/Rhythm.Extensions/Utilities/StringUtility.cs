using System.Linq;
using System.Text.RegularExpressions;

namespace Rhythm.Extensions.Utilities {
	
	/// <summary>
	/// Utility to help with strings.
	/// </summary>
	public class StringUtility {

		/// <summary>
		/// The regex to find invalid characters in string (so they can be
		/// converted to a character that is valid in a CSS class).
		/// </summary>
		private static Regex InvalidClassChars { get; set; }

		/// <summary>
		/// Static constructor.
		/// </summary>
		static StringUtility()
		{
			var defaultOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;
			InvalidClassChars = new Regex(@"((?![a-z0-9_])(.|\r|\n))+", defaultOptions);
		}

		/// <summary>
		/// Splits a CSV into a collection.
		/// </summary>
		/// <param name="csv">The comma-separated values string.</param>
		/// <returns>The collection of values.</returns>
		/// <remarks>
		/// Items are trimmed of whitespace and empty items are excluded.
		/// </remarks>
		public static string[] SplitCsv(string csv) {
			if (string.IsNullOrWhiteSpace(csv)) {
				return new string[] { };
			}
			else {
				return csv.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
			}
		}

		/// <summary>
		/// Converts a string into a CSS class.
		/// </summary>
		/// <param name="value">The value to convert to a CSS class.</param>
		/// <returns></returns>
		/// <remarks>
		/// Invalid characters are replaced with a dash.
		/// Dash sequences are replaced with a single dash.
		/// </remarks>
		public static string ToCssClass(string value) {
			value = InvalidClassChars.Replace((value ?? string.Empty), "-").ToLower();
			return value;
		}

	}

}