namespace Rhythm.Extensions.Events
{

	// Namespaces.
	using Binding;
	using System.Web.Mvc;
	using Umbraco.Core;


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

		}

	}

}