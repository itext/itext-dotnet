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
using Microsoft.Extensions.Logging;

namespace iText.Commons.Logs {
    public class TestLogger : ILogger {
        private readonly TestLoggerStats stats = new TestLoggerStats();

        private readonly bool isEnabled;

        public TestLogger(bool isEnabled) {
            this.isEnabled = isEnabled;
        }
        
        public bool IsEnabled(LogLevel logLevel)
        {
            return isEnabled;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            String msg = formatter(state, exception);
            switch (logLevel) {
                case LogLevel.Trace:
                    if (exception == null) {
                        stats.AddTraceCall(msg);
                    } else {
                        stats.AddTraceWithThrowableCall(msg, exception);
                    }
                    break;
                case LogLevel.Debug:
                    if (exception == null) {
                        stats.AddDebugCall(msg);
                    } else {
                        stats.AddDebugWithThrowableCall(msg, exception);
                    }
                    break;
                case LogLevel.Information:
                    if (exception == null) {
                        stats.AddInfoCall(msg);
                    } else {
                        stats.AddInfoWithThrowableCall(msg, exception);
                    }
                    break;
                case LogLevel.Warning:
                    if (exception == null) {
                        stats.AddWarnCall(msg);
                    } else {
                        stats.AddWarnWithThrowableCall(msg, exception);
                    }
                    break;
                case LogLevel.Error:
                    if (exception == null) {
                        stats.AddErrorCall(msg);
                    } else {
                        stats.AddErrorWithThrowableCall(msg, exception);
                    }
                    break;
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotSupportedException();
        }
        
        public virtual TestLoggerStats GetStats() {
            return stats;
        }
    }
}
