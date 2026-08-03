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
using System.Collections.Generic;
using iText.Commons.Datastructures;
using iText.Commons.Exceptions;
using iText.Test;

namespace iText.Commons.Logs {
    [NUnit.Framework.Category("UnitTest")]
    public class LazyLoggerTest : ExtendedITextTest {
        private static readonly Exception TEST_EXCEPTION = new ITextException();

        [NUnit.Framework.Test]
        public virtual void ErrorEnabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(true);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsTrue(logger.IsErrorEnabled());
            logger.Error(() => testMessageProvider.Provide());
            NUnit.Framework.Assert.AreEqual(1, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            IList<String> calls = logStats.GetErrorCalls();
            NUnit.Framework.Assert.AreEqual(1, calls.Count);
            NUnit.Framework.Assert.AreEqual(TestStringProvider.MESSAGE, calls[0]);
            NUnit.Framework.Assert.AreEqual(1, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void ErrorDisabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(false);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsFalse(logger.IsErrorEnabled());
            logger.Error(() => testMessageProvider.Provide());
            NUnit.Framework.Assert.AreEqual(0, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            NUnit.Framework.Assert.AreEqual(0, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void ErrorWithExceptionEnabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(true);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsTrue(logger.IsErrorEnabled());
            logger.Error(() => testMessageProvider.Provide(), TEST_EXCEPTION);
            NUnit.Framework.Assert.AreEqual(1, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            IList<Tuple2<String, Exception>> calls = logStats.GetErrorWithThrowableCalls();
            NUnit.Framework.Assert.AreEqual(1, calls.Count);
            Tuple2<String, Exception> call = calls[0];
            NUnit.Framework.Assert.AreEqual(TestStringProvider.MESSAGE, call.GetFirst());
            NUnit.Framework.Assert.AreSame(TEST_EXCEPTION, call.GetSecond());
            NUnit.Framework.Assert.AreEqual(1, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void ErrorWithExceptionDisabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(false);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsFalse(logger.IsErrorEnabled());
            logger.Error(() => testMessageProvider.Provide(), TEST_EXCEPTION);
            NUnit.Framework.Assert.AreEqual(0, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            NUnit.Framework.Assert.AreEqual(0, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void WarnEnabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(true);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsTrue(logger.IsWarnEnabled());
            logger.Warn(() => testMessageProvider.Provide());
            NUnit.Framework.Assert.AreEqual(1, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            IList<String> calls = logStats.GetWarnCalls();
            NUnit.Framework.Assert.AreEqual(1, calls.Count);
            NUnit.Framework.Assert.AreEqual(TestStringProvider.MESSAGE, calls[0]);
            NUnit.Framework.Assert.AreEqual(1, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void WarnDisabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(false);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsFalse(logger.IsWarnEnabled());
            logger.Warn(() => testMessageProvider.Provide());
            NUnit.Framework.Assert.AreEqual(0, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            NUnit.Framework.Assert.AreEqual(0, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void WarnWithExceptionEnabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(true);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsTrue(logger.IsWarnEnabled());
            logger.Warn(() => testMessageProvider.Provide(), TEST_EXCEPTION);
            NUnit.Framework.Assert.AreEqual(1, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            IList<Tuple2<String, Exception>> calls = logStats.GetWarnWithThrowableCalls();
            NUnit.Framework.Assert.AreEqual(1, calls.Count);
            Tuple2<String, Exception> call = calls[0];
            NUnit.Framework.Assert.AreEqual(TestStringProvider.MESSAGE, call.GetFirst());
            NUnit.Framework.Assert.AreSame(TEST_EXCEPTION, call.GetSecond());
            NUnit.Framework.Assert.AreEqual(1, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void WarnWithExceptionDisabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(false);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsFalse(logger.IsWarnEnabled());
            logger.Warn(() => testMessageProvider.Provide(), TEST_EXCEPTION);
            NUnit.Framework.Assert.AreEqual(0, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            NUnit.Framework.Assert.AreEqual(0, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void InfoEnabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(true);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsTrue(logger.IsInfoEnabled());
            logger.Info(() => testMessageProvider.Provide());
            NUnit.Framework.Assert.AreEqual(1, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            IList<String> calls = logStats.GetInfoCalls();
            NUnit.Framework.Assert.AreEqual(1, calls.Count);
            NUnit.Framework.Assert.AreEqual(TestStringProvider.MESSAGE, calls[0]);
            NUnit.Framework.Assert.AreEqual(1, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void InfoDisabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(false);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsFalse(logger.IsInfoEnabled());
            logger.Info(() => testMessageProvider.Provide());
            NUnit.Framework.Assert.AreEqual(0, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            NUnit.Framework.Assert.AreEqual(0, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void InfoWithExceptionEnabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(true);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsTrue(logger.IsInfoEnabled());
            logger.Info(() => testMessageProvider.Provide(), TEST_EXCEPTION);
            NUnit.Framework.Assert.AreEqual(1, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            IList<Tuple2<String, Exception>> calls = logStats.GetInfoWithThrowableCalls();
            NUnit.Framework.Assert.AreEqual(1, calls.Count);
            Tuple2<String, Exception> call = calls[0];
            NUnit.Framework.Assert.AreEqual(TestStringProvider.MESSAGE, call.GetFirst());
            NUnit.Framework.Assert.AreSame(TEST_EXCEPTION, call.GetSecond());
            NUnit.Framework.Assert.AreEqual(1, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void InfoWithExceptionDisabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(false);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsFalse(logger.IsInfoEnabled());
            logger.Info(() => testMessageProvider.Provide(), TEST_EXCEPTION);
            NUnit.Framework.Assert.AreEqual(0, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            NUnit.Framework.Assert.AreEqual(0, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void DebugEnabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(true);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsTrue(logger.IsDebugEnabled());
            logger.Debug(() => testMessageProvider.Provide());
            NUnit.Framework.Assert.AreEqual(1, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            IList<String> calls = logStats.GetDebugCalls();
            NUnit.Framework.Assert.AreEqual(1, calls.Count);
            NUnit.Framework.Assert.AreEqual(TestStringProvider.MESSAGE, calls[0]);
            NUnit.Framework.Assert.AreEqual(1, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void DebugDisabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(false);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsFalse(logger.IsDebugEnabled());
            logger.Debug(() => testMessageProvider.Provide());
            NUnit.Framework.Assert.AreEqual(0, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            NUnit.Framework.Assert.AreEqual(0, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void DebugWithExceptionEnabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(true);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsTrue(logger.IsDebugEnabled());
            logger.Debug(() => testMessageProvider.Provide(), TEST_EXCEPTION);
            NUnit.Framework.Assert.AreEqual(1, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            IList<Tuple2<String, Exception>> calls = logStats.GetDebugWithThrowableCalls();
            NUnit.Framework.Assert.AreEqual(1, calls.Count);
            Tuple2<String, Exception> call = calls[0];
            NUnit.Framework.Assert.AreEqual(TestStringProvider.MESSAGE, call.GetFirst());
            NUnit.Framework.Assert.AreSame(TEST_EXCEPTION, call.GetSecond());
            NUnit.Framework.Assert.AreEqual(1, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void DebugWithExceptionDisabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(false);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsFalse(logger.IsDebugEnabled());
            logger.Debug(() => testMessageProvider.Provide(), TEST_EXCEPTION);
            NUnit.Framework.Assert.AreEqual(0, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            NUnit.Framework.Assert.AreEqual(0, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void TraceEnabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(true);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsTrue(logger.IsTraceEnabled());
            logger.Trace(() => testMessageProvider.Provide());
            NUnit.Framework.Assert.AreEqual(1, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            IList<String> calls = logStats.GetTraceCalls();
            NUnit.Framework.Assert.AreEqual(1, calls.Count);
            NUnit.Framework.Assert.AreEqual(TestStringProvider.MESSAGE, calls[0]);
            NUnit.Framework.Assert.AreEqual(1, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void TraceDisabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(false);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsFalse(logger.IsTraceEnabled());
            logger.Trace(() => testMessageProvider.Provide());
            NUnit.Framework.Assert.AreEqual(0, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            NUnit.Framework.Assert.AreEqual(0, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void TraceWithExceptionEnabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(true);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsTrue(logger.IsTraceEnabled());
            logger.Trace(() => testMessageProvider.Provide(), TEST_EXCEPTION);
            NUnit.Framework.Assert.AreEqual(1, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            IList<Tuple2<String, Exception>> calls = logStats.GetTraceWithThrowableCalls();
            NUnit.Framework.Assert.AreEqual(1, calls.Count);
            Tuple2<String, Exception> call = calls[0];
            NUnit.Framework.Assert.AreEqual(TestStringProvider.MESSAGE, call.GetFirst());
            NUnit.Framework.Assert.AreSame(TEST_EXCEPTION, call.GetSecond());
            NUnit.Framework.Assert.AreEqual(1, logStats.GetTotalInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void TraceWithExceptionDisabledTest() {
            TestStringProvider testMessageProvider = new TestStringProvider();
            TestLogger testLogger = new TestLogger(false);
            LazyLogger logger = new LazyLogger(testLogger);
            NUnit.Framework.Assert.IsFalse(logger.IsTraceEnabled());
            logger.Trace(() => testMessageProvider.Provide(), TEST_EXCEPTION);
            NUnit.Framework.Assert.AreEqual(0, testMessageProvider.GetCallCount());
            TestLoggerStats logStats = testLogger.GetStats();
            NUnit.Framework.Assert.AreEqual(0, logStats.GetTotalInvocationsCount());
        }
    }
}
