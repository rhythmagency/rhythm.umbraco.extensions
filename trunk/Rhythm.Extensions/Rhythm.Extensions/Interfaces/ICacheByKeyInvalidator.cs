using System.Collections.Generic;
namespace Rhythm.Extensions.Interfaces {

	/// <summary>
	/// Interface for cache by key invalidators.
	/// </summary>
	public interface ICacheByKeyInvalidator {
		void InvalidateForIds(IEnumerable<int> ids);
	}

}