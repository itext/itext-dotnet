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

namespace iText.Test {
    [NUnit.Framework.Category("UnitTest")]
    public class LoggerHelperTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NotEqualMessageWithSimilarStartTest() {
            String pattern = "There might be a message: {0} with text.";
            String example = "There might be a message: TEMP with text. And add some other text.";
            NUnit.Framework.Assert.IsFalse(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public virtual void NotEqualMessageWithSimilarEndTest() {
            String pattern = "a message: {0} with text.";
            String example = "There might be a message: TEMP with text.";
            NUnit.Framework.Assert.IsFalse(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public virtual void EqualsMessageByTemplate() {
            String pattern = "There might be a message: {0}";
            String example = String.Format(pattern, "message");
            NUnit.Framework.Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public virtual void EqualsMessageByTemplateWithEmptyParameter() {
            String pattern = "There might be a message: {0}";
            String example = String.Format(pattern, "message");
            NUnit.Framework.Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public virtual void EqualsMessageByTemplateWithMultipleParameters() {
            String pattern = "There might be messages: {0} {1}";
            String example = String.Format(pattern, "message1", "message2");
            NUnit.Framework.Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public virtual void EqualsMessageByTemplateWithQuotes() {
            String pattern = "There might be a message '': {0}";
            String example = "There might be a message ': message";
            NUnit.Framework.Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public virtual void EqualsMessageByTemplateWithCyrillic() {
            String pattern = "There might be a cyrillic message: {0}";
            String example = String.Format(pattern, "сообщение");
            NUnit.Framework.Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public virtual void EqualsMessageByTemplateWithAsterisks() {
            String pattern = "some text * *** {0}";
            String example = String.Format(pattern, "message");
            NUnit.Framework.Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public virtual void EqualsMessageByTemplateWithBrackets() {
            String pattern = "some text ( ) (0) ( {0}";
            String example = String.Format(pattern, "message");
            NUnit.Framework.Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public virtual void EqualsMessageByTemplateWithSquareBrackets() {
            String pattern = "some text [ ] [0] [ {0}";
            String example = String.Format(pattern, "message");
            NUnit.Framework.Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public virtual void EqualsMessageByTemplateWithQuestionSign() {
            String pattern = "some text ? ??? .*? {0}";
            String example = String.Format(pattern, "message");
            NUnit.Framework.Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public virtual void EqualsMessageByTemplateWithDot() {
            String pattern = "some text . ... .* {0}";
            String example = String.Format(pattern, "message");
            NUnit.Framework.Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public virtual void EqualsMessageByTemplateWithBraces() {
            String pattern = "some text {} {a} { {0}";
            String example = "some text {} {a} { message";
            NUnit.Framework.Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public virtual void EqualsComplicatedMessageByTemplate() {
            String pattern = "Not supported list style type ? {a} [b] . * (not working) {0}";
            String example = "Not supported list style type ? {a} [b] . * (not working) *some phrase instead of template*";
            NUnit.Framework.Assert.IsTrue(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }

        [NUnit.Framework.Test]
        public virtual void NotEqualsMessageByTemplate() {
            String pattern = "There might be a message: {0}";
            String example = "There should be a message: message";
            NUnit.Framework.Assert.IsFalse(LogListenerHelper.EqualsMessageByTemplate(example, pattern));
        }
    }
}
