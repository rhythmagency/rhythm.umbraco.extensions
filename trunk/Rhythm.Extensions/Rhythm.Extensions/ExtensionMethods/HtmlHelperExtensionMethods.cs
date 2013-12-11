using System.Text.RegularExpressions;
using System.Web.Mvc;
using Umbraco.Core;

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
        /// Ensures the specified URL starts with "http://".
        /// </summary>
        /// <param name="helper">Ignored (just used to make this method an extension method).</param>
        /// <param name="url">The URL to normalize.</param>
        /// <returns>The normalized URL.</returns>
        public static string NormalizeUrl(this System.Web.Mvc.HtmlHelper helper, string url) {
            url = url ?? string.Empty;
            if (!url.InvariantStartsWith("http")) {
                url = "http://" + url;
            }
            return url;
        }

    }
}