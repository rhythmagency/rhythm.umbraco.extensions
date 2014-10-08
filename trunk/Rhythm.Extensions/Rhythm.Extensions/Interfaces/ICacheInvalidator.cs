using System.Collections.Generic;
namespace Rhythm.Extensions.Interfaces {

	/// <summary>
	/// Interface for cache invalidators.
	/// </summary>
	public interface ICacheInvalidator {
		void InvalidateForAliases(IEnumerable<string> aliases);
	}

}