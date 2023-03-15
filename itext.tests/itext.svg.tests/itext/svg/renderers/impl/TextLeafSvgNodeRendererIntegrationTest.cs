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
using iText.Kernel.Font;
using iText.Svg;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TextLeafSvgNodeRendererIntegrationTest : SvgIntegrationTest {
        [NUnit.Framework.Test]
        public virtual void GetContentLengthBaseTest() {
            TextLeafSvgNodeRenderer toTest = new TextLeafSvgNodeRenderer();
            toTest.SetAttribute(SvgConstants.Attributes.TEXT_CONTENT, "Hello");
            toTest.SetAttribute(SvgConstants.Attributes.FONT_SIZE, "10");
            PdfFont font = PdfFontFactory.CreateFont();
            float actual = toTest.GetTextContentLength(12, font);
            float expected = 17.085f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 1e-6f);
        }

        [NUnit.Framework.Test]
        public virtual void GetContentLengthNoValueTest() {
            TextLeafSvgNodeRenderer toTest = new TextLeafSvgNodeRenderer();
            toTest.SetAttribute(SvgConstants.Attributes.TEXT_CONTENT, "Hello");
            PdfFont font = PdfFontFactory.CreateFont();
            float actual = toTest.GetTextContentLength(12, font);
            float expected = 27.336f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 1e-6f);
        }

        [NUnit.Framework.Test]
        public virtual void GetContentLengthNaNTest() {
            TextLeafSvgNodeRenderer toTest = new TextLeafSvgNodeRenderer();
            toTest.SetAttribute(SvgConstants.Attributes.TEXT_CONTENT, "Hello");
            toTest.SetAttribute(SvgConstants.Attributes.FONT_SIZE, "spice");
            PdfFont font = PdfFontFactory.CreateFont();
            float actual = toTest.GetTextContentLength(12, font);
            float expected = 0.0f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 1e-6f);
        }

        [NUnit.Framework.Test]
        public virtual void GetContentLengthNegativeTest() {
            TextLeafSvgNodeRenderer toTest = new TextLeafSvgNodeRenderer();
            toTest.SetAttribute(SvgConstants.Attributes.TEXT_CONTENT, "Hello");
            toTest.SetAttribute(SvgConstants.Attributes.FONT_SIZE, "-10");
            PdfFont font = PdfFontFactory.CreateFont();
            float actual = toTest.GetTextContentLength(12, font);
            float expected = 27.336f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 1e-6f);
        }
    }
}
