using Rhythm.Extensions.Events;
using Rhythm.Extensions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Rhythm.Extensions.Types {

	/// <summary>
	/// Invalidates an InstanceCache when content of particular content types is changed.
	/// </summary>
	/// <typeparam name="T">The type of data being cached.</typeparam>
	public class CacheInvalidator<T> : ICacheInvalidator {

		#region Properties

		/// <summary>
		/// The aliases that will cause cache invalidation.
		/// </summary>
		private IEnumerable<string> Aliases { get; set; }

		/// <summary>
		/// The instance cache to invalidate.
		/// </summary>
		private InstanceCache<T> InstanceCache { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="cache">The instance cache to invalidate.</param>
		/// <param name="aliases">
		/// The aliases of the content types to monitor for changes.
		/// If none are specified, all content types are monitored.
		/// </param>
		public CacheInvalidator(InstanceCache<T> cache, params string[] aliases) {
			this.Aliases = (aliases ?? new string[] {}).ToList();
			this.InstanceCache = cache;
			RhythmEventHandler.RegisterInvalidator(this);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Invalidates the cache if the specified aliases match the monitored aliases.
		/// </summary>
		/// <param name="aliases">The content type aliases of changed content.</param>
		public void InvalidateForAliases(IEnumerable<string> aliases) {
			var ignoreCase = StringComparer.InvariantCultureIgnoreCase;
			if (!Aliases.Any() || Aliases.Intersect(aliases, ignoreCase).Any()) {
				this.InstanceCache.Clear();
			}
		}

		/// <summary>
		/// Invalidates the instance cache unconditionally.
		/// </summary>
		public void Invalidate() {
			this.InstanceCache.Clear();
		}

		#endregion

	}

}