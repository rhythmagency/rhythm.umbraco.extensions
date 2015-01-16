using System;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Web.Routing;

namespace Rhythm.Extensions.Routing {
	/// <summary>
	/// A URL provider with a special provision to allow for fragment identifiers.
	/// </summary>
	/// <remarks>
	/// In the current implementation, fragment identifiers only work when using
	/// extensionless URL's without trailing slashes.
	/// </remarks>
	public class FragmentUrlProvider : DefaultUrlProvider {
		private const string ErrorFragmentUrl = @"Error while attempting to formulate URL with fragment identifier.";

		/// <summary>
		/// Gets the URL of the content.
		/// </summary>
		public override string GetUrl(Umbraco.Web.UmbracoContext umbracoContext, int id,
			Uri current, UrlProviderMode mode) {

			// Variables.
			var url = base.GetUrl(umbracoContext, id, current, mode);
			var alias = null as string;

			// Avoid unnecessary work when possible by returning early.
			if (string.IsNullOrWhiteSpace(url)) {
				return url;
			}

			// Log errors.
			try {

				// Get the doctype alias from the cache or from the content service.
				if (umbracoContext.PublishedContentRequest == null) {

					// Get alias from content service.
					var service = umbracoContext.Application.Services.ContentService;
					var node = service.GetById(id);
					if (node != null) {
						alias = node.ContentType.Alias;
					}

				} else {

					// Get alias from cache.
					var node = umbracoContext.ContentCache.GetById(id);
					if (node != null) {
						alias = node.DocumentTypeAlias;
					}

				}

				// For document fragments, convert the last part to a fragment identifier.
				// Removes trailing slashes and the ".aspx" extension.
				if (alias != null && "DocumentFragment".InvariantEquals(alias)) {
					var originalUrl = url;
					var hashReplace = "#";
					if (url.EndsWith("/")) {
						url = url.TrimEnd("/".ToCharArray());
						hashReplace = "/#";
					}
					if (url.EndsWith(".aspx", StringComparison.InvariantCultureIgnoreCase)) {
						url = url.Substring(0, url.Length - 5);
					}
					var pos = url.LastIndexOf('/');
					if (pos >= 0) {
						url = url.Substring(0, pos) + hashReplace + url.Substring(pos + 1);
						if (url.StartsWith("#")) {
							url = "/" + url;
						}
					} else {
						url = originalUrl;
					}
				}

			}
			catch (Exception ex) {
				LogHelper.Error<FragmentUrlProvider>(ErrorFragmentUrl, ex);
			}

			// Return URL.
			return url;

		}

	}
}