using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Rhythm.Extensions.ExtensionMethods {
	public static class UmbracoHelperExtensionMethods {
		public static IPublishedContent GetContentAtRootByDocumentType(this UmbracoHelper helper, string documentType) {
			return helper.TypedContentAtRoot().FirstOrDefault(x => x.DocumentTypeAlias.Equals(documentType));
		}

		public static IPublishedContent GetHome(this UmbracoHelper helper) {
			return helper.GetContentAtRootByDocumentType(Constants.DocumentTypes.HOME);
		}

		public static IEnumerable<IPublishedContent> TypedSearchInHome(this UmbracoHelper helper, string query, int take = 999, int skip = 0) {
			var home = helper.GetHome();

			if (home == null || string.IsNullOrWhiteSpace(query)) return new List<IPublishedContent>();

			return helper
				.TypedSearch(query)
				.Where(x =>
					x.Path.Split(',').Contains(home.Id.ToString(CultureInfo.InvariantCulture))
					&& x.IsVisible()
					&& !x.TemplateId.Equals(0)
				)
				.Skip(skip)
				.Take(take);
		}
	}
}