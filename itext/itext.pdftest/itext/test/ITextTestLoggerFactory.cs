/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
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
using Microsoft.Extensions.Logging;

namespace iText.Test
{
    internal class ITextTestLoggerFactory : ILoggerFactory
    {
        private readonly Dictionary<String, Boolean> expectedTemplates = new Dictionary<String, Boolean>();
        
        private readonly IList<ITextTestLogEvent> logEvents = new List<ITextTestLogEvent>();
        
        public void SetExpectedTemplates(Dictionary<String, Boolean> expectedTemplates) {
            this.expectedTemplates.Clear();
            foreach (KeyValuePair<String, Boolean> item in expectedTemplates)
            {
                this.expectedTemplates[item.Key] = item.Value;
            }
        }
        
        public IList<ITextTestLogEvent> GetLogEvents()
        {
            return logEvents;
        }
        
        public void Dispose()
        {
            expectedTemplates.Clear();
            logEvents.Clear();
        }

        public ILogger CreateLogger(string categoryName)
        {
            
            return new ITextTestLogger(categoryName, this);
        }

        public void AddProvider(ILoggerProvider provider)
        {
            // Now we don't implement provider logic cause creating loggers in factory is enough for testing and this
            // factory is internal. But logic with providers can be added in future if necessary. 
            throw new NotImplementedException();
        }

        private bool IsExpectedMessage(String message) {
            if (message != null) {
                foreach (var template in expectedTemplates) {
                    if (LogListenerHelper.EqualsMessageByTemplate(message, template.Key)) {
                        return true;
                    }
                }
            }
            return false;
        }
        
        private bool IsExpectedMessageQuiet(String message) {
            if (message != null) {
                foreach (String template in expectedTemplates.Keys) {
                    if (LogListenerHelper.EqualsMessageByTemplate(message, template) && expectedTemplates[template]) {
                        return true;
                    }
                }
            }
            return false;
        }

        private void AddLogEvent(ITextTestLogEvent testLogEvent)
        {
            logEvents.Add(testLogEvent);
        }

        public class ITextTestLogEvent
        {
            public readonly LogLevel logLevel;
            public readonly string message;
            
            public ITextTestLogEvent(LogLevel logLevel, string message)
            {
                this.logLevel = logLevel;
                this.message = message;
            }
        }
        
        private class ITextTestLogger : ILogger
        {
            private readonly string categoryName;
            private readonly ITextTestLoggerFactory factory;

            public ITextTestLogger(string categoryName, ITextTestLoggerFactory factory)
            {
                this.categoryName = categoryName;
                this.factory = factory;
            }
            
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (!factory.IsExpectedMessageQuiet(state.ToString()))
                {
                    Console.WriteLine(categoryName + ": " + state);
                }
                if (logLevel >= LogLevel.Warning || factory.IsExpectedMessage(state.ToString()))
                {
                    factory.AddLogEvent(new ITextTestLogEvent(logLevel, state.ToString()));
                }
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                // We allow all log levels for testing
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                // We dont expect using of scope logic for this test logger
                throw new NotImplementedException();
            }
        }
    }
}
