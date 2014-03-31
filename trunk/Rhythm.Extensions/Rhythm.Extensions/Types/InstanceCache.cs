namespace Rhythm.Extensions.Types {

    // Namespaces.
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
        /// <returns>The instance variable.</returns>
        public T Get(TimeSpan duration, Func<T> replenisher) {
            lock (InstanceLock) {
                var now = DateTime.Now;
                if (!LastCache.HasValue || now.Subtract(LastCache.Value) >= duration) {
                    Instance = replenisher();
                    LastCache = now;
                }
                return Instance;
            }
        }

        #endregion

    }

}