using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Rhythm.Extensions.Utilities {
	
	/// <summary>
	/// Utility to help with strings.
	/// </summary>
	public class StringUtility {

		/// <summary>
		/// The regex to find sequences of whitespace (so they can be
		/// replaced with a single space).
		/// </summary>
		private static Regex WhitespaceRegex { get; set; }

		/// <summary>
		/// The regex to find invalid characters in string (so they can be
		/// converted to a character that is valid in a CSS class).
		/// </summary>
		private static Regex InvalidClassChars { get; set; }

		/// <summary>
		/// Matches lines in multi-line text.
		/// </summary>
		private static Regex LineRegex { get; set; }

		/// <summary>
		/// Static constructor.
		/// </summary>
		static StringUtility() {
			var defaultOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;
			WhitespaceRegex = new Regex("( |\t|\r|\n)+", defaultOptions);
			InvalidClassChars = new Regex(@"((?![a-z0-9_])(.|\r|\n))+", defaultOptions);
			LineRegex = new Regex(@"((?!\r|\n).)+", defaultOptions);
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

		/// <summary>
		/// Gets the text that would be displayed to a user of a web browser.
		/// </summary>
		/// <param name="markup">The markup.</param>
		/// <returns>The text.</returns>
		public static string RenderMarkupToText(string markup) {
			
			// Validation.
			if (string.IsNullOrWhiteSpace(markup)) {
				return markup;
			}

			// Variables.
			var text = null as string;

			// Remove HTML commments.
			var doc = new HtmlDocument();
			doc.LoadHtml(markup);
			doc.DocumentNode.DescendantsAndSelf().Where(x => x.NodeType == HtmlNodeType.Comment)
				.ToList().ForEach(x => x.Remove());

			// Get text.
			if (doc.DocumentNode == null) {
				text = string.Empty;
			} else {
				text = (doc.DocumentNode.InnerText ?? string.Empty).Trim();
			}

			// Unencode HTML entities.
			text = WebUtility.HtmlDecode(text) ?? string.Empty;

			// Reduce whitespace.
			text = (WhitespaceRegex.Replace(text, " ") ?? string.Empty).Trim();

			// Return text.
			return text;

		}

		/// <summary>
		/// Truncates a string, adding an ellipsis when necessary.
		/// </summary>
		/// <param name="str">The string to truncate.</param>
		/// <param name="maxLength">The maximum allowed length.</param>
		/// <param name="ellipsis">
		/// The ellipsis (can be any string to suffix after truncating).
		/// </param>
		/// <returns>The truncated string.</returns>
		/// <remarks>
		/// The ellipsis will only be suffixed if truncation is performed.
		/// The resulting string will not be longer than the max length,
		/// including the ellipsis.
		/// </remarks>
		public static string Truncate(string str, int maxLength, string ellipsis) {
			if (!string.IsNullOrWhiteSpace(str) && str.Length > maxLength) {
				var newLength = Math.Max(0, maxLength - ellipsis.Length);
				str = str.Substring(0, newLength).TrimEnd() + ellipsis;
				if (str.Length > maxLength) {
					str = str.Substring(0, maxLength);
				}
			}
			return str;
		}

		/// <summary>
		/// Splits text on line breaks.
		/// </summary>
		/// <param name="text">The text containing line breaks.</param>
		/// <returns>
		/// The individual lines.
		/// </returns>
		/// <remarks>
		/// Lines that are null or whitespace will be excluded.
		/// </remarks>
		public static string[] SplitOnLineBreaks(string text) {
			if (text == null) {
				return new string[] { };
			}
			if (string.IsNullOrEmpty(text)) {
				return new [] { text };
			} else {
				return LineRegex.Matches(text).Cast<Match>().Select(x => x.Value)
					.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
			}
		}

	}

}