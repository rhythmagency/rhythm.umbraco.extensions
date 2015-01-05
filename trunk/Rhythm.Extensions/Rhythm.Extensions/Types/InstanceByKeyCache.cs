namespace Rhythm.Extensions.Types {

	// Namespaces.
	using Enums;
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Caches a collection of instance variables by key (similar to a dictionary).
	/// </summary>
	/// <typeparam name="T">The type of variable to cache.</typeparam>
	/// <typeparam name="TKey">The type of key to cache by.</typeparam>
	public class InstanceByKeyCache<T, TKey> {

		#region Properties

		private Dictionary<TKey, Tuple<T, DateTime>> Instances { get; set; }
		private object InstancesLock { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public InstanceByKeyCache() {
			InstancesLock = new object();
			Instances = new Dictionary<TKey, Tuple<T, DateTime>>();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the instance variable (either from the cache or from the specified function).
		/// </summary>
		/// <param name="key">The key to use when fetching the variable.</param>
		/// <param name="replenisher">The function that replenishes the cache.</param>
		/// <param name="duration">The duration to cache for.</param>
		/// <param name="method">Optional. The cache method to use when retrieving the value.</returns>
		public T Get(TKey key, Func<TKey, T> replenisher, TimeSpan duration,
			CacheGetMethod method = CacheGetMethod.Default) {
			if (method == CacheGetMethod.FromCache) {

				// Attempt to get instance from cache without replenishing.
				lock (InstancesLock) {
					var tempTuple = default(Tuple<T, DateTime>);
					if (Instances.TryGetValue(key, out tempTuple)) {
						return tempTuple.Item1;
					} else {
						return default(T);
					}
				}

			} else if (method == CacheGetMethod.NoCache) {

				// Get a new instance without caching it.
				return replenisher(key);

			} else {
				lock (InstancesLock) {
					var tempInstance = default(T);
					var now = DateTime.Now;
					if (method == CacheGetMethod.Recache) {

						// Force the cache to replenish.
						tempInstance = replenisher(key);
						Instances[key] = new Tuple<T,DateTime>(tempInstance, now);

					} else {

						// Value already cached?
						var tempTuple = default(Tuple<T, DateTime>);
						if (Instances.TryGetValue(key, out tempTuple)) {
							if (now.Subtract(Instances[key].Item2) >= duration) {
								if (method == CacheGetMethod.NoStore) {

									// Cache expired. Get a new value without modifying the cache.
									tempInstance = replenisher(key);

								} else {

									// Cache expired. Replenish the cache.
									tempInstance = replenisher(key);
									Instances[key] = new Tuple<T, DateTime>(tempInstance, now);

								}
							} else {

								// Cache still valid. Use cached value.
								tempInstance = tempTuple.Item1;

							}
						} else {
							if (method == CacheGetMethod.NoStore) {

								// No cached value. Get a new value without modifying the cache.
								tempInstance = replenisher(key);

							} else {

								// No cached value. Replenish the cache.
								tempInstance = replenisher(key);
								Instances[key] = new Tuple<T, DateTime>(tempInstance, now);

							}
						}

					}
					return tempInstance;
				}
			}
		}

		/// <summary>
		/// Clears the cache.
		/// </summary>
		public void Clear() {
			lock (InstancesLock) {
				Instances.Clear();
			}
		}

		/// <summary>
		/// Clears the cache of the specified keys.
		/// </summary>
		/// <param name="keys">The keys to clear the cache of.</param>
		public void ClearKeys(IEnumerable<TKey> keys) {
			lock (InstancesLock) {
				foreach (var key in keys) {
					Instances.Remove(key);
				}
			}
		}

		#endregion

	}

}