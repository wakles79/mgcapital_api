using System;
using TimeZoneConverter;

namespace MGCap.Domain.Utils
{
    public static class DateTimeExtensions
    {
        public static int ToEpoch(this DateTime dateTime)
        {
            if (dateTime.Year < 1970)
            {
                return 0;
            }

            TimeSpan ts = dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (int)ts.TotalSeconds;
        }

        public static DateTime FromEpoch(this int epochTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(epochTime);
        }

        /// <summary>
        /// Adjusts the date time (if applicable) given an offset.
        /// </summary>
        /// <returns>The date time.</returns>
        /// <param name="epoch">Unix timestamp</param>
        /// <param name="timezoneOffset">Timezone offset.</param>
        /// <param name="timezoneId">Timezone identifer.</param>
        public static DateTime AdjustDateTime(this int epoch, int timezoneOffset, string timezoneId)
        {
            if (epoch <= 0)
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }

            epoch -= timezoneOffset * 60;
            var dt = epoch.FromEpoch();
            // var ts = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            var ts = TZConvert.GetTimeZoneInfo(timezoneId);
            if (!ts.IsDaylightSavingTime(dt))
            {
                dt = dt.AddHours(-1);
            }
            return dt;
        }
    }
}
