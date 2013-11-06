using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Rhythm.Extensions.ExtensionMethods {
	/// <summary>
	/// Simple extension methods for the XmlDocument class.
	/// </summary>
	public static class XmlExtensionMethods {
		/// <summary>
		/// Converts an XmlDocument into a JSON string.
		/// </summary>
		/// <param name="document">The xml document to convert.</param>
		/// <returns>A JSON string representation of the xml document.</returns>
		public static string ToJson(this XmlDocument document) {
			return JsonConvert.SerializeXmlNode(document);
		}

		/// <summary>
		/// Converts an XmlDocument into a JObject.
		/// </summary>
		/// <param name="document">The xml document to convert.</param>
		/// <returns>A JObject representation of the xml document.</returns>
		public static JObject ToJObject(this XmlDocument document) {
			return JObject.Parse(document.ToJson());
		}

		/// <summary>
		/// Tries to parse a string into an XmlDocument and returns whether the operation was successful.
		/// </summary>
		/// <param name="document">The XmlDocument to load the xml string into.</param>
		/// <param name="xml">The xml string to parse.</param>
		/// <returns>Returns whether the operation was successful or not.</returns>
		public static bool TryParseXml(this XmlDocument document, string xml) {
			try {
				document.LoadXml(xml);
				return true;
			} catch {
				return false;
			}
		}

		/// <summary>
		/// Tries to parse a string into an XmlDocument and returns whether the operation was successful.
		/// </summary>
		/// <param name="xml">The xml string to parse.</param>
		/// <returns>Returns whether the operation was successful or not.</returns>
		public static bool TryParseXml(string xml) {
			var document = new XmlDocument();

			try {
				document.LoadXml(xml);
				return true;
			} catch {
				return false;
			}
		}

		/// <summary>
		/// Loads an xml string into an XmlDocument and returns the XmlDocument instance.
		/// </summary>
		/// <param name="xml">The xml string to load.</param>
		/// <returns>Returns an XmlDocument that contains the xml passed in.</returns>
		public static XmlDocument LoadXml(string xml) {
			var document = new XmlDocument();
			document.LoadXml(xml);
			return document;
		}
	}
}