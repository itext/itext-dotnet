using System;
using log4net;
using log4net.Core;
using ILogger = iTextSharp.IO.Log.ILogger;

namespace iTextSharp.Test {
    public class Log4NetLogger : ILogger {
        private ILog iLog = LogManager.GetLogger("iTextSharp");

        public ILogger GetLogger(Type klass) {
            iLog = LogManager.GetLogger(klass);
            return this;
        }

        public ILogger GetLogger(string name) {
            iLog = LogManager.GetLogger(name);
            return this;
        }

        public void Warn(string message) {
            iLog.Warn(message);
        }

        public bool IsWarnEnabled() {
            return iLog.IsWarnEnabled;
        }

        public void Trace(string message) {
            System.Diagnostics.Trace.WriteLine(message);
        }

        public bool IsTraceEnabled() {
            return iLog.Logger.IsEnabledFor(Level.Trace);
        }

        public void Debug(string message) {
            iLog.Debug(message);
        }

        public bool IsDebugEnabled() {
            return iLog.IsDebugEnabled;
        }

        public void Info(string message) {
            iLog.Info(message);
        }

        public bool IsInfoEnabled() {
            return iLog.IsInfoEnabled;
        }

        public void Error(string message) {
            iLog.Error(message);
        }

        public void Error(string message, Exception e) {
            iLog.Error(message, e);
        }

        public bool IsErrorEnabled() {
            return iLog.IsErrorEnabled;
        }

        public ILog GetILog() {
            return iLog;
        }
    }
}
