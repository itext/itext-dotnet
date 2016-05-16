//using System;
//using System.Collections.Generic;
//using System.Text;
//using iTextSharp.text.log;
//using log4net;

//namespace iTextSharp.Test
//{
//    public class Log4NetLogger : ILogger
//    {
//        private ILog iLog = LogManager.GetLogger("iTextSharp");
//        public ILogger GetLogger(Type klass)
//        {
//            iLog = LogManager.GetLogger(klass);
//            return this;
//        }

//        public ILogger GetLogger(string name)
//        {
//            iLog = LogManager.GetLogger(name);
//            return this;
//        }

//        public bool IsLogging(Level level)
//        {
//            return true;
//        }

//        public void Warn(string message)
//        {
//            iLog.Warn(message);
//        }

//        public void Trace(string message)
//        {
//            iLog.Warn(message);
//        }

//        public void Debug(string message)
//        {
//            iLog.Debug(message);
//        }

//        public void Info(string message)
//        {
//            iLog.Info(message);
//        }

//        public void Error(string message)
//        {
//            iLog.Error(message);
//        }

//        public void Error(string message, Exception e)
//        {
//            iLog.Error(message, e);
//        }

//        public ILog GetILog()
//        {
//            return iLog;
//        }
//    }
//}
