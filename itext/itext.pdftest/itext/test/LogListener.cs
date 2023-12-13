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
using Common.Logging;
using Common.Logging.Simple;
using iText.Test.Attributes;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace iText.Test {
    [AttributeUsage(AttributeTargets.Class)]
    public class LogListener : TestActionAttribute {
        private static ITextMemoryAddapter adapter;

        private ILoggerFactoryAdapter defaultLogAdapter;

        static LogListener() {
            adapter = new ITextMemoryAddapter();
        }

        public override void BeforeTest(ITest testDetails) {
            defaultLogAdapter = LogManager.Adapter;
            LogManager.Adapter = adapter;
            Init(testDetails);
        }

        public override void AfterTest(ITest testDetails) {
            CheckLogMessages(testDetails);
            LogManager.Adapter = defaultLogAdapter;
        }

        public override ActionTargets Targets {
            get { return ActionTargets.Test; }
        }

        private void CheckLogMessages(ITest testDetails) {
            int checkedMessages = 0;
            LogMessageAttribute[] attributes = LogListenerHelper.GetTestAttributes<LogMessageAttribute>(testDetails);
            if (attributes.Length > 0) {
                for (int i = 0; i < attributes.Length; i++) {
                    LogMessageAttribute logMessage = attributes[i];
                    int foundCount = Contains(logMessage);
                    if (foundCount != logMessage.Count && !logMessage.Ignore) {
                        LogListenerHelper.FailWrongMessageCount(logMessage.Count, foundCount, logMessage.GetMessageTemplate(), testDetails);
                    } else {
                        checkedMessages += foundCount;
                    }
                }
            }

            if (GetSize() > checkedMessages) {
                LogListenerHelper.FailWrongTotalCount(GetSize(), checkedMessages, testDetails);
            }
        }

        private int Contains(LogMessageAttribute loggingStatement) {
            IList<CapturingLoggerEvent> eventList = adapter.LoggerEvents;
            int index = 0;
            for (int i = 0; i < eventList.Count; i++) {
                if (IsLevelCompatible(loggingStatement.LogLevel, eventList[i].Level) 
                    && LogListenerHelper.EqualsMessageByTemplate(eventList[i].RenderedMessage, loggingStatement.GetMessageTemplate())) {
                    index++;
                }
            }
            return index;
        }
        
        private bool IsLevelCompatible(int logMessageLevel, LogLevel eventLevel) {
            switch (logMessageLevel) {
                case LogLevelConstants.UNKNOWN:
                    return eventLevel >= LogLevel.Warn;
                case LogLevelConstants.ERROR:
                    return eventLevel == LogLevel.Error;
                case LogLevelConstants.WARN:
                    return eventLevel == LogLevel.Warn;
                case LogLevelConstants.INFO:
                    return eventLevel == LogLevel.Info;
                case LogLevelConstants.DEBUG:
                    return eventLevel == LogLevel.Debug;
                default:
                    return false;
            }
        }

        private void Init(ITest testDetails) {
            adapter.Clear();
            LogMessageAttribute[] attributes = LogListenerHelper.GetTestAttributes<LogMessageAttribute>(testDetails);
            if (attributes.Length > 0) {
                HashSet<String> expectedTemplates = new HashSet<string>();
                for (int i = 0; i < attributes.Length; i++) {
                    LogMessageAttribute logMessage = attributes[i];
                    expectedTemplates.Add(logMessage.GetMessageTemplate());
                }
                adapter.SetExpectedTemplates(expectedTemplates);
            }
        }

        private int GetSize() {
            return adapter.LoggerEvents.Count;
        }
    }
}
