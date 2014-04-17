namespace Rhythm.Extensions.ExtensionMethods
{
	using System;
	public static class TimeZoneExtensions {

		/// <summary>
		/// Returns a friendly name for the specified timezone.
		/// </summary>
		/// <param name="zone">The time zone.</param>
		/// <param name="referenceDate">A date (to determine whether or not to return the daylight name).</param>
		/// <returns>The friendly name (e.g., "Pacific Standard Time").</returns>
		public static string ToFriendlyName(this TimeZone zone, DateTime referenceDate) {
			return zone.IsDaylightSavingTime(referenceDate) ? zone.DaylightName : zone.StandardName;
		}

	}
}