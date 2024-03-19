using TimeZoneConverter;

namespace TransactionManagement.Helpers
{
    /// <summary>
    /// Provides methods to convert between IANA and Windows time zone identifiers.
    /// </summary>
    public static class TimeZoneHelper
    {
        /// <summary>
        /// Converts the specified IANA time zone identifier to the corresponding Windows time zone identifier.
        /// </summary>
        /// <param name="timeZoneId">The IANA time zone identifier to convert.</param>
        /// <returns>The corresponding Windows time zone identifier.</returns>
        public static string ConvertIanaToWindows(string timeZoneId)
        {
            string standardTimeZoneId;

            try
            {
                standardTimeZoneId = TZConvert.IanaToWindows(timeZoneId);
            }
            catch (InvalidTimeZoneException)
            {
                // Handle the case when the specified time zone is not available in Windows time zones.
                // In this scenario, we handle the specific time zone "Troll Time" (Antarctica/Troll) which is not available in Windows time zones.
                // Troll Time is UTC +0 and experiences daylight saving time adjustments, transitioning to Central European Summer Time (CEST) on March 31.
                // From March 31, Troll Time will be UTC +2 (CEST) during daylight saving time.
                // When Troll Time is in standard time, it is UTC +0, and when it is in daylight saving time, it is the same as the time in Amsterdam.
                // Troll Time uses the IANA time zone identifier "Antarctica/Troll".

                DateTime currentUtcTime = DateTime.UtcNow;

                DateTime summerTimeTransition = new DateTime(currentUtcTime.Year, 3, 31, 1, 0, 0, DateTimeKind.Utc);
                DateTime standardTimeTransition = new DateTime(currentUtcTime.Year, 10, 29, 3, 0, 0, DateTimeKind.Utc);

                if (currentUtcTime >= summerTimeTransition && currentUtcTime < standardTimeTransition)
                {
                    // From March 31 Antarctica/Troll Time will be UTC+2 (CEST)
                    standardTimeZoneId = "Central European Summer Time";
                }
                else
                {
                    // Otherwise, Antarctica/Troll Time is in standard time UTC+0
                    standardTimeZoneId = "UTC";
                }
            }

            return standardTimeZoneId;
        }
    }
}
