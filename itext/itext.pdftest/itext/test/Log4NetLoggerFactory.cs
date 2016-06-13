using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using iText.IO.Log;
using iText.Test;

namespace iText.Test
{
    public class Log4NetLoggerFactory : ILoggerFactory
    {
        private ConcurrentDictionary<Object, Log4NetLogger> cache = new  ConcurrentDictionary<Object, Log4NetLogger>(); 

        public ILogger GetLogger(Type klass) {
            Log4NetLogger result;
            if (!cache.TryGetValue(klass, out result)) {
                result = new Log4NetLogger(klass);
                cache.TryAdd(klass, result);
            }
            return result;
        }

        public ILogger GetLogger(string name) {
            Log4NetLogger result;
            if (!cache.TryGetValue(name, out result))
            {
                result = new Log4NetLogger(name);
                cache.TryAdd(name, result);
            }
            return result;
        }
    }
}
