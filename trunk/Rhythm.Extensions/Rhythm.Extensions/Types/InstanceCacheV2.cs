namespace Rhythm.Extensions.Types {

	// Namespaces.
	using Enums;
	using System;
	using System.Threading;

	/// <summary>
	/// Caches and instance variable.
	/// </summary>
	/// <typeparam name="T">The type of variable to cache.</typeparam>
    /// <remarks>
    /// Compared to the original InstanceCache, this version moves the retrieval parameters into
    /// the constructor, so that the class can have more awareness of its state.
    /// </remarks>
	public class InstanceCacheV2<T> {

		#region Properties

		private DateTime? LastCache { get; set; }
		private T Instance { get; set; }
		private object InstanceLock { get; set; }

        private TimeSpan Duration { get; set; }
        private Func<T> Replenisher { get; set; }
		private CacheGetMethod Method { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public InstanceCacheV2(
            TimeSpan duration,
            Func<T> replenisher,
            CacheGetMethod method = CacheGetMethod.Default
        ) {
			InstanceLock = new object();
			LastCache = null;

            Duration = duration;
            Replenisher = replenisher;
            Method = method;
		}

        #endregion

        #region Methods

        public bool HasValue {
            get { return LastCache.HasValue; }
        }

        public bool IsExpired {
            get { return LastCache.HasValue && (DateTime.Now - LastCache.Value) > Duration; }
        }

		/// <summary>
		/// Gets the instance variable (either from the cache or from the specified function).
		/// </summary>
		/// <param name="duration">The duration to cache for.</param>
		/// <param name="replenisher">The function that replenishes the cache.</param>
		/// <param name="method">Optional. The cache method to use when retrieving the value.</returns>
		public T Get() {
			if (Method == CacheGetMethod.FromCache) {
				lock (InstanceLock) {
					return LastCache.HasValue ? Instance : default(T);
				}
			} else if (Method == CacheGetMethod.NoCache) {
				return Replenisher();
			} else {
				lock (InstanceLock) {
					var now = DateTime.Now;
					if (Method == CacheGetMethod.Recache) {
						Recache(Replenisher);
					}
					else if (!LastCache.HasValue || now.Subtract(LastCache.Value) >= Duration) {
						if (Method == CacheGetMethod.NoStore) {
							return Replenisher();
						} else if (Method == CacheGetMethod.DefaultRecacheAfter && LastCache.HasValue) {
							DeferredRecache(Replenisher);
						} else {
							Recache(Replenisher);
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