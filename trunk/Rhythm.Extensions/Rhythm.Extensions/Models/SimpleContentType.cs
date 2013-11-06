using Newtonsoft.Json.Linq;
using Umbraco.Core.Models;

namespace Rhythm.Extensions.Models {
	public class SimpleContentType {
		public int Id;
		public string Alias;
		public SimpleContentType Parent;

		/// <summary>
		/// Converts a SimpleContentType instance to a JSON string.
		/// </summary>
		/// <returns>JSON string representation of the SimpleContentType instance.</returns>
		public string ToJson() {
			return this.ToJObject().ToString();
		}

		/// <summary>
		/// Converts a SimpleContentType instance to a JObject.
		/// </summary>
		/// <returns>JObject representation of the SimpleContentType instance.</returns>
		public JObject ToJObject() {
			return JObject.FromObject(this);
		}

		public static SimpleContentType FromContentType(ContentType contentType, bool recurse = true) {
			if (contentType == null) return null;

			var result = new SimpleContentType() {
				Id = contentType.Id,
				Alias = contentType.Alias
			};

			if (recurse) {
				result.Parent = FromContentType(contentType.ParentId);
			}

			return result;
		}

		public static SimpleContentType FromContentType(int contentTypeId, bool recurse = true) {
			if (contentTypeId <= 0) return null;

			var dt = new ContentType(contentTypeId);
			return FromContentType(dt, recurse);
		}
	}
}
