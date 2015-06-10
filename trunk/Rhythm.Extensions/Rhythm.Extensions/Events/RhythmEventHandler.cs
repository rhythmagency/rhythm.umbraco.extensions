using Rhythm.Extensions.Binding;
using Rhythm.Extensions.Enums;
using Rhythm.Extensions.Interfaces;
using Rhythm.Extensions.Routing;
using Rhythm.Extensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;
using Umbraco.Core.Sync;
using Umbraco.Web.Cache;
using Umbraco.Web.Routing;
namespace Rhythm.Extensions.Events {

	/// <summary>
	/// Handles Umbraco events.
	/// </summary>
	public partial class RhythmEventHandler : ApplicationEventHandler
	{

		#region Constants

		private const string UpdatingCacheAlias = "Updating cache for document type alias, {0}.";
		private const string UpdatingCacheAll = "Updating all caches.";
		private const string ScanningDeleted = "Scanning the recycle bin for deleted nodes. This might be an computationally expensive operation.";

		#endregion

		#region Static Members

		private static object InvalidatorsLock { get; set; }
		private static List<WeakReference<ICacheInvalidator>> Invalidators { get; set; }
		private static object KeyInvalidatorsLock { get; set; }
		private static List<WeakReference<ICacheByKeyInvalidator>> KeyInvalidators { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Static constructor.
		/// </summary>
		static RhythmEventHandler() {
			InvalidatorsLock = new object();
			Invalidators = new List<WeakReference<ICacheInvalidator>>();
			KeyInvalidatorsLock = new object();
			KeyInvalidators = new List<WeakReference<ICacheByKeyInvalidator>>();
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

		/// <summary>
		/// Registers a cache by key invalidator so that it can be notified when change vents occur.
		/// </summary>
		/// <param name="invalidator">The invalidator.</param>
		public static void RegisterInvalidator(ICacheByKeyInvalidator invalidator) {
			if (invalidator != null) {
				lock (KeyInvalidatorsLock) {
					KeyInvalidators.Add(new WeakReference<ICacheByKeyInvalidator>(invalidator));
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
			var customProvider = ConfigUtility.GetString(ConfigKeys.CustomUrlProviderName);
			if ("FragmentUrlProvider".InvariantEquals(customProvider)) {
				try {
					UrlProviderResolver.Current.InsertTypeBefore<DefaultUrlProvider, FragmentUrlProvider>();
					UrlProviderResolver.Current.RemoveType<DefaultUrlProvider>();
				} catch { }
			}

			// Listen for content change events.
			ContentService.Moved += ContentService_Moved;
			ContentService.Published += ContentService_Published;
			ContentService.Deleted += ContentService_Deleted;
			PageCacheRefresher.CacheUpdated += PageCacheRefresher_CacheUpdated;

			// Listen for media change events.
			MediaService.Moved += MediaService_Moved;
			MediaService.Saved += MediaService_Saved;
			MediaService.Deleted += MediaService_Deleted;

		}

		/// <summary>
		/// Page cache was updated.
		/// </summary>
		void PageCacheRefresher_CacheUpdated(PageCacheRefresher sender, CacheRefresherEventArgs e) {
			var kind = e.MessageType;
			if (kind == MessageType.RefreshById || kind == MessageType.RemoveById) {

				// Attempt to update caches by document type alias.
				var id = e.MessageObject as int?;
				if (id.HasValue) {
					var contentService = ApplicationContext.Current.Services.ContentService;
					var node = contentService.GetById(id.Value);
					if (node == null) {

						// If the node doesn't exist in the content tree, check the recycle bin.
						LogHelper.Info<RhythmEventHandler>(ScanningDeleted);
						var recycled = contentService.GetContentInRecycleBin();
						foreach (var recycledNode in recycled) {
							if (recycledNode != null) {
								var recycledId = recycledNode.Id;
								if (recycledId == id.Value) {
									node = recycledNode;
									break;
								}
							}
						}

					}
					if (node != null) {
						var alias = node.ContentType.Alias;
						if (!string.IsNullOrWhiteSpace(alias)) {
							LogHelper.Info<RhythmEventHandler>(UpdatingCacheAlias, () => alias);
							HandleChangedContent(new[] { alias });
						}
					}
				}

			} else if (kind == MessageType.RefreshAll) {

				// Update all caches.
				LogHelper.Info<RhythmEventHandler>(UpdatingCacheAll);
				HandleChangedContent(null);

			}
		}

		/// <summary>
		/// Some media was deleted.
		/// </summary>
		void MediaService_Deleted(IMediaService sender, DeleteEventArgs<IMedia> e) {
			var ids = e.DeletedEntities.Select(x => x.Id).ToList();
			HandleChangedMedia(ids);
		}

		/// <summary>
		/// Some media was saved.
		/// </summary>
		void MediaService_Saved(IMediaService sender, SaveEventArgs<IMedia> e) {
			var ids = e.SavedEntities.Select(x => x.Id).ToList();
			HandleChangedMedia(ids);
		}

		/// <summary>
		/// Some media was moved.
		/// </summary>
		void MediaService_Moved(IMediaService sender, MoveEventArgs<IMedia> e) {
			Specialized_MediaService_Moved(sender, e);
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
		/// <param name="aliases">
		/// The aliases of the content types that were changed.
		/// If null, all aliases will be assumed.
		/// </param>
		private void HandleChangedContent(IEnumerable<string> aliases) {

			// Variables.
			var foundInvalidators = new List<ICacheInvalidator>();

			// Find invalidators that are still in memory.
			lock (InvalidatorsLock) {
				var invalidators = Invalidators;
				foundInvalidators = GetLiveValues(ref invalidators);
				Invalidators = invalidators;
			}

			// Call invalidators for the changed aliases (or unconditionally).
			foreach (var invalidator in foundInvalidators) {
				if (aliases == null) {

					// Invalidate the cache regardless of alias.
					invalidator.Invalidate();

				} else {

					// Invalidate the cache only for the specified alises.
					invalidator.InvalidateForAliases(aliases);

				}
			}

		}

		/// <summary>
		/// Handles changed media.
		/// </summary>
		/// <param name="ids">The ID's of the media items that were changed.</param>
		private void HandleChangedMedia(IEnumerable<int> ids) {

			// Variables.
			var foundInvalidators = new List<ICacheByKeyInvalidator>();

			// Find invalidators that are still in memory.
			lock (KeyInvalidatorsLock) {
				var invalidators = KeyInvalidators;
				foundInvalidators = GetLiveValues(ref invalidators);
				KeyInvalidators = invalidators;
			}

			// Call invalidators for the changed ID's.
			foreach (var invalidator in foundInvalidators) {
				invalidator.InvalidateForIds(ids);
			}

		}

		/// <summary>
		/// Gets the values in a list of weak references that aren't stale yet.
		/// </summary>
		/// <typeparam name="T">The type of item stored by each weak reference.</typeparam>
		/// <param name="items">The list of weak references.</param>
		/// <returns>The list of values that aren't stale.</returns>
		/// <remarks>
		/// The supplied item list will have stale references removed.
		/// </remarks>
		private List<T> GetLiveValues<T>(ref List<WeakReference<T>> items) where T : class
		{

			// Variables.
			var foundItems = new List<T>();
			var newItems = new List<WeakReference<T>>();

			// Find invalidators that are still in memory.
			for (var i = 0; i < items.Count; i++) {
				var weakItem = items[i];
				if (weakItem != null) {
					var item = default(T);
					if (weakItem.TryGetTarget(out item)) {
						newItems.Add(weakItem);
						foundItems.Add(item);
					}
				}
			}

			// Remove stale references.
			items = newItems;

			// Return found items.
			return foundItems;

		}

		#endregion

	}

}