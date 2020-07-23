/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2020 iText Group NV
    Authors: iText Software.

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
using NUnit.Framework;

namespace iText.Test {
    public class LoggerHelperTest : ExtendedITextTest
    {
        [NUnit.Framework.Test]
        public void EqualsMessageByTemplate()
        {
            string pattern = "There might be a message: {0}";
            string example = string.Format(pattern, "message");
            Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public void EqualsMessageByTemplateWithEmptyParameter()
        {
            string pattern = "There might be a message: {0}";
            string example = string.Format(pattern, "message");
            Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public void EqualsMessageByTemplateWithMultipleParameters()
        {
            string pattern = "There might be messages: {0} {1}";
            string example = string.Format(pattern, "message1", "message2");
            Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public void EqualsMessageByTemplateWithQuotes()
        {
            string pattern = "There might be a message '': {0}";
            string example = "There might be a message ': message";
            Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public void EqualsMessageByTemplateWithCyrillic()
        {
            string pattern = "There might be a cyrillic message: {0}";
            string example = string.Format(pattern, "сообщение");
            Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public void EqualsMessageByTemplateWithAsterisks()
        {
            string pattern = "some text * *** {0}";
            string example = string.Format(pattern, "message");
            Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public void EqualsMessageByTemplateWithBrackets()
        {
            string pattern = "some text ( ) (0) ( {0}";
            string example = string.Format(pattern, "message");
            Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public void EqualsMessageByTemplateWithSquareBrackets()
        {
            string pattern = "some text [ ] [0] [ {0}";
            string example = string.Format(pattern, "message");
            Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public void EqualsMessageByTemplateWithQuestionSign()
        {
            string pattern = "some text ? ??? .*? {0}";
            string example = string.Format(pattern, "message");
            Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public void EqualsMessageByTemplateWithDot()
        {
            string pattern = "some text . ... .* {0}";
            string example = string.Format(pattern, "message");
            Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public void EqualsMessageByTemplateWithBraces()
        {
            string pattern = "some text {} {a} { {0}";
            string example = "some text {} {a} { message";
            Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public void EqualsComplicatedMessageByTemplate()
        {
            string pattern = "Not supported list style type ? {a} [b] . * (not working) {0}";
            string example = "Not supported list style type ? {a} [b] . * (not working) *some phrase instead of template*";
            Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public void NotEqualsMessageByTemplate()
        {
            string pattern = "There must be a message: {0}";
            string example = "There should be a message: message";
            Assert.IsFalse(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }
    }
}
