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
using NUnit.Framework;

namespace iText.Test {
    /// <summary>Utilities class for assertion operation.</summary>
    public class AssertUtil {
        private AssertUtil() {
        }

        public static void AssertThrows(Type expectedThrowable, TestDelegate runnable)
        {
            Assert.That(runnable, Throws.InstanceOf(expectedThrowable));
        }

        public static void AssertThrows(string message, Type expectedThrowable, TestDelegate runnable)
        {
            Assert.That(runnable, Throws.InstanceOf(expectedThrowable).With.Message.EqualTo(message));
        }

        /// <summary>Assert that the assertion passed within the timeout</summary>
        /// <param name="assertion">callback to the actual asserts to be safeguarded</param>
        /// <param name="timeout">the maximum time it can take before passing the assertions</param>
        public static void AssertPassedWithinTimeout(Action assertion, TimeSpan timeout)
        {
            // Pass 1 millis sleepTime to force thread yield
            AssertPassedWithinTimeout(assertion, timeout, TimeSpan.FromMilliseconds(1));
        }

        /// <summary>Assert that the assertion passed within the timeout</summary>
        /// <param name="assertion">callback to the actual asserts to be safeguarded</param>
        /// <param name="timeout">the maximum time it can take before passing the assertions</param>
        /// <param name="sleepTime">the time to sleep between polls</param>
        public static void AssertPassedWithinTimeout(Action assertion, TimeSpan timeout, TimeSpan
             sleepTime) {
            AssertPassedWithinTimeout(assertion, timeout, () => Convert.ToInt64(Math.Round(sleepTime.TotalMilliseconds)));
        }

        /// <summary>Assert that the assertion passed within the random timeout.</summary>
        /// <remarks>
        /// Assert that the assertion passed within the random timeout.
        /// <para />
        /// This method may be useful when your tests fetch some stateful resources concurrently. It allows to de-synchronize
        /// access from different clients. It's exponential backoff with jitter but without 'exponential' part. Because
        /// it does not really care if tests become longer, the main goal is to pass the tests.
        /// </remarks>
        /// <param name="assertion">callback to the actual asserts to be safeguarded</param>
        /// <param name="timeout">the maximum time it can take before passing the assertions</param>
        /// <param name="maxSleepTime">the maximum time to sleep between polls</param>
        public static void AssertPassedWithinRandomTimeout(Action assertion, TimeSpan timeout
            , TimeSpan maxSleepTime) {
            AssertPassedWithinTimeout(assertion, timeout, () => Convert.ToInt64(Math.Round(maxSleepTime.TotalMilliseconds * new Random().NextDouble())));
        }

        private static void AssertPassedWithinTimeout(Action assertion, TimeSpan timeout, Func<long> timeoutCalc) {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var elapsedMs = watch.ElapsedMilliseconds;
            bool passed = false;
            while (!passed)
            {
                try
                {
                    assertion.Invoke();
                    passed = true;
                }
                catch (AssertionException e)
                {
                    if (timeout.TotalMilliseconds < watch.ElapsedMilliseconds)
                    {
                        throw e;
                    }
                    int a = (int) timeoutCalc();
                    Thread.Sleep((int) timeoutCalc());
                    //ignore assertion failure if timeout not spent.                                                                                       
                }

            }
            watch.Stop();
        }

        public static void AreEqual(long expected, long actual, Func<string> messageGenerator)
        {
            Assert.AreEqual(expected, actual, 0, messageGenerator());
        }

        public static void AreEqual(int expected, int actual, Func<string> messageGenerator)
        {
            Assert.AreEqual(expected, actual, 0, messageGenerator());
        }

        public static void AreEqual(double expected, double actual, Func<string> messageGenerator)
        {
            Assert.AreEqual(expected, actual, 0, messageGenerator());
        }
        
        public static void AreEqual(object expected, object actual, Func<string> messageGenerator)
        {
            Assert.AreEqual(expected, actual, messageGenerator());
        }
    }
}
