using Rhythm.Extensions.Events;
using Rhythm.Extensions.Interfaces;
using System.Collections.Generic;
namespace Rhythm.Extensions.Types {

	/// <summary>
	/// Invalidates an InstanceByKeyCache when media of particular ID's are changed.
	/// </summary>
	/// <typeparam name="T">The type of data being cached.</typeparam>
	public class MediaCacheInvalidator<T> : ICacheByKeyInvalidator {

		#region Properties

		/// <summary>
		/// The instance cache to invalidate.
		/// </summary>
		private InstanceByKeyCache<T, int> InstanceCache { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="cache">The instance cache to invalidate.</param>
		public MediaCacheInvalidator(InstanceByKeyCache<T, int> cache) {
			this.InstanceCache = cache;
			RhythmEventHandler.RegisterInvalidator(this);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Invalidates the cache for the specified ID's.
		/// </summary>
		/// <param name="ids">The ID's to invalidate the cache for.</param>
		public void InvalidateForIds(IEnumerable<int> ids) {
			this.InstanceCache.ClearKeys(ids);
		}

		#endregion

	}

}