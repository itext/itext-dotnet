/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2026 Apryse Group NV
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
using System.Threading;
using iText.Test;
using NUnit.Framework;

namespace iText.Commons.Utils {
    public class DateTimeUtilTest : ExtendedITextTest {
        
        private const double ZERO_DELTA = 1e-6;
        private const double ONE_SECOND_DELTA = 1000.0;
        
        [Test]
        public virtual void GetCurrentTest() {
            DateTime date = DateTime.Now;
            Assert.AreEqual(date.ToString(), DateTimeUtil.GetCurrentTime().ToString());
        }

        [Test]
        public virtual void IsInPastTest() {
            DateTime date = new DateTime(1);
            Assert.IsTrue(DateTimeUtil.IsInPast(date));
        }
        
        [Test]
        public void ParseDateAndGetUtcMillisFromEpochTest()
        {
            DateTime date = DateTimeUtil.ParseWithDefaultPattern("2020-05-05");
            DateTime parsedDate = DateTimeUtil.GetCalendar(date);
            double millisFromEpochTo2020_05_05 = DateTimeUtil.GetUtcMillisFromEpoch(parsedDate);

            long offset = DateTimeUtil.GetCurrentTimeZoneOffset(date);
            Assert.AreEqual(1588636800000d - offset, millisFromEpochTo2020_05_05, ZERO_DELTA);
        }

        [Test]
        public void AddMillisToDateTest() {
            DateTime almostCurrentTime = DateTime.Now.AddMilliseconds(-2000);
            long twoSeconds = 2000;
            Assert.AreEqual(DateTimeUtil.GetRelativeTime(DateTime.Now),
                DateTimeUtil.GetRelativeTime(DateTimeUtil.AddMillisToDate(almostCurrentTime, twoSeconds)),
                ONE_SECOND_DELTA);
        }

        [Test]
        public void CompareUtcMillisFromEpochWithNullParamAndCurrentTimeTest() {
            double getUtcMillisFromEpochWithNullParam = DateTimeUtil.GetUtcMillisFromEpoch(null);
            double millisFromEpochToCurrentTime = DateTimeUtil.GetUtcMillisFromEpoch(DateTimeUtil.GetCurrentUtcTime());

            Assert.AreEqual(millisFromEpochToCurrentTime, getUtcMillisFromEpochWithNullParam, ONE_SECOND_DELTA);
        }

        [Test]
        public void ParseDateAndGetRelativeTimeTest()
        {
            DateTime date = DateTimeUtil.ParseWithDefaultPattern("2020-05-05");
            long relativeTime = DateTimeUtil.GetRelativeTime(date);

            long offset = DateTimeUtil.GetCurrentTimeZoneOffset(date);
            
            Assert.AreEqual(1588636800000d - offset, relativeTime, ZERO_DELTA);
        }

        [Test]
        public void serializeDateToISO8601Test()
        {
            DateTime localDateTime = new DateTime(2000, 1, 11, 12, 13, 14, 0);
            String actualString = DateTimeUtil.SerializeDateToISO8601(localDateTime);
            Assert.AreEqual("2000-01-11T12:13:14", actualString);
        }

        [Test]
        public void ofEpochSecondUTCTest()
        {
            long timeInSeconds = 1000000000;
            DateTime actualTime = DateTimeUtil.OfEpochSecondUTC(timeInSeconds);
            Assert.AreEqual(2001, actualTime.Year);
            Assert.AreEqual(9, actualTime.Month);
            Assert.AreEqual(9, actualTime.Day);
            Assert.AreEqual(1, actualTime.Hour);
            Assert.AreEqual(46, actualTime.Minute);
            Assert.AreEqual(40, actualTime.Second);
        }

        [Test]
        public void getLocalDateTimeTest()
        {
            DateTime expectedTime = DateTime.Now;
            Thread.Sleep(10);
            DateTime actualTime = DateTimeUtil.GetCurrentTime();
            Assert.AreEqual(expectedTime.Year, actualTime.Year);
            Assert.AreEqual(expectedTime.Month, actualTime.Month);
            Assert.AreEqual(expectedTime.Day, actualTime.Day);
            Assert.IsTrue(expectedTime.CompareTo(actualTime) < 0);
        }
    }
}
