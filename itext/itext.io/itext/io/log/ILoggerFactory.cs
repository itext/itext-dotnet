using System;

namespace iText.IO.Log
{
    public interface ILoggerFactory {

        ILogger GetLogger(Type klass);

        ILogger GetLogger(String name);

    }
}