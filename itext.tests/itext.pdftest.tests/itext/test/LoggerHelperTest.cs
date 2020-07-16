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
