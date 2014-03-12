using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Rhythm.Extensions.Models
{
    public partial class SimpleContent
    {
        /// <summary>
		/// Converts an IPublishedContent instance to SimpleContent.
		/// </summary>
		/// <param name="content">The IPublishedContent instance you wish to convert.</param>
		/// <param name="recurseChildren">Whether to include the children in the SimpleContent instance.</param>
		/// <param name="recurseContentTypes">Whether to include the parent content types on each SimpleContent instance.</param>
		/// <param name="recurseTemplates">Whether to include the parent templates on each SimpleContent instance.</param>
		/// <returns>A SimpleContent representation of the specified IPublishedContent</returns>
		public static SimpleContent FromIPublishedContent(IPublishedContent content, bool recurseChildren = true, bool recurseContentTypes = true, bool recurseTemplates = true) {
			if (content == null) return null;

			/*
			 * Using string, object for key/value pairs.
			 * An object is used so that the JavaScriptSerializer will
			 * automatically detect the type and serialize it to the
			 * correct JavaScript type.
			 */
			var properties =
				content.Properties
				       .Where(p => !String.IsNullOrWhiteSpace(p.Value.ToString()))
				       .ToDictionary(prop => prop.Alias, prop => prop.Value);
			
			var result = new SimpleContent() {
				Id = content.Id,
				Name = content.Name,
				Url = content.Url,
				Level = content.Level,
				Properties = properties,
				ContentType = SimpleContentType.FromContentType(content.DocumentTypeId, recurseContentTypes),
				Template = SimpleTemplate.FromTemplate(content.TemplateId, recurseTemplates),
				ChildrenIds = content.Children.Select(x => x.Id).ToList()
			};

			if (recurseChildren) {
				result.Children = FromIPublishedContent(content.Children, true, recurseContentTypes, recurseTemplates);
			}

			return result;
		}
    }
}