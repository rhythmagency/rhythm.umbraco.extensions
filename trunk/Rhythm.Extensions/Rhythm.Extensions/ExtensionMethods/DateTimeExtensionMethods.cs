using System;

namespace Rhythm.Extensions.ExtensionMethods {
	public static class DateTimeExtensionMethods {
		/// <summary>
		/// Converts a DateTime instance to a GMT string.
		/// </summary>
		/// <param name="date">The DateTime instance to convert.</param>
		/// <returns>The DateTime instance as a GMT string.</returns>
		public static string ToGmt(this DateTime date) {
			return String.Format("{0} GMT", date.ToString("ddd, dd MMM yyyy hh:mm:ss"));
		}

		/// <summary>
		/// Returns the number of months between two dates.
		/// </summary>
		/// <param name="start">The starting date.</param>
		/// <param name="end">The end date.</param>
		/// <returns>Returns the number of months between two dates.</returns>
		public static int TotalMonths(this DateTime start, DateTime end) {
			return (start.Year * 12 + start.Month) - (end.Year * 12 + end.Month);
		}
	}
}