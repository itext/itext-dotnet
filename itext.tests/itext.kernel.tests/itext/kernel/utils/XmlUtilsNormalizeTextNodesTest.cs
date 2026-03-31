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
using iText.Test;

namespace iText.Kernel.Utils {
    public class XmlUtilsNormalizeTextNodesTest : ExtendedITextTest {
        private static System.IO.Stream Stream(String s) {
            return new MemoryStream(s.GetBytes(System.Text.Encoding.UTF8));
        }

        [NUnit.Framework.Test]
        public virtual void CompareXmlsSameStructureDifferentWhitespace() {
            String pretty = "<root>\n" + "  <a>1</a>\n" + "  <b>2</b>\n" + "</root>";
            String compact = "<root><a>1</a><b>2</b></root>";
            NUnit.Framework.Assert.IsTrue(XmlUtils.CompareXmls(Stream(pretty), Stream(compact)));
        }

        [NUnit.Framework.Test]
        public virtual void CompareXmlsMixedContentDifferentFormatting() {
            String xml1 = "<Title>Text\n" + "  <Link>link</Link>\n" + "</Title>";
            String xml2 = "<Title>Text<Link>link</Link></Title>";
            NUnit.Framework.Assert.IsFalse(XmlUtils.CompareXmls(Stream(xml1), Stream(xml2)));
        }

        [NUnit.Framework.Test]
        public virtual void CompareXmlsDifferentTextContent() {
            NUnit.Framework.Assert.IsFalse(XmlUtils.CompareXmls(Stream("<root><a>1</a></root>"), Stream("<root><a>2</a></root>"
                )));
        }

        [NUnit.Framework.Test]
        public virtual void CompareXmlsEmptyElementsWithAttributes() {
            String xml1 = "<root><a x=\"1\"/></root>";
            String xml2 = "<root>\n  <a x=\"1\" />\n</root>";
            NUnit.Framework.Assert.IsTrue(XmlUtils.CompareXmls(Stream(xml1), Stream(xml2)));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyElementWithAttributesIsNotRemoved() {
            String xmlWithWhitespace = "<root>\n" + "  <a x=\"1\">   \n   </a>\n" + "</root>";
            String xmlExpected = "<root><a x=\"1\"/></root>";
            NUnit.Framework.Assert.IsTrue(XmlUtils.CompareXmls(Stream(xmlWithWhitespace), Stream(xmlExpected)));
        }
    }
}
