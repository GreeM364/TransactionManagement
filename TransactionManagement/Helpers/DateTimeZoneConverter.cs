using TimeZoneConverter;

namespace TransactionManagement.Helpers
{
    /// <summary>
    /// Provides methods to convert between UTC and local time in different time zones.
    /// </summary>
    public static class DateTimeZoneConverter
    {
        /// <summary>
        /// Converts the specified local date and time to Coordinated Universal Time (UTC) based on the provided time zone.
        /// </summary>
        /// <param name="localDateTime">The local date and time to convert.</param>
        /// <param name="timezoneId">The identifier of the target time zone.</param>
        /// <returns>The equivalent UTC date and time.</returns>
        public static DateTime ConvertToUtc(DateTime localDateTime, string timezoneId)
        {
            TimeZoneInfo timezone = TZConvert.GetTimeZoneInfo(timezoneId);

            return TimeZoneInfo.ConvertTimeToUtc(localDateTime, timezone);
        }

        /// <summary>
        /// Converts the specified UTC date and time to the local time based on the provided time zone.
        /// </summary>
        /// <param name="utcDateTime">The UTC date and time to convert.</param>
        /// <param name="timezoneId">The identifier of the target time zone.</param>
        /// <returns>The equivalent local date and time.</returns>
        public static DateTime ConvertToLocal(DateTime utcDateTime, string timezoneId)
        {
            TimeZoneInfo timezone = TZConvert.GetTimeZoneInfo(timezoneId);

            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timezone);
        }
    }
}
