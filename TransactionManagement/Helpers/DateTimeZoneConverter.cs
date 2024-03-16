using TimeZoneConverter;

namespace TransactionManagement.Helpers
{
    public static class DateTimeZoneConverter
    {
        public static DateTime ConvertToUtc(DateTime localDateTime, string timezoneId)
        {
            TimeZoneInfo timezone = TZConvert.GetTimeZoneInfo(timezoneId);

            return TimeZoneInfo.ConvertTimeToUtc(localDateTime, timezone);
        }

        public static DateTime ConvertToLocal(DateTime utcDateTime, string timezoneId)
        {
            TimeZoneInfo timezone = TZConvert.GetTimeZoneInfo(timezoneId);

            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timezone);
        }
    }


}
