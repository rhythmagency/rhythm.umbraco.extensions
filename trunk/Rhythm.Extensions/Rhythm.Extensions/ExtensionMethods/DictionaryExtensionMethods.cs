using System.Collections.Generic;

namespace Rhythm.Extensions.ExtensionMethods {

	/// <summary>
	/// Extension methods for Dictionary.
	/// </summary>
	public static class DictionaryExtensionMethods {

		#region Extension Methods

		/// <summary>
		/// Tries to get the values using the specified keys.
		/// </summary>
		/// <typeparam name="TKey">They type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="source">The dictionary.</param>
		/// <param name="keys">The keys to look up.</param>
		/// <returns>The values.</returns>
		public static IEnumerable<TValue> TryGetValues<TKey, TValue>(this Dictionary<TKey, TValue> source,
			IEnumerable<TKey> keys) {
			foreach (var key in keys) {
				TValue value;
				if (source.TryGetValue(key, out value)) {
					yield return value;
				}
			}
		}


		/// <summary>
		/// Tries to get the values using the specified keys.
		/// </summary>
		/// <typeparam name="TKey">They type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="source">The dictionary.</param>
		/// <param name="keys">The keys to look up.</param>
		/// <returns>The values.</returns>
		public static IEnumerable<TValue> TryGetValues<TKey, TValue>(this Dictionary<TKey, TValue> source,
			params TKey[] keys) {
			return TryGetValues(source, keys as IEnumerable<TKey>);
		}

		#endregion

	}

}