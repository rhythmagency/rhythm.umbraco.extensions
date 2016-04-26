namespace Rhythm.Extensions.Utilities {

	/// <summary>
	/// Utility to assist with parsing.
	/// </summary>
	public class ParseUtility {

		#region Methods

		/// <summary>
		/// Attempts to parse an integer.
		/// </summary>
		/// <param name="str">
		/// The string to parse.
		/// </param>
		/// <returns>
		/// The parsed integer, or null.
		/// </returns>
		public static int? AttemptParseInt(string str) {
			var value = default(int);
			if (int.TryParse(str, out value)) {
				return value;
			}
			else {
				return null;
			}
		}

		#endregion

	}

}