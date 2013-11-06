using Newtonsoft.Json.Linq;
using umbraco;

namespace Rhythm.Extensions.Models {
	/// <summary>
	/// Class that represents the bare minimum data for a page to serialize to JSON
	/// </summary>
	public class SimpleTemplate {
		public string Alias;
		public SimpleTemplate Parent;

		/// <summary>
		/// Converts a SimpleTemplate instance to a JSON string.
		/// </summary>
		/// <returns>JSON string representation of the SimpleTemplate instance.</returns>
		public string ToJson() {
			return this.ToJObject().ToString();
		}

		/// <summary>
		/// Converts a SimpleTemplate instance to a JObject.
		/// </summary>
		/// <returns>JObject representation of the SimpleTemplate instance.</returns>
		public JObject ToJObject() {
			return JObject.FromObject(this);
		}

		public static SimpleTemplate FromTemplate(template tpl, bool recurse = true) {
			if (tpl == null) return null;

			var result = new SimpleTemplate() {
				Alias = tpl.TemplateAlias
			};

			if (recurse) {
				result.Parent = FromTemplate(tpl.MasterTemplate);
			}

			return result;
		}

		public static SimpleTemplate FromTemplate(int templateId, bool recurse = true) {
			if (templateId <= 0) return null;

			var tpl = new template(templateId);

			return FromTemplate(tpl, recurse);
		}
	}
}
