/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using iText.Test;
using NUnit.Framework;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>Tests for the DocumentType node</summary>
    /// <author>Jonathan Hedley, http://jonathanhedley.com/</author>
    public class DocumentTypeExceptionTest : ExtendedITextTest {
        [Test]
        public virtual void ConstructorValidationThrowsExceptionOnNulls() {
            Assert.Throws(typeof(ArgumentException), () => new DocumentType("html", null, null, ""));
        }
    }
}
