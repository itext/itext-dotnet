using System;
using log4net.Appender;
using log4net.Core;

namespace iTextSharp.Test {
    public class ITextMemoryAppender : MemoryAppender {
        protected override void Append(LoggingEvent le) {
            Console.WriteLine(le.LoggerName + ": " + le.RenderedMessage);
            base.Append(le);
        }
    }
}
