namespace Rhythm.Extensions.Types {

	// Namespaces.
	using Enums;
	using System;

	/// <summary>
	/// Caches and instance variable.
	/// </summary>
	/// <typeparam name="T">The type of variable to cache.</typeparam>
	public class InstanceCache<T> {

		#region Properties

		private DateTime? LastCache { get; set; }
		private T Instance { get; set; }
		private object InstanceLock { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public InstanceCache() {
			InstanceLock = new object();
			LastCache = null;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the instance variable (either from the cache or from the specified function).
		/// </summary>
		/// <param name="duration">The duration to cache for.</param>
		/// <param name="replenisher">The function that replenishes the cache.</param>
		/// <param name="method">Optional. The cache method to use when retrieving the value.</returns>
		public T Get(TimeSpan duration, Func<T> replenisher,
			CacheGetMethod method = CacheGetMethod.Default) {
				if (method == CacheGetMethod.NoCache) {
					return replenisher();
				} else {
					lock (InstanceLock) {
						var now = DateTime.Now;
						if (method == CacheGetMethod.Recache) {
							Instance = replenisher();
							LastCache = now;
						}
						else if (!LastCache.HasValue || now.Subtract(LastCache.Value) >= duration) {
							if (method == CacheGetMethod.NoStore) {
								return replenisher();
							} else {
								Instance = replenisher();
								LastCache = now;
							}
						}
						return Instance;
					}
				}
		}

		#endregion

	}

}