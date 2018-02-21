using System;
using NUnit.Framework;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>Tests for the DocumentType node</summary>
    /// <author>Jonathan Hedley, http://jonathanhedley.com/</author>
    public class DocumentTypeExceptionTest {
        [Test]
        public virtual void ConstructorValidationThrowsExceptionOnNulls() {
            Assert.Throws(typeof(ArgumentException), () => new DocumentType("html", null, null, ""));
        }
    }
}
