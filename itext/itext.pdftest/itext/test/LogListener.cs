/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons;
using iText.IO;
using iText.Test.Attributes;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace iText.Test {
    [AttributeUsage(AttributeTargets.Class)]
    public class LogListener : TestActionAttribute {
        
        private static readonly ITextTestLoggerFactory TEST_LOGGER_FACTORY;
        
        private ILoggerFactory defaultLoggerFactory;

        static LogListener() {
            TEST_LOGGER_FACTORY = new ITextTestLoggerFactory();
        }

        public override void BeforeTest(ITest testDetails)
        {
            defaultLoggerFactory = ITextLogManager.GetLoggerFactory();
            ITextLogManager.SetLoggerFactory(TEST_LOGGER_FACTORY);
            Init(testDetails);
        }

        public override void AfterTest(ITest testDetails) {
            CheckLogMessages(testDetails);
            ITextLogManager.SetLoggerFactory(defaultLoggerFactory);
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
            IList<ITextTestLoggerFactory.ITextTestLogEvent> eventList = TEST_LOGGER_FACTORY.GetLogEvents();
            int index = 0;
            for (int i = 0; i < eventList.Count; i++) {
                if (IsLevelCompatible(loggingStatement.LogLevel, eventList[i].logLevel) 
                    && LogListenerHelper.EqualsMessageByTemplate(eventList[i].message, loggingStatement.GetMessageTemplate())) {
                    index++;
                }
            }
            return index;
        }
        
        private bool IsLevelCompatible(int logMessageLevel, LogLevel eventLevel) {
            switch (logMessageLevel) {
                case LogLevelConstants.UNKNOWN:
                    return eventLevel >= LogLevel.Warning;
                case LogLevelConstants.ERROR:
                    return eventLevel == LogLevel.Error;
                case LogLevelConstants.WARN:
                    return eventLevel == LogLevel.Warning;
                case LogLevelConstants.INFO:
                    return eventLevel == LogLevel.Information;
                case LogLevelConstants.DEBUG:
                    return eventLevel == LogLevel.Debug;
                default:
                    return false;
            }
        }

        private void Init(ITest testDetails) {
            TEST_LOGGER_FACTORY.Dispose();
            LogMessageAttribute[] attributes = LogListenerHelper.GetTestAttributes<LogMessageAttribute>(testDetails);
            if (attributes.Length > 0)
            {
                Dictionary<String, Boolean> expectedTemplates = new Dictionary<string, bool>();

                for (int i = 0; i < attributes.Length; i++) {
                    LogMessageAttribute logMessage = attributes[i];
                    expectedTemplates.Add(logMessage.GetMessageTemplate(), logMessage.QuietMode);
                }
                TEST_LOGGER_FACTORY.SetExpectedTemplates(expectedTemplates);
            }
        }

        private int GetSize() {
            return TEST_LOGGER_FACTORY.GetLogEvents().Count;
        }
    }
}
