using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Rhythm.Extensions.Models {
	/// <summary>
	/// Class that represents the bare minimum data for a page to serialize to JSON
	/// </summary>
	public class SimpleContent {
		public int Id;
		public string Name;
		public string Url;
		public int Level;
		public Dictionary<string, object> Properties;

		/// <summary>
		/// Converts a SimpleContent instance to a JSON string.
		/// </summary>
		/// <returns>JSON string representation of the SimpleContent instance.</returns>
		public string ToJson() {
			return this.ToJObject().ToString();
		}

		/// <summary>
		/// Converts a SimpleContent instance to a JObject.
		/// </summary>
		/// <returns>JObject representation of the SimpleContent instance.</returns>
		public JObject ToJObject() {
			return JObject.FromObject(this);
		}

		/// <summary>
		/// Converts an IPublishedContent instance to SimpleContent.
		/// </summary>
		/// <param name="content">The IPublishedContent instance you wish to convert.</param>
		/// <returns>A SimpleContent representation of the specified IPublishedContent</returns>
		public static SimpleContent FromIPublishedContent(IPublishedContent content) {
			if (content == null) return null;

			/*
			 * Using string, object for key/value pairs.
			 * An object is used so that the JavaScriptSerializer will
			 * automatically detect the type and serialize it to the
			 * correct JavaScript type.
			 */
			var properties = content.Properties
				.Where(p => !String.IsNullOrWhiteSpace(p.Value.ToString()))
				.ToDictionary(prop => prop.Alias, prop => prop.Value);

			return new SimpleContent() {
				Id = content.Id,
				Name = content.Name,
				Url = content.Url,
				Level = content.Level,
				Properties = properties
			};
		}

		/// <summary>
		/// Converts a collection of IPublishedContent instances to a collection of SimpleContent instances.
		/// </summary>
		/// <param name="content">The IPublishedContent collection you wish to convert.</param>
		/// <returns>A collection of SimpleContent instances</returns>
		public static IEnumerable<SimpleContent> FromIPublishedContent(IEnumerable<IPublishedContent> content) {
			return content.Select(FromIPublishedContent);
		}
	}

	/// <summary>
	/// Extension methods for the SimpleContent class and related classes.
	/// </summary>
	public static class SimpleContentExtensionMethods {
		/// <summary>
		/// Converts a collection of SimpleContent instances to a JSON array string.
		/// </summary>
		/// <param name="content">The SimpleContent collection you wish to convert.</param>
		/// <returns>JSON array string representation of the specified SimpleContent collection.</returns>
		public static string ToJson(this IEnumerable<SimpleContent> content) {
			return content.ToJArray().ToString();
		}

		/// <summary>
		/// Converts a collection of SimpleContent instances to a JArray.
		/// </summary>
		/// <param name="content">The SimpleContent collection you wish to convert.</param>
		/// <returns>JArray representation of the specified SimpleContent collection.</returns>
		public static JArray ToJArray(this IEnumerable<SimpleContent> content) {
			return JArray.FromObject(content);
		}
	}
}
