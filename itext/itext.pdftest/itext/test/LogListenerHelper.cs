/*
    This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Org.BouncyCastle.Crypto.Engines;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace iText.Test {
    internal class LogListenerHelper {

        public static T[] GetTestAttributes<T>(ITest testDetails) where T : class {
            T[] attributes = testDetails.Method.GetCustomAttributes<T>(true);
            if (attributes.Length == 0)
            {
                attributes = testDetails.Fixture.GetType().GetCustomAttributes(typeof(T), true)
                    .Select(attr => (T)attr).ToArray();
            }
            return attributes;
        }

        public static void FailWrongMessageCount(int expected, int actual, String messageTemplate, ITest testDetails, System.Collections.Generic.IList<ITextTestLoggerFactory.ITextTestLogEvent> textTestLogEvents) {
            Assert.Fail("{0} Expected to find {1}, but found {2} messages with the following content: \"{3}\"\nActual logs:\n{4}",
                            testDetails.FullName, expected, actual, messageTemplate, createActualLogsMessage(textTestLogEvents));
        }

        public static void FailWrongTotalCount(int expected, int actual, ITest testDetails, System.Collections.Generic.IList<ITextTestLoggerFactory.ITextTestLogEvent> textTestLogEvents) {
            Assert.Fail("{0}: The test does not check the message logging - {1} messages\nActual logs:\n{2}",
                    testDetails.FullName,
                    expected - actual, createActualLogsMessage(textTestLogEvents));
        }

        /*
        * compare  parametrized message with  base template, for example:
        *  "Hello fox1 , World  fox2 !" with "Hello {0} , World {1} !"
        * */

        public static bool EqualsMessageByTemplate(string message, string template)
        {
            if (template.Contains("{") && template.Contains("}"))
            {
                String templateWithoutParameters = Regex.Replace(Regex.Escape(template).Replace("''", "'"), "\\\\\\{[0-9]+?\\}", "(.)*?");
                templateWithoutParameters = "^" + templateWithoutParameters + "$";
                return Regex.IsMatch(message, templateWithoutParameters, RegexOptions.Singleline);
            }

            return message.Contains(template);
        }

        private static String createActualLogsMessage(IList<ITextTestLoggerFactory.ITextTestLogEvent> loggedMessages)
        {
            if (loggedMessages.Count == 0)
            {
                return "No messages were logged.";
            }
            StringBuilder sb = new StringBuilder();
            var sortedMessages = new List<ITextTestLoggerFactory.ITextTestLogEvent>(loggedMessages);
            sortedMessages.Sort((m1, m2) => compareEvents(m1, m2));
            ITextTestLoggerFactory.ITextTestLogEvent prevMessage = null;
            int count = 0;
            foreach (var e in loggedMessages)
            {
                if (prevMessage == null || compareEvents(e, prevMessage) == 0)
                {
                    count++;
                }
                else
                {
                    sb.Append('\t')
                            .Append("Occurrences: ").Append(count).Append(" - ")
                            .Append(prevMessage.categoryName).Append(" - ")
                            .Append(prevMessage.logLevel).Append(" : ")
                            .Append(prevMessage.message)
                            .Append("\n");
                    count = 1;
                }
                prevMessage = e;
            }
            sb.Append('\t')
                    .Append("Occurrences: ").Append(count).Append(" - ")
                    .Append(prevMessage.categoryName).Append(" - ")
                    .Append(prevMessage.logLevel).Append(" : ")
                    .Append(prevMessage.message)
                    .Append("\n");
            return sb.ToString();
        }

        private static int compareEvents(ITextTestLoggerFactory.ITextTestLogEvent m1, ITextTestLoggerFactory.ITextTestLogEvent m2)
        {
            if (m1 == null && m2 == null)
            {
                return 0;
            }
            if (m1 == null && m2 != null)
            {
                return -1;
            }
            if (m2 == null && m1 != null)
            {
                return 1;
            }
            int result = m1.categoryName.CompareTo(m2.categoryName);
            if (result == 0)
            {
                result = m1.logLevel.CompareTo(m2.logLevel);
            }
            if (result == 0)
            {
                result = m1.message.CompareTo(m2.message);
            }
            return result;
        }
    }
}
