using System;
using Microsoft.Extensions.Logging;

namespace iText.Commons {
    /// <summary>
    /// iText static log manager working with the <c>Microsoft.Extensions.Logging</c> framework. Use it to add iText
    /// logs to your application. Call <see cref = "SetLoggerFactory" /> to set up a logger factory that will
    /// receive iText log messages.
    /// </summary>
    public static class ITextLogManager {
        private static ILoggerFactory _loggerFactory;

        static ITextLogManager() {
            _loggerFactory = new LoggerFactory();
        }

        /// <summary>
        /// Sets the implementation of <see cref="Microsoft.Extensions.Logging.ILoggerFactory"/> to be used for creating <see cref="Microsoft.Extensions.Logging.ILogger"/>
        /// objects.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public static void SetLoggerFactory(ILoggerFactory factory) {
            _loggerFactory = factory;
        }

        /// <summary>
        /// Gets an instance of the used logger factory.
        /// </summary>
        /// <returns>The factory.</returns>
        public static ILoggerFactory GetLoggerFactory() {
            return _loggerFactory;
        }

        /// <summary>
        /// Creates a new <see cref="Microsoft.Extensions.Logging.ILogger"/> instance using the full name of the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static ILogger GetLogger(Type type) {
            return _loggerFactory.CreateLogger(type);
        }
    }
}