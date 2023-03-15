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
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

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

        public static void FailWrongMessageCount(int expected, int actual, String messageTemplate, ITest testDetails) {
            Assert.Fail("{0} Expected to find {1}, but found {2} messages with the following content: \"{3}\"",
                            testDetails.FullName, expected, actual, messageTemplate);
        }

        public static void FailWrongTotalCount(int expected, int actual, ITest testDetails) {
            Assert.Fail("{0}: The test does not check the message logging - {1} messages",
                    testDetails.FullName,
                    expected - actual);
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
    }
}
