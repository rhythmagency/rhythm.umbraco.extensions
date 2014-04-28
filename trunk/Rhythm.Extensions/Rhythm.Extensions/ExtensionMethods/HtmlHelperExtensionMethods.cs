using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Rhythm.Extensions.ExtensionMethods {
	public static class HtmlHelperExtensionMethods {

		/// <summary>
		/// The regex to find invalid characters in a string (so they can be
		/// converted to a character that is valid in a CSS class).
		/// </summary>
		private static Regex InvalidClassChars { get; set; }

		/// <summary>
		/// Static constructor.
		/// </summary>
		static HtmlHelperExtensionMethods() {
			var defaultOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;
			InvalidClassChars = new Regex(@"((?![a-z0-9])(.|\r|\n))+|$[0-9]+", defaultOptions);
		}

		/// <summary>
		/// Converts a string to a CSS class (e.g., "hello, World" would be come "hello-world").
		/// </summary>
		/// <param name="helper">Ignored (just used to make this method an extension method).</param>
		/// <param name="str">The string to convert.</param>
		/// <returns>The CSS class.</returns>
		public static string GetCssClass(this HtmlHelper helper, string str) {
			return InvalidClassChars.Replace(str ?? string.Empty, "-").ToLower();
		}

		/// <summary>
		/// Ensures the specified URL starts with "http://" or "https://".
		/// </summary>
		/// <param name="helper">Ignored (just used to make this method an extension method).</param>
		/// <param name="url">The URL to normalize.</param>
		/// <returns>The normalized URL, or an empty string.</returns>
		public static string NormalizeUrl(this System.Web.Mvc.HtmlHelper helper, string url) {

			// Variables.
			url = url ?? string.Empty;
			var ignoreCase = StringComparison.InvariantCultureIgnoreCase;

			// Adjust schemeless URL's.
			if (url.StartsWith("//", ignoreCase)) {
				url = "http:" + url;
			}

			// Ensure the scheme is HTTP or HTTPS.
			if (!url.StartsWith("http://", ignoreCase) &&
				!url.StartsWith("https://", ignoreCase)) {

				// Avoid alternate schemes (e.g., FTP).
				if (Uri.IsWellFormedUriString(url, UriKind.Absolute)) {
					url = string.Empty;
				}
				else {
					url = "http://" + url;
				}

			}

			// URL is valid?
			if (Uri.IsWellFormedUriString(url, UriKind.Absolute)) {
				var sampleUrl = null as Uri;
				if (Uri.TryCreate(url, UriKind.Absolute, out sampleUrl)) {
					var scheme = sampleUrl.Scheme;
					if (scheme != Uri.UriSchemeHttp && scheme != Uri.UriSchemeHttps) {
						url = string.Empty;
					}
				}
				else {
					url = string.Empty;
				}
			}
			else {
				url = string.Empty;
			}

			// Return adjusted URL.
			return url;

		}

	}
}