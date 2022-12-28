/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
            if (attributes.Length > 0) {
                HashSet<String> expectedTemplates = new HashSet<string>();
                for (int i = 0; i < attributes.Length; i++) {
                    LogMessageAttribute logMessage = attributes[i];
                    expectedTemplates.Add(logMessage.GetMessageTemplate());
                }
                TEST_LOGGER_FACTORY.SetExpectedTemplates(expectedTemplates);
            }
        }

        private int GetSize() {
            return TEST_LOGGER_FACTORY.GetLogEvents().Count;
        }
    }
}
