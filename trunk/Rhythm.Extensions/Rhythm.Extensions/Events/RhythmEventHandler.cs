namespace Rhythm.Extensions.Events
{

	// Namespaces.
	using Binding;
	using Routing;
	using System.Web.Mvc;
	using Umbraco.Core;
	using Umbraco.Web.Routing;


	/// <summary>
	/// Handles Umbraco events.
	/// </summary>
	public class RhythmEventHandler : ApplicationEventHandler
	{

		/// <summary>
		/// Handles application startup.
		/// </summary>
		protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication,
			ApplicationContext applicationContext)
		{

			// Custom model binder for some special behaviors.
			ModelBinders.Binders.DefaultBinder = new RhythmModelBinder();

			// URL provider to facilitate linking to fragment identifiers.
			UrlProviderResolver.Current.InsertTypeBefore<DefaultUrlProvider, FragmentUrlProvider>();

		}

	}

}