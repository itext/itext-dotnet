using System;

namespace iText.IO.Log
{
    public class NoOpLoggerFactory : ILoggerFactory
    {
        private ILogger logger = new NoOpLogger();

        public ILogger GetLogger(Type klass) {
            return logger;
        }

        public ILogger GetLogger(string name) {
            return logger;
        }
    }
}
