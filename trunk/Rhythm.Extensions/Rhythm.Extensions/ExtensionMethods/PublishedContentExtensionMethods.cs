using Newtonsoft.Json.Linq;
using Rhythm.Extensions.Models;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using Dimi.Polyglot.BLL;
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
using UmbracoLibrary = global::umbraco.library;

namespace Rhythm.Extensions.ExtensionMethods {
	public static class PublishedContentExtensionMethods {

        #region Variables

        private static readonly Regex LangRegex = new Regex("^[a-z]{2}(-[a-z]{2})?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
        /// <returns>The value.</returns>
        public static string LocalizedPropertyValue(this IPublishedContent source, string propertyAlias) {
            return LocalizedPropertyValue<string>(source, propertyAlias);
        }

        /// <summary>
        /// Returns the localized value of the specified property.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="source">The node with the property.</param>
        /// <param name="propertyAlias">The property alias.</param>
        /// <returns>The value with the specified type.</returns>
        public static T LocalizedPropertyValue<T>(this IPublishedContent source, string propertyAlias) {
            if (source == null)
            {
                return default(T);
            }
            else
            {
                return LocalizedPropertyValueHelper<T>(new DynamicNode(source.Id), propertyAlias);
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
            if (recursive) {
                while (source != null) {
                    if (source.HasValue(propertyAlias, false)) {
                        break;
                    }
                    source = source.Parent;
                }
            }
            if (source != null) {
                string pickerXml = source.LocalizedPropertyValue<string>(propertyAlias);
                if (pickerXml != null)
                {
                    int nodeId;

                    // Integer or XML?
                    if (int.TryParse(pickerXml, out nodeId)) {
                        var pickedNode = GetHelper().TypedContent(nodeId);
                        if (pickedNode != null) {
                            yield return pickedNode;
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(pickerXml as string)) {
                        var pickedNodes = new DynamicXml(pickerXml as string);
                        foreach (dynamic nodeItem in pickedNodes) {
                            nodeId = int.Parse(nodeItem.InnerText);
                            var pickedNode = GetHelper().TypedContent(nodeId);
                            if (pickedNode != null) {
                                yield return pickedNode;
                            }
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
                        return UmbracoLibrary.GetPreValueAsString(parsedValue);
                    } else {
                        return strValue;
                    }
                }
            }
            return null;
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

        #endregion

        #region Helper Methods

        /// <summary>
        /// Returns the localized value of the specified property.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="page">Page Content</param>
        /// <param name="propertyAlias">The property alias.</param>
        /// <returns>The value.</returns>
        private static T LocalizedPropertyValueHelper<T>(DynamicNode page, string propertyAlias) {

            // Variables.
            var selectedLanguage = LocalizationSelectedLanguage();
            var suffixedAlias = propertyAlias + "_" + selectedLanguage;
            var ignoreCase = StringComparison.InvariantCultureIgnoreCase;
            var caseIgnorer = StringComparer.InvariantCultureIgnoreCase;
            string[] empties = { "Content,False,,,", "<items />" };

            // Check current page for property with language suffix.
            if (page.HasProperty(suffixedAlias) && !string.IsNullOrEmpty(page.GetPropertyValue(suffixedAlias))) {
                if (UmbracoContext.Current.PageId != null)
                    return GetPropertyValue<T>(page.Id, suffixedAlias);
            }

            // Check child nodes for a translation folder?
            foreach (DynamicNode child in page.GetChildrenAsList) {
                if (!string.Equals(child.NodeTypeAlias, page.NodeTypeAlias + "_TranslationFolder", ignoreCase)) continue;
                foreach (DynamicNode translation in child.GetChildrenAsList) {
                    var language = translation.GetPropertyValue("language");
                    if (language == null || !string.Equals(language, selectedLanguage, ignoreCase)) continue;
                    if (translation.HasProperty(propertyAlias)
                        && !string.IsNullOrEmpty(translation.GetPropertyValue(propertyAlias))
                        && !empties.Contains(translation.GetPropertyValue(propertyAlias), caseIgnorer)) {
                        return GetPropertyValue<T>(translation.Id, propertyAlias);
                    }
                }
            }

            // Return the primary property?
            if (page.HasProperty(propertyAlias) && !string.IsNullOrEmpty(page.GetPropertyValue(propertyAlias))) {
                return GetPropertyValue<T>(page.Id, propertyAlias);
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
            return (new UmbracoHelper(UmbracoContext.Current)).TypedContent(nodeId).GetPropertyValue<T>(propertyAlias);
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