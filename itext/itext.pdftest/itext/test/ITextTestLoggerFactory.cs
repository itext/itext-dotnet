/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2025 Apryse Group NV
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace iText.Test
{
    internal class ITextTestLoggerFactory : ILoggerFactory
    {
        private readonly IDictionary<int, IDictionary<string, bool>> expectedTemplates = new ConcurrentDictionary<int, IDictionary<string, bool>>();
        
        private readonly IDictionary<int, IList<ITextTestLogEvent>> logEvents = new ConcurrentDictionary<int, IList<ITextTestLogEvent>>();

        private static readonly int DEFAULT_THREAD_ID = 1;
        private static bool threadsAware = false;

        public ITextTestLoggerFactory()
        {
            InitThreadAwareness();
        }

        public void SetExpectedTemplates(Dictionary<string, bool> expectedTemplates) {
            var threadId = GetThreadID();
            
            if (!this.expectedTemplates.ContainsKey(threadId))
            {
                this.expectedTemplates.Add(threadId, new Dictionary<string, bool>());
            }
            this.expectedTemplates[threadId] = expectedTemplates;
        }
        
        public IList<ITextTestLogEvent> GetLogEvents()
        {
            var threadId = GetThreadID();

            return logEvents.ContainsKey(threadId) ? logEvents[threadId] : new List<ITextTestLogEvent>();
        }
        
        public void Dispose()
        {
            var threadId = GetThreadID();
            if (expectedTemplates.ContainsKey(threadId))
            {
                expectedTemplates[threadId].Clear();
            }

            if (logEvents.ContainsKey(threadId))
            {
                logEvents[threadId].Clear();
            }
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

        private bool IsExpectedMessage(string message, int threadId) {
            if (message != null && expectedTemplates.ContainsKey(threadId)) {
                foreach (var template in expectedTemplates[threadId].Keys) {
                    if (LogListenerHelper.EqualsMessageByTemplate(message, template)) {
                        return true;
                    }
                }
            }
            return false;
        }
        
        private bool IsExpectedMessageQuiet(string message, int threadId) {
            if (message != null && expectedTemplates.ContainsKey(threadId)) {
                foreach (var template in expectedTemplates[threadId].Keys) {
                    if (LogListenerHelper.EqualsMessageByTemplate(message, template) && expectedTemplates[threadId][template]) {
                        return true;
                    }
                }
            }
            return false;
        }

        private void AddLogEvent(ITextTestLogEvent testLogEvent)
        {
            var threadId = GetThreadID();

            if (!logEvents.ContainsKey(threadId))
            {
                logEvents.Add(threadId, new List<ITextTestLogEvent>());
            }

            logEvents[threadId].Add(testLogEvent);
        }

        private static void InitThreadAwareness()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in loadedAssemblies)
            {
                if (asm.IsDynamic || !asm.CodeBase.ToLower().EndsWith("tests.dll"))
                {
                    continue;
                }

                var attrs = asm.GetCustomAttributes(true);
                int parallel = attrs.Count(t => t is ParallelizableAttribute);
                if (parallel > 0)
                {
                    threadsAware = true;
                    return;
                }
            }
        }

        private static int GetThreadID()
        {
            if (threadsAware)
            {
                return Thread.CurrentThread.ManagedThreadId;
            }
            else
            {
                return DEFAULT_THREAD_ID;
            }
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

        internal class ITextTestLogger : ILogger
        {
            private readonly string categoryName;
            private readonly ITextTestLoggerFactory factory;
            private bool runInSilentMode;

            private static readonly string TOKEN_ITEXT_SILENT_MODE = "ITEXT_SILENT_MODE";
            private static readonly string ITEXT_LICENCING_PACKAGE = "iText.Licensing";
            private static readonly string ITEXT_ACTIONS_PACKAGE = "iText.Commons.Actions.Processors";
            
            public ITextTestLogger(string categoryName, ITextTestLoggerFactory factory)
            {
                this.categoryName = categoryName;
                this.factory = factory;
                SetupRunInSilentMode(); 
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (!factory.IsExpectedMessageQuiet(state.ToString(), GetThreadID()))
                {
                    if (ShouldPrintMessage(categoryName)) 
                    {
                        Console.WriteLine( categoryName + ": " + state);
                    }
                }
                if (logLevel >= LogLevel.Warning || factory.IsExpectedMessage(state.ToString(), GetThreadID()))
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

            private bool ShouldPrintMessage(string category) {
                if (category.StartsWith(ITEXT_LICENCING_PACKAGE))
                {
                    return true;
                }
                if (category.StartsWith(ITEXT_ACTIONS_PACKAGE))
                {
                    return true;
                }
                return !runInSilentMode;
            }
            
            private void SetupRunInSilentMode() {
                var envIsSilentModeEnabled = Environment.GetEnvironmentVariable(TOKEN_ITEXT_SILENT_MODE);
                if (string.IsNullOrEmpty(envIsSilentModeEnabled)) {
                    runInSilentMode = false;
                    return;
                }
                runInSilentMode = envIsSilentModeEnabled.ToUpper().Equals("TRUE");
            }
        }
    }
}
