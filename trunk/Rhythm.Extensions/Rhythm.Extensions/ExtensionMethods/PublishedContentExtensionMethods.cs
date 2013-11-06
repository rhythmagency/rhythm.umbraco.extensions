using Newtonsoft.Json.Linq;
using Rhythm.Extensions.Models;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Rhythm.Extensions.ExtensionMethods {
	public static class PublishedContentExtensionMethods {
		public static string GetTitle(this IPublishedContent content) {
			return content.HasProperty(Constants.Properties.TITLE) ? content.GetPropertyValue<string>(Constants.Properties.TITLE) : content.Name;
		}

		public static string ToJson(this IPublishedContent content) {
			return SimpleContent.FromIPublishedContent(content).ToJson();
		}

		public static string ToJson(this IEnumerable<IPublishedContent> content) {
			return SimpleContent.FromIPublishedContent(content).ToJson();
		}

		public static JObject ToJObject(this IPublishedContent content) {
			return SimpleContent.FromIPublishedContent(content).ToJObject();
		}

		public static JArray ToJArray(this IEnumerable<IPublishedContent> content) {
			return SimpleContent.FromIPublishedContent(content).ToJArray();
		}
	}
}