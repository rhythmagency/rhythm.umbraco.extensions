using System.Collections.Generic;
namespace Rhythm.Extensions.Interfaces {

	/// <summary>
	/// Interface for cache invalidators.
	/// </summary>
	public interface ICacheInvalidator {

		/// <summary>
		/// Invalidates if the invalidator matches any of the specified doctype aliases.
		/// </summary>
		/// <param name="aliases">The doctype aliases to invalidate for.</param>
		void InvalidateForAliases(IEnumerable<string> aliases);


		/// <summary>
		/// Invalidates the cache unconditionally.
		/// </summary>
		void Invalidate();

	}

}