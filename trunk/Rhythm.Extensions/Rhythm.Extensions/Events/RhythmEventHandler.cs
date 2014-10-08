using Rhythm.Extensions.Binding;
using Rhythm.Extensions.Interfaces;
using Rhythm.Extensions.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;
using Umbraco.Web.Routing;
namespace Rhythm.Extensions.Events {

	/// <summary>
	/// Handles Umbraco events.
	/// </summary>
	public partial class RhythmEventHandler : ApplicationEventHandler {

		#region Static Members

		private static object InvalidatorsLock { get; set; }
		private static List<WeakReference<ICacheInvalidator>> Invalidators { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Static constructor.
		/// </summary>
		static RhythmEventHandler() {
			InvalidatorsLock = new object();
			Invalidators = new List<WeakReference<ICacheInvalidator>>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Registers a cache invalidator so that it can be notified when change events occur.
		/// </summary>
		/// <param name="invalidator">The invalidator.</param>
		public static void RegisterInvalidator(ICacheInvalidator invalidator) {
			if (invalidator != null) {
				lock (InvalidatorsLock) {
					Invalidators.Add(new WeakReference<ICacheInvalidator>(invalidator));
				}
			}
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Handles application startup.
		/// </summary>
		protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication,
			ApplicationContext applicationContext) {

			// Custom model binder for some special behaviors.
			ModelBinders.Binders.DefaultBinder = new RhythmModelBinder();

			// URL provider to facilitate linking to fragment identifiers.
			UrlProviderResolver.Current.InsertTypeBefore<DefaultUrlProvider, FragmentUrlProvider>();

			// Listen for content change events.
			ContentService.Moved += ContentService_Moved;
			ContentService.Published += ContentService_Published;
			ContentService.Deleted += ContentService_Deleted;

		}

		/// <summary>
		/// Some content was deleted.
		/// </summary>
		void ContentService_Deleted(IContentService sender, DeleteEventArgs<IContent> e) {
			var aliases = e.DeletedEntities.Select(x => x.ContentType.Alias).Distinct().ToList();
			HandleChangedContent(aliases);
		}

		/// <summary>
		/// Some content was published.
		/// </summary>
		void ContentService_Published(IPublishingStrategy sender, PublishEventArgs<IContent> e) {
			var aliases = e.PublishedEntities.Select(x => x.ContentType.Alias).Distinct().ToList();
			HandleChangedContent(aliases);
		}

		/// <summary>
		/// Some content was moved.
		/// </summary>
		void ContentService_Moved(IContentService sender, MoveEventArgs<IContent> e) {
			Specialized_ContentService_Moved(sender, e);
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Handles changed content.
		/// </summary>
		/// <param name="aliases">The aliases of the content types that were changed.</param>
		private void HandleChangedContent(IEnumerable<string> aliases) {

			// Variables.
			var foundInvalidators = new List<ICacheInvalidator>();

			// Find invalidators that are still in memory.
			lock (InvalidatorsLock) {
				for (var i = 0; i < Invalidators.Count; i++) {
					var weakInvalidator = Invalidators[i];
					if (weakInvalidator != null) {
						var invalidator = default(ICacheInvalidator);
						if (weakInvalidator.TryGetTarget(out invalidator)) {
							foundInvalidators.Add(invalidator);
						} else {
							Invalidators[i] = null;
						}
					}
				}
				Invalidators = Invalidators.Where(x => x != null).ToList();
			}

			// Call invalidators for the changed aliases.
			foreach (var invalidator in foundInvalidators) {
				invalidator.InvalidateForAliases(aliases);
			}
		}

		#endregion

	}

}