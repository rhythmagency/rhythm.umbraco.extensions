namespace Rhythm.Extensions.Enums
{

	/// <summary>
	/// The methods that can be used when retrieving values from a cache.
	/// </summary>
	public enum CacheGetMethod
	{

		/// <summary>
		/// Gets a value from the cache, replenishing the cache with a new value
		/// if necessary.
		/// </summary>
		Default,

		/// <summary>
		/// Gets a value from the cache, or gets a new value, but will not store
		/// the new value to the cache.
		/// </summary>
		NoStore,

		/// <summary>
		/// Gets a new value each time and does not store it to the cache.
		/// </summary>
		NoCache,

		/// <summary>
		/// Gets a new value and stores the result to the cache.
		/// </summary>
		Recache,

		/// <summary>
		/// Gets the cached value, or null.
		/// Does not modify the cached value or call the replenisher.
		/// </summary>
		FromCache

	}

}