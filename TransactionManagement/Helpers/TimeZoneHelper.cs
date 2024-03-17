using TimeZoneConverter;

namespace TransactionManagement.Helpers
{
    public static class TimeZoneHelper
    {
        public static string ConvertIanaToWindows(string timeZoneId)
        {
            string standardTimeZoneId;

            try
            {
                standardTimeZoneId = TZConvert.IanaToWindows(timeZoneId);
            }
            catch (InvalidTimeZoneException)
            {
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
