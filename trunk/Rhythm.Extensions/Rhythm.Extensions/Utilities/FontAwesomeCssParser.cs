using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Web;

namespace Rhythm.Extensions.Utilities {
	/// <summary>
	/// Service responsible for applying the regex and retrieving the class names
	/// Modified from https://github.com/marcemarc/monosnow.umbraco.uCssClassNameDropdown/blob/master/Services/ClassNameRetrievalService.cs
	/// </summary>
	public class FontAwesomeCssParser {
		private readonly string pathToCss = "/css/font-awesome.css";
		private readonly string exceptions = "large";
		private readonly string cssClassRegEx = @"\.icon-([^:]*?):before";

		public FontAwesomeCssParser() { }
		/// <summary>
		/// Checks if filename exists
		/// </summary>
		public Boolean CssFileExists { get { return (File.Exists(HttpContext.Current.Server.MapPath(pathToCss))); } }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="pathToCss">path to Css File Name</param>
		public FontAwesomeCssParser(string pathToCss) {
			this.pathToCss = String.IsNullOrEmpty(pathToCss) ? this.pathToCss : pathToCss; ;

		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="pathToCss">Path to Css File name</param>
		/// <param name="exceptions">List of classes to ignore</param>
		public FontAwesomeCssParser(string pathToCss, string exceptions) {

			this.pathToCss = String.IsNullOrEmpty(pathToCss) ? this.pathToCss : pathToCss;
			this.exceptions = String.IsNullOrEmpty(exceptions) ? this.exceptions : exceptions;

		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="pathToCss">Path to Css File name</param>
		/// <param name="exceptions">List of classes to ignore</param>
		/// <param name="cssClassRegEx">Regex to match classnames, first capture group is the text used</param>
		public FontAwesomeCssParser(string pathToCss, string exceptions, string cssClassRegEx) {
			this.pathToCss = String.IsNullOrEmpty(pathToCss) ? this.pathToCss : pathToCss; ;
			this.cssClassRegEx = String.IsNullOrEmpty(cssClassRegEx) ? this.cssClassRegEx : cssClassRegEx;
			this.exceptions = String.IsNullOrEmpty(exceptions) ? this.exceptions : exceptions;

		}
		/// <summary>
		/// Method to get the CssClass names from the file by applying the regex
		/// </summary>
		/// <returns>An enumeration of class names</returns>
		public IEnumerable<string> GetClassNames() {
			var cssContents = GetCssFileContents();
			// use regex to find all the class names
			var cssClassNames = new List<string>();

			// Define a regular expression for repeated words.
			var rx = new Regex(cssClassRegEx, RegexOptions.Compiled | RegexOptions.IgnoreCase);

			// Find matches.
			var matches = rx.Matches(cssContents);
			var ex = this.exceptions.Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries);
			// loop through all the matches 
			foreach (var matchingText in matches.Cast<Match>().Select(match => match.Groups[1].Value.Trim()).Where(matchingText => matchingText.Length > 2 && !ex.Contains(matchingText.ToLower()) && !cssClassNames.Contains(matchingText))) {
				cssClassNames.Add(matchingText);
			}

			//sort the list alphabetically
			cssClassNames.Sort();
			return cssClassNames;

		}
		/// <summary>
		/// method to read the contents of the css file from disc
		/// contents are added to server cache, and cache is attempted to be read first
		/// until file changes.
		/// </summary>
		/// <returns>string containing contents of css file</returns>
		private string GetCssFileContents() {
			var cssContents = String.Empty;
			if (this.CssFileExists) {
				ObjectCache cache = MemoryCache.Default;
				cssContents = (string)cache[pathToCss + "CssFileContents"];
				if (String.IsNullOrEmpty(cssContents)) {
					if (File.Exists(HttpContext.Current.Server.MapPath(pathToCss))) {
						var cssFileStream = new StreamReader(HttpContext.Current.Server.MapPath(pathToCss));
						cssContents = cssFileStream.ReadToEnd();
						cssFileStream.Close();
						if (!String.IsNullOrEmpty(cssContents)) {
							var policy = new CacheItemPolicy();

							var filePaths = new List<string> {HttpContext.Current.Server.MapPath(pathToCss)};

							policy.ChangeMonitors.Add(new
							HostFileChangeMonitor(filePaths));

							cache.Add(pathToCss + "CssFileContents", cssContents, policy);
						}
					}
				}

			}
			return cssContents;

		}

	}
}