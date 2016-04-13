namespace Rhythm.Extensions.Types
{

    // Namespaces.
    using Enums;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Caches a collection of instance variables by key (similar to a dictionary).
    /// </summary>
    /// <typeparam name="T">The type of variable to cache.</typeparam>
    /// <typeparam name="TKey">The type of key to cache by.</typeparam>
    public class InstanceByKeyCacheEx<T, TKey>
    {
        #region Properties

        private Dictionary<TKey, Tuple<T, DateTime>> Instances { get; set; }
        private object InstancesLock { get; set; }
        private TimeSpan TimeToLive { get; set; }
        private Func<TKey, T> Replenisher { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public InstanceByKeyCacheEx()
        {
            InstancesLock = new object();
            Instances = new Dictionary<TKey, Tuple<T, DateTime>>();
            TimeToLive = new TimeSpan(1,0,0); // default cache ttl to be 1 hour
            var methodInfo = GetType().GetProperty("Prop").GetGetMethod();
        }

        /// <summary>
        /// Constructor object with TimeToLive to define an item duration
        /// </summary>
        /// <param name="timeToLive"></param>
        /// <param name="replenisher"></param>
        public InstanceByKeyCacheEx(TimeSpan timeToLive, Func<TKey, T> replenisher)
        {
            InstancesLock = new object();
            Instances = new Dictionary<TKey, Tuple<T, DateTime>>();
            TimeToLive = timeToLive;
            Replenisher = replenisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Using TimeToLive, determine if the instance should be fetched
        /// </summary>
        /// <returns></returns>
        public bool IsCacheExpired(Tuple<T, DateTime> instance)
        {
            return instance.Item2.Add(TimeToLive) < DateTime.Now;
        }

        /// <summary>
        /// Gets the instance variable (either from the cache or from the specified function).
        /// </summary>
        /// <param name="key">The key to use when fetching the variable.</param>
        /// <param name="replenisher">The function that replenishes the cache.</param>
        /// <param name="method">Optional. The cache method to use when retrieving the value.</param>
        public T Get(TKey key, CacheGetMethod method = CacheGetMethod.Default)
        {

            var now = DateTime.Now;
            var tempInstance = default(T);
            var tempTuple = default(Tuple<T, DateTime>);

            lock (InstancesLock)

                switch (method)
                {
                    case CacheGetMethod.NoCache:
                        return Replenisher(key);

                    case CacheGetMethod.FromCache:
                        // Attempt to get instance from cache without replenishing.
                        if (Instances.TryGetValue(key, out tempTuple))
                        {
                            return tempTuple.Item1;
                        }
                        else
                        {
                            return default(T);
                        }

                    case CacheGetMethod.Recache:
                        tempInstance = Replenisher(key);
                        Instances[key] = new Tuple<T, DateTime>(tempInstance, now);
                        return tempInstance;

                    case CacheGetMethod.Default:
                    case CacheGetMethod.NoStore:
                        if (Instances.TryGetValue(key, out tempTuple) && !IsCacheExpired(tempTuple))
                        {
                            // Cache still valid. Use cached value.
                            return tempTuple.Item1;
                        }
                        else
                        {
                            tempInstance = Replenisher(key);
                            // Cache expired. Get a new value without modifying the cache.
                            if (method == CacheGetMethod.Default)
                            {
                                Instances[key] = new Tuple<T, DateTime>(tempInstance, now);
                            }
                            return tempInstance;
                        }

                    default:
                        return default(T);
                }
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void Clear()
        {
            lock (InstancesLock)
            {
                Instances.Clear();
            }
        }

        /// <summary>
        /// Clears the cache of the specified keys.
        /// </summary>
        /// <param name="keys">The keys to clear the cache of.</param>
        public void ClearKeys(IEnumerable<TKey> keys)
        {
            lock (InstancesLock)
            {
                foreach (var key in keys)
                {
                    Instances.Remove(key);
                }
            }
        }

        #endregion
    }
}