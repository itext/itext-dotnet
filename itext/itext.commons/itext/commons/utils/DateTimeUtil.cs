/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
    Authors: Apryse Software.

    This program is offered under a commercial and under the AGPL license.
    For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

    AGPL licensing:
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
using System;
using System.Globalization;

namespace iText.Commons.Utils {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </summary>
    public static class DateTimeUtil {
        private static readonly String DEFAULT_PATTERN = "yyyy-MM-dd";
        
        /// <summary>
        /// Gets the date time as UTC milliseconds from the epoch.
        /// </summary>
        /// <param name="dateTime">date to be converted to millis</param>
        /// <returns>the date as UTC milliseconds from the epoch</returns>
        public static double GetUtcMillisFromEpoch(DateTime? dateTime) {
            if (dateTime == null) {
                dateTime = GetCurrentUtcTime();
            }
            return (dateTime.Value.ToUniversalTime() - GetInitial()).TotalMilliseconds;
        }

        /// <summary>
        /// Gets the calendar date and time of a day.
        /// </summary>
        /// <param name="dateTime">the date to be returned as calendar</param>
        /// <returns>the calendar date and time of a day</returns>
        public static DateTime GetCalendar(DateTime dateTime) { 
            return dateTime;
        }

        /// <summary>
        /// Gets the current time in the default time zone with the default locale.
        /// </summary>
        /// <returns>the current time in the default time zone with the default locale</returns>
        public static DateTime GetCurrentTime() {
            return DateTime.Now;
        }

        /// <summary>
        /// Gets the current time consistently.
        /// </summary>
        /// <returns>the time at which it was allocated, measured to the nearest millisecond</returns>
        public static DateTime GetCurrentUtcTime() {
            return DateTime.UtcNow;
        }
        
        /// <summary>
        /// Defines if date is in past.
        /// </summary>
        /// <returns>true if given date is in past, false instead</returns>
        public static bool IsInPast(DateTime date) {
            return date.CompareTo(GetCurrentTime()) < 0;
        }

        /// <summary>
        /// Gets the number of milliseconds since January 1, 1970, 00:00:00 GMT represented by specified date.
        /// </summary>
        /// <param name="date">the specified date to get time</param>
        /// <returns>the number of milliseconds since January 1, 1970, 00:00:00 GMT represented by the specified date</returns>
        public static long GetRelativeTime(DateTime date) {
            return (long) (date.ToUniversalTime() - GetInitial()).TotalMilliseconds;
        }

        /// <summary>
        /// Adds provided number of milliseconds to the DateTime.
        /// </summary>
        /// <param name="date">DateTime to increase</param>
        /// <param name="millis">number of milliseconds to add</param>
        /// <returns>updated DateTime</returns>
        public static DateTime AddMillisToDate(DateTime date, long millis) {
            return date.AddMilliseconds(millis);
        }

        /// <summary>
        /// Parses passing date with default {@code yyyy-MM-dd} pattern.
        /// </summary>
        /// <param name="date">date is date to be parse</param>
        /// <returns>parse date</returns>
        public static DateTime ParseWithDefaultPattern(String date) {
            return Parse(date, DEFAULT_PATTERN);
        }
        
        /// <summary>
        /// Parses passing date with specified format.
        /// </summary>
        /// <param name="date">the date to be parsed</param>
        /// <param name="format">the format of parsing the date</param>
        /// <returns>parsed date</returns>
        public static DateTime Parse(String date, String format) {
            // The method is rarely called, so every time we create a new DateTimeFormatInfo (necessary for automatic testing)
            DateTimeFormatInfo dateTimeFormatInfo = new DateTimeFormatInfo();
            dateTimeFormatInfo.Calendar = new GregorianCalendar();
            return DateTime.ParseExact(date, format, dateTimeFormatInfo);
        }

        /// <summary>
        /// Format passing date with default yyyy-MM-dd pattern.
        /// </summary>
        /// <param name="date">the date to be formatted</param>
        /// <returns>formatted date</returns>
        public static String FormatWithDefaultPattern(DateTime date) {
            return Format(date, DEFAULT_PATTERN);
        }
        
        /// <summary>
        /// Format passing date with specified pattern.
        /// </summary>
        /// <param name="date">the date to be formatted</param>
        /// <param name="pattern">pattern for format</param>
        /// <returns>formatted date</returns>
        public static String Format(DateTime date, String pattern)
        {
            return date.ToString(pattern, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the offset of time zone from UTC at the specified date.
        /// </summary>
        /// <param name="date">the date represented in milliseconds since January 1, 1970 00:00:00 GMT</param>
        /// <returns>the offset of time zone from UTC at the specified date adjusted with the amount of daylight saving.</returns>
        public static long GetCurrentTimeZoneOffset(DateTime date) {
            TimeZone tz = TimeZone.CurrentTimeZone;
            return (long) tz.GetUtcOffset(date).TotalMilliseconds;
        }

        /// <summary>
        /// Converts date to string of "yyyy.MM.dd HH:mm:ss z" format.
        /// </summary>
        /// <param name="date">date to convert.</param>
        /// <returns>string date value.</returns>
        public static String DateToString(DateTime signDate) {
            return signDate.ToLocalTime().ToString("yyyy.MM.dd HH:mm:ss zzz");
        }

        private static DateTime GetInitial() {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
