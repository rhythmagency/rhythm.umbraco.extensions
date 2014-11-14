namespace Rhythm.Extensions.Helpers {

	// Namespaces.
	using System.Web;
	using Umbraco.Web;

	/// <summary>
	/// Helps with Umbraco content.
	/// </summary>
	public static class ContentHelper {

		#region Constants

		private const string UmbracoHelperKey = "RueUmbracoHelper";

		#endregion

		#region Methods

		/// <summary>
		/// Gets an UmbracoHelper.
		/// </summary>
		/// <returns>An UmbracoHelper.</returns>
		/// <remarks>
		/// This is optimized to create only one UmbracoHelper per HTTP request.
		/// </remarks>
		public static UmbracoHelper GetHelper() {
			var items = HttpContext.Current.Items;
			var exists = items.Contains(UmbracoHelperKey);
			var helper = exists
				? items[UmbracoHelperKey] as UmbracoHelper
				: new UmbracoHelper(UmbracoContext.Current);
			if (!exists) {
				items[UmbracoHelperKey] = helper;
			}
			return helper;
		}

		#endregion

	}

}