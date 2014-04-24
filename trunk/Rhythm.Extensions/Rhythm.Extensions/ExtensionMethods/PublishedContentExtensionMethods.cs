using Dimi.Polyglot.BLL;
using Newtonsoft.Json.Linq;
using Rhythm.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using DocumentTypes = Rhythm.Extensions.Constants.DocumentTypes;
using DynamicNode = umbraco.MacroEngines.DynamicNode;
using DynamicXml = Umbraco.Core.Dynamics.DynamicXml;
using Properties = Rhythm.Extensions.Constants.Properties;
using StringUtility = Rhythm.Extensions.Utilities.StringUtility;
using UmbracoLibrary = global::umbraco.library;

namespace Rhythm.Extensions.ExtensionMethods {
	public static class PublishedContentExtensionMethods {

		#region Variables

		private static readonly Regex LangRegex = new Regex(@"^[a-z]{2}(-[a-z]{2})?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static readonly Regex CsvRegex = new Regex(@"\s*[0-9](\s*,\s*[0-9]+)+\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		#endregion

		#region Cache

		private static Dictionary<int, Tuple<int?, DateTime>> TranslationCache { get; set; }
		private static object TranslationLock { get; set; }
		private static Dictionary<int, string> PrevalueCache { get; set; }
		private static object PrevalueLock { get; set; }

		#endregion

		#region Constructors

		static PublishedContentExtensionMethods() {
			TranslationCache = new Dictionary<int,Tuple<int?,DateTime>>();
			TranslationLock = new object();
			PrevalueCache = new Dictionary<int,string>();
			PrevalueLock = new object();
		}

		#endregion

		#region Extension Methods

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

		/// <summary>
		/// Returns the localized value of the specified property.
		/// </summary>
		/// <param name="source">The node with the property.</param>
		/// <param name="propertyAlias">The property alias.</param>
		/// <param name="recursive">
		/// Recursively check ancestors?
		/// </param>
		/// <returns>The value.</returns>
		public static string LocalizedPropertyValue(this IPublishedContent source, string propertyAlias, bool recursive = false) {
			return LocalizedPropertyValue<string>(source, propertyAlias, recursive);
		}

		/// <summary>
		/// Returns the localized value of the specified property.
		/// </summary>
		/// <typeparam name="T">The type to return.</typeparam>
		/// <param name="source">The node with the property.</param>
		/// <param name="propertyAlias">The property alias.</param>
		/// <param name="recursive">
		/// Recursively check ancestors?
		/// </param>
		/// <returns>The value with the specified type.</returns>
		public static T LocalizedPropertyValue<T>(this IPublishedContent source, string propertyAlias, bool recursive = false) {
			if (source == null)
			{
				return default(T);
			}
			else
			{
				return LocalizedPropertyValueHelper<T>(new DynamicNode(source.Id), propertyAlias, recursive);
			}
		}

		/// <summary>
		/// Gets the picked node.
		/// </summary>
		/// <param name="source">The node with the picker property.</param>
		/// <param name="propertyAlias">The alias of the picker property.</param>
		/// <param name="recursive">Recursively check ancestors (false by default)?</param>
		/// <returns>The picked node, the first of many picked nodes, or null.</returns>
		public static IPublishedContent LocalizedGetPickedNode(this IPublishedContent source, string propertyAlias, bool recursive = false) {
			return LocalizedGetPickedNodes(source, propertyAlias, recursive).FirstOrDefault();
		}

		/// <summary>
		/// Gets the picked nodes.
		/// </summary>
		/// <param name="source">The node with the picker property.</param>
		/// <param name="propertyAlias">The alias of the picker property.</param>
		/// <param name="recursive">Recursively check ancestors (false by default)?</param>
		/// <returns>The picked nodes.</returns>
		public static IEnumerable<IPublishedContent> LocalizedGetPickedNodes(this IPublishedContent source, string propertyAlias, bool recursive = false) {
			var nodeIds = source.LocalizedGetPickedNodeIds(propertyAlias, recursive);
			if (nodeIds.Any()) {
				var helper = GetHelper();
				foreach (var id in nodeIds) {
					var pickedNode = helper.TypedContent(id);
					if (pickedNode != null) {
						yield return pickedNode;
					}
				}
			}
		}

		/// <summary>
		/// Gets the ID's of the picked nodes.
		/// </summary>
		/// <param name="source">The node with the picker property.</param>
		/// <param name="propertyAlias">The alias of the picker property.</param>
		/// <param name="recursive">Recursively check ancestors (false by default)?</param>
		/// <returns>
		/// The picked node ID's.
		/// </returns>
		/// <remarks>
		/// This is faster than LocalizedGetPickedNodes when you only need node ID's.
		/// </remarks>
		public static IEnumerable<int> LocalizedGetPickedNodeIds(this IPublishedContent source,
			string propertyAlias, bool recursive = false)
		{
			if (recursive) {
				while (source != null) {
					if (source.HasValue(propertyAlias, false)) {
						break;
					}
					source = source.Parent;
				}
			}
			if (source != null) {
				string pickerValue = source.LocalizedPropertyValue<string>(propertyAlias);
				if (pickerValue != null)
				{
					int nodeId;

					// Integer, CSV, or XML?
					if (int.TryParse(pickerValue, out nodeId)) {
						yield return nodeId;
					}
					else if (CsvRegex.IsMatch(pickerValue))
					{
						var pickedNodes = StringUtility.SplitCsv(pickerValue);
						foreach (var nodeItem in pickedNodes)
						{
							nodeId = int.Parse(nodeItem.Trim());
							yield return nodeId;
						}
					}
					else if (!string.IsNullOrWhiteSpace(pickerValue as string)) {
						var pickedNodes = new DynamicXml(pickerValue as string);
						foreach (dynamic nodeItem in pickedNodes) {
							nodeId = int.Parse(nodeItem.InnerText);
							yield return nodeId;
						}
					}

				}
			}
		}

		/// <summary>
		/// Gets the picked media.
		/// </summary>
		/// <param name="source">The node with the picker property.</param>
		/// <param name="propertyAlias">The alias of the picker property.</param>
		/// <param name="recursive">Recursively check ancestors (false by default)?</param>
		/// <returns>The picked media node, or null.</returns>
		public static IPublishedContent LocalizedGetPickedMedia(this IPublishedContent source, string propertyAlias, bool recursive = false) {
			if (recursive) {
				while (source != null) {
					if (source.HasValue(propertyAlias, false)) {
						break;
					}
					source = source.Parent;
				}
			}
			if (source != null) {
				int? pickedId = source.LocalizedPropertyValue<int?>(propertyAlias);
				if (pickedId.HasValue) {
					return GetHelper().TypedMedia(pickedId.Value);
				}
			}
			return null;
		}

		/// <summary>
		/// Gets a setting value.
		/// </summary>
		/// <typeparam name="T">The type of the setting value.</typeparam>
		/// <param name="source">The node to start the search at.</param>
		/// <param name="settingKey">The setting key (i.e., the name of the setting node).</param>
		/// <returns>The setting value.</returns>
		/// <remarks>The nearest ancestor with the specified setting will be used.</remarks>
		public static T LocalizedGetSetting<T>(this IPublishedContent source, string settingKey) {
			//TODO: Cache by page ID and key. Maybe allow a duration to be configured on the setting node?
			if (!string.IsNullOrWhiteSpace(settingKey))
			{
				while (source != null)
				{
					var settingsNode = source.Children.Where(x => DocumentTypes.SETTINGS.InvariantEquals(x.DocumentTypeAlias)).FirstOrDefault();
					if (settingsNode != null)
					{
						var settingNode = settingsNode.Children.Where(x => settingKey.InvariantEquals(x.Name)).FirstOrDefault();
						if (settingNode != null)
						{
							return settingNode.LocalizedPropertyValue<T>(Properties.VALUE);
						}
					}
					source = source.Parent;
				}
			}
			return default(T);
		}

		/// <summary>
		/// Gets a drop down value (aka, a pre-value) as a string.
		/// </summary>
		/// <param name="source">The node to start the search at.</param>
		/// <param name="propertyAlias">The alias of the drop down property.</param>
		/// <param name="recursive">Recursively check ancestors (false by default)?</param>
		/// <returns>The selected drop down value.</returns>
		public static string LocalizedDropDownValue(this IPublishedContent source, string propertyAlias, bool recursive = false) {
			if (recursive) {
				while (source != null) {
					if (source.HasValue(propertyAlias, false)) {
						break;
					}
					source = source.Parent;
				}
			}
			if (source != null) {
				var objectValue = source.LocalizedPropertyValue<object>(propertyAlias);
				if (objectValue != null) {
					int parsedValue;
					string strValue = objectValue.ToString();
					if (!string.IsNullOrWhiteSpace(strValue) && int.TryParse(strValue, out parsedValue)) {
						lock (PrevalueLock) {
							if (PrevalueCache.TryGetValue(parsedValue, out strValue)) {
								return strValue;
							}
							else {
								strValue = UmbracoLibrary.GetPreValueAsString(parsedValue);
								PrevalueCache[parsedValue] = strValue;
							}
						}
					} else {
						return strValue;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the drop down values (aka, pre-values) as a collection of strings.
		/// </summary>
		/// <param name="source">The node to start the search at.</param>
		/// <param name="propertyAlias">The alias of the drop down property.</param>
		/// <param name="recursive">Recursively check ancestors (false by default)?</param>
		/// <returns>The selected drop down values.</returns>
		public static string[] LocalizedDropDownValues(this IPublishedContent source, string propertyAlias, bool recursive = false)
		{
			var value = source.LocalizedDropDownValue(propertyAlias, recursive);
			return StringUtility.SplitCsv(value);
		}

		/// <summary>
		/// Searches for the nearest ancestor with the specified content type.
		/// </summary>
		/// <param name="source">The node to start searching from.</param>
		/// <param name="typeAlias">The alias of the content type.</param>
		/// <param name="includeSelf">Include the supplied node in the search (by default the search will start at the parent)?</param>
		/// <returns>The nearest matching ancestor, or null.</returns>
		public static IPublishedContent NearestAncestorOfType(this IPublishedContent source, string typeAlias, bool includeSelf = false)
		{
			if (!includeSelf && source != null)
			{
				source = source.Parent;
			}
			while (source != null)
			{
				if (typeAlias.InvariantEquals(source.DocumentTypeAlias))
				{
					return source;
				}
				source = source.Parent;
			}
			return null;
		}

		/// <summary>
		/// Returns all siblings of the specified types.
		/// </summary>
		/// <param name="source">The node to start the search at.</param>
		/// <param name="typeAliases">The aliases of the content types.</param>
		/// <returns>The siblings.</returns>
		/// <remarks>
		/// The source node is excluded from the results.
		/// </remarks>
		public static IEnumerable<IPublishedContent> SiblingsOfTypes(this IPublishedContent source,
			params string[] typeAliases) {
			return source.Siblings()
				.Where(x => x.Id != source.Id && typeAliases.Any(y => y.InvariantEquals(x.DocumentTypeAlias)));
		}

		/// <summary>
		/// Returns all descendants of the specified types.
		/// </summary>
		/// <param name="source">The node to start the search at.</param>
		/// <param name="typeAliases">The aliases of the content types.</param>
		/// <returns>
		/// The matching descendants.
		/// </returns>
		public static IEnumerable<IPublishedContent> DescendantsOfTypes(this IPublishedContent source,
			params string[] typeAliases) {
			if (source != null && typeAliases != null && typeAliases.Length > 0) {
				foreach (var child in source.Children) {
					if (typeAliases.Any(x => x.InvariantEquals(child.DocumentTypeAlias))) {
						yield return child;
					}
					foreach (var subchild in child.DescendantsOfTypes(typeAliases)) {
						yield return subchild;
					}
				}
			}
		}

		/// <summary>
		/// Returns all direct children of the specified types.
		/// </summary>
		/// <param name="source">The parent node.</param>
		/// <param name="typeAliases">The aliases of the content types.</param>
		/// <returns>
		/// The matching children.
		/// </returns>
		public static IEnumerable<IPublishedContent> ChildrenOfTypes(this IPublishedContent source,
			params string[] typeAliases) {
			if (source != null && typeAliases != null && typeAliases.Length > 0) {
				foreach (var child in source.Children) {
					if (typeAliases.Any(x => x.InvariantEquals(child.DocumentTypeAlias))) {
						yield return child;
					}
				}
			}
		}

		/// <summary>
		/// Finds the highest-level ancestor (typically the homepage).
		/// </summary>
		/// <param name="source">The node to start from.</param>
		/// <returns>The ancestor.</returns>
		public static IPublishedContent RootAncestor(this IPublishedContent source) {
			var parent = source;
			var ancestor = parent;
			while (parent != null) {
				ancestor = parent;
				parent = parent.Parent;
			}
			return ancestor;
		}

		/// <summary>
		/// Finds the first child that is a descendant that matches the specified list of content type aliases,
		/// relative to the specified page.
		/// </summary>
		/// <param name="source">The parent page to start the search.</param>
		/// <param name="typeAliases">The content type aliases.</param>
		/// <returns>The node, if one was found; otherwise, false.</returns>
		/// <remarks>
		/// Only looks at the first matching child at each step (for performance).
		/// </remarks>
		public static IPublishedContent ChildByTypePath(this IPublishedContent source, params string[] typeAliases) {
			var child = source;
			if (child != null) {
				foreach (var alias in typeAliases) {
					child = child.Children.Where(x => alias.InvariantEquals(x.DocumentTypeAlias)).FirstOrDefault();
					if (child == null) {
						break;
					}
				}
			}
			return child;
		}
		
		/// <summary>
		/// Finds the descendant children located by the specified list of content type aliases,
		/// relative to the specified page.
		/// </summary>
		/// <param name="source">The parent page to start the search.</param>
		/// <param name="typeAliases">The content type aliases.</param>
		/// <returns>The descendant nodes.</returns>
		/// <remarks>
		/// This is faster than Umbraco's implementation of Descendants() because this version
		/// does not need to scan the entire content tree under the specified node.
		/// </remarks>
		public static List<IPublishedContent> ChildrenByTypePath(this IPublishedContent source, params string[] typeAliases) {
			var children = new List<IPublishedContent>();
			if (source != null) {
				children.Add(source);
				foreach (var alias in typeAliases) {
					children = children.SelectMany(y =>
						y.Children.Where(x => alias.InvariantEquals(x.DocumentTypeAlias))).ToList();
					if (children.Count == 0) {
						break;
					}
				}
			}
			return children;
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Returns the localized value of the specified property.
		/// </summary>
		/// <typeparam name="T">The type to return.</typeparam>
		/// <param name="page">Page Content</param>
		/// <param name="propertyAlias">The property alias.</param>
		/// <param name="recursive">
		/// Recursively check ancestors?
		/// </param>
		/// <returns>The value.</returns>
		private static T LocalizedPropertyValueHelper<T>(DynamicNode page, string propertyAlias, bool recursive = false) {

			// Validation / base case.
			if (page == null)
			{
				return default(T);
			}

			// Variables.
			var selectedLanguage = LocalizationSelectedLanguage();
			var suffixedAlias = propertyAlias + "_" + selectedLanguage;
			var ignoreCase = StringComparison.InvariantCultureIgnoreCase;
			var caseIgnorer = StringComparer.InvariantCultureIgnoreCase;
			string[] empties = { "Content,False,,,", "<items />", "<values />" };

			// Check current page for property with language suffix.
			if (page.HasProperty(suffixedAlias) && !string.IsNullOrEmpty(page.GetPropertyValue(suffixedAlias))) {
				if (UmbracoContext.Current.PageId != null)
					return GetPropertyValue<T>(page.Id, suffixedAlias);
			}

			// Check child nodes for a translation folder?
			lock (TranslationLock) {

				// Variables.
				Tuple<int?, DateTime> translationInfo;
				bool recache = false;
				int? translationId = null;

				// Check the cache first (a hit is cached longer than a miss).
				if (TranslationCache.TryGetValue(page.Id, out translationInfo)) {
					translationId = translationInfo.Item1;
					if (translationInfo.Item1.HasValue) {
						var span = DateTime.Now.Subtract(translationInfo.Item2);
						if (span > TimeSpan.FromMinutes(5)) {
							recache = true;
						}
					}
					else {
						var span = DateTime.Now.Subtract(translationInfo.Item2);
						if (span > TimeSpan.FromMinutes(1)) {
							recache = true;
						}
					}
				}
				else {
					recache = true;
				}

				// Repopulate the cache?
				if (recache) {
					foreach (DynamicNode child in page.GetChildrenAsList) {
						if (!string.Equals(child.NodeTypeAlias, page.NodeTypeAlias + "_TranslationFolder", ignoreCase)) continue;
						translationId = child.Id;
						break;
					}
					TranslationCache[page.Id] = new Tuple<int?,DateTime>(translationId, DateTime.Now);
				}

				// Check translations under translation folder.
				if (translationId.HasValue) {
					var translationFolder = new DynamicNode(translationId.Value);
					foreach (DynamicNode translation in translationFolder.GetChildrenAsList) {
						var language = translation.GetPropertyValue("language");
						if (language == null || !string.Equals(language, selectedLanguage, ignoreCase)) continue;
						if (translation.HasProperty(propertyAlias))
						{
							var translationValue = translation.GetPropertyValue(propertyAlias);
							if (!string.IsNullOrEmpty(translationValue)
								&& !empties.Contains(translationValue, caseIgnorer)) {
								return GetPropertyValue<T>(translation.Id, propertyAlias);
							}
						}
					}
				}

			}

			// Return the primary property?
			if (page.HasProperty(propertyAlias)
				&& !string.IsNullOrEmpty(page.GetPropertyValue(propertyAlias))
				&& !empties.Contains(page.GetPropertyValue(propertyAlias), caseIgnorer)) {
				return GetPropertyValue<T>(page.Id, propertyAlias);
			}

			// Recursive?
			if (recursive) {
				return LocalizedPropertyValueHelper<T>(page.Parent, propertyAlias, recursive);
			}

			// Property not found. Return the default for the type.
			return default(T);

		}

		/// <summary>
		/// Gets the typed value of the property on the specified node ID.
		/// </summary>
		/// <typeparam name="T">The type of the value to return.</typeparam>
		/// <param name="nodeId">The node ID.</param>
		/// <param name="propertyAlias">The property alias.</param>
		/// <returns>The value of the specified type.</returns>
		private static T GetPropertyValue<T>(int nodeId, string propertyAlias) {

			// Variables.
			var page = (new UmbracoHelper(UmbracoContext.Current)).TypedContent(nodeId);

			// Special case for a multiple textstring.
			if (typeof(T) == typeof(string[]))
			{
				var result = page.GetPropertyValue<DynamicXml>(propertyAlias);
				if (result != null)
				{
					var strItems = new List<string>();
					foreach (dynamic child in result)
					{
						strItems.Add(child.InnerText as string);
					}
					return (T)(strItems.ToArray() as object);
				}
			}
			// Special case for a textstring array.
			else if (typeof(T) == typeof(List<string[]>))
			{
				var result = page.GetPropertyValue<DynamicXml>(propertyAlias);
				if (result != null)
				{
					var strItems = new List<string[]>();
					foreach (var child in result)
					{
						if ("values".InvariantEquals(child.BaseElement.Name.LocalName))
						{
							var subItems = new List<string>();
							foreach (var subChild in child)
							{
								if ("value".InvariantEquals(subChild.BaseElement.Name.LocalName))
								{
									subItems.Add(subChild.InnerText);
								}
							}
							strItems.Add(subItems.ToArray());
						}
					}
					return (T)(strItems as object);
				}
			}
			// Special case for multinode treepicker/CSV.
			else if (typeof(T) == typeof(int[]))
			{
				var result = page.GetPropertyValue<string>(propertyAlias) ?? string.Empty;
				var resultItems = StringUtility.SplitCsv(result);
				var intItems = new List<int>();
				int intItem;
				foreach (var item in resultItems)
				{
					if (int.TryParse(item, out intItem))
					{
						intItems.Add(intItem);
					}
				}
				return (T)(intItems.ToArray() as object);
			}

			// Fallback to Umbraco's implementation.
			return page.GetPropertyValue<T>(propertyAlias);

		}

		/// <summary>
		/// Returns user's selected language.
		/// </summary>
		/// <returns>The selected language.</returns>
		private static string LocalizationSelectedLanguage() {
			var selectedLanguage = "";
			var lang = HttpContext.Current.Request.Params["lang"];

			if (!string.IsNullOrEmpty(lang) && LangRegex.IsMatch(lang))
			{
				selectedLanguage = lang.Length == 5 ? string.Format("{0}{1}", lang.Substring(0, 3).ToLower(),
					lang.Substring(3, 2).ToUpper()) : lang.ToLower();
			}
			else
			{
				selectedLanguage = Languages.GetDefaultLanguage();
			}

			return selectedLanguage;
		}

		/// <summary>
		/// Gets an Umbraco helper.
		/// </summary>
		/// <returns>An Umbraco helper.</returns>
		private static UmbracoHelper GetHelper() {
			return (new UmbracoHelper(UmbracoContext.Current));
		}

		#endregion

	}
}