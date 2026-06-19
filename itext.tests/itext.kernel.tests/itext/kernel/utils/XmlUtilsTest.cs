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
using System;
using System.IO;
using System.Xml;
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class XmlUtilsTest : ExtendedITextTest {
        private const String XML_WITH_XXE = "<?xml version=\"1.0\"?>\n" + "<!DOCTYPE r [ <!ENTITY xxe SYSTEM \"xxe-data.txt\"> ]>\n"
             + "<body xmlns=\"http://www.w3.org/1999/xhtml\"><p>&xxe;</p></body>";

        [NUnit.Framework.SetUp]
        public virtual void ResetXmlParserFactoryToDefault() {
            XmlProcessorCreator.SetXmlParserFactory(null);
        }

        [NUnit.Framework.Test]
        public virtual void CompareXmlsSameStructureDifferentWhitespace() {
            String pretty = "<root>\n" + "  <a>1</a>\n" + "  <b>2</b>\n" + "</root>";
            String compact = "<root><a>1</a><b>2</b></root>";
            NUnit.Framework.Assert.IsTrue(XmlUtils.CompareXmls(GetStream(pretty), GetStream(compact)));
        }

        [NUnit.Framework.Test]
        public virtual void CompareXmlsMixedContentDifferentFormatting() {
            String xml1 = "<Title>Text\n" + "  <Link>link</Link>\n" + "</Title>";
            String xml2 = "<Title>Text<Link>link</Link></Title>";
            NUnit.Framework.Assert.IsFalse(XmlUtils.CompareXmls(GetStream(xml1), GetStream(xml2)));
        }

        [NUnit.Framework.Test]
        public virtual void CompareXmlsDifferentTextContent() {
            NUnit.Framework.Assert.IsFalse(XmlUtils.CompareXmls(GetStream("<root><a>1</a></root>"), GetStream("<root><a>2</a></root>"
                )));
        }

        [NUnit.Framework.Test]
        public virtual void CompareXmlsEmptyElementsWithAttributes() {
            String xml1 = "<root><a x=\"1\"/></root>";
            String xml2 = "<root>\n  <a x=\"1\" />\n</root>";
            NUnit.Framework.Assert.IsTrue(XmlUtils.CompareXmls(GetStream(xml1), GetStream(xml2)));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyElementWithAttributesIsNotRemoved() {
            String xmlWithWhitespace = "<root>\n" + "  <a x=\"1\">   \n   </a>\n" + "</root>";
            String xmlExpected = "<root><a x=\"1\"/></root>";
            NUnit.Framework.Assert.IsTrue(XmlUtils.CompareXmls(GetStream(xmlWithWhitespace), GetStream(xmlExpected)));
        }

        [NUnit.Framework.Test]
        public virtual void SafeXmlDocumentTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => XmlUtils.InitXmlDocument(GetStream(
                XML_WITH_XXE)));
            NUnit.Framework.Assert.AreEqual(ExceptionTestUtil.GetDoctypeIsDisallowedExceptionMessage(), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InitNewXmlDocumentTest() {
            XmlDocument doc = XmlUtils.InitNewXmlDocument();
            NUnit.Framework.Assert.IsNotNull(doc);
        }

        private static Stream GetStream(String s) {
            return new MemoryStream(s.GetBytes(System.Text.Encoding.UTF8));
        }
    }
}
