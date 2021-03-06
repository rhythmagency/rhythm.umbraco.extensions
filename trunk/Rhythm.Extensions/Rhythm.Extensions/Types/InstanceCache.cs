﻿namespace Rhythm.Extensions.Types {

	// Namespaces.
	using Enums;
	using System;
	using System.Threading;

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
		public T Get(
			TimeSpan duration,
			Func<T> replenisher,
			CacheGetMethod method = CacheGetMethod.Default
		) {
			if (method == CacheGetMethod.FromCache) {
				lock (InstanceLock) {
					return LastCache.HasValue ? Instance : default(T);
				}
			} else if (method == CacheGetMethod.NoCache) {
				return replenisher();
			} else {
				lock (InstanceLock) {
					var now = DateTime.Now;
					if (method == CacheGetMethod.Recache) {
						Recache(replenisher);
					}
					else if (!LastCache.HasValue || now.Subtract(LastCache.Value) >= duration) {
						if (method == CacheGetMethod.NoStore) {
							return replenisher();
						} else if (method == CacheGetMethod.DefaultRecacheAfter && LastCache.HasValue) {
							DeferredRecache(replenisher);
						} else {
							Recache(replenisher);
						}
					}
					return Instance;
				}
			}
		}

		private void Recache(Func<T> replenisher) {
			Instance = replenisher();
			LastCache = DateTime.Now;
		}

		private void DeferredRecache(Func<T> replenisher) {
			var threadStart = new ThreadStart(() => {
				lock (InstanceLock) {
					Recache(replenisher);
				}
			});
			var thread = new Thread(threadStart);
			thread.Start();
		}

		/// <summary>
		/// Clears the cache.
		/// </summary>
		public void Clear() {
			lock (InstanceLock) {
				LastCache = null;
			}
		}

		#endregion

	}

}