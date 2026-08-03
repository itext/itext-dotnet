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
