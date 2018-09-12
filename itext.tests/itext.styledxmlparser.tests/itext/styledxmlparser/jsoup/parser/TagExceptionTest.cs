using System;
using NUnit.Framework;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>Tag tests.</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class TagExceptionTest {

        [Test]
        public virtual void ValueOfChecksNotNull() {
            Assert.Throws(typeof(ArgumentException), () => iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf(null));
        }
        
        [Test]
        public virtual void ValueOfChecksNotEmpty() {
            Assert.Throws(typeof(ArgumentException), () => iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf(" "));
        }
    }
}
