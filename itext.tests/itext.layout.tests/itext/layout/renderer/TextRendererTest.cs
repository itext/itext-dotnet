/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.IO;
using iText.IO.Util;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    public class TextRendererTest : AbstractRendererUnitTest {
        private static readonly String FONTS_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        private const double EPS = 1e-5;

        [NUnit.Framework.Test]
        public virtual void NextRendererTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            pdfDoc.AddNewPage();
            Document doc = new Document(pdfDoc);
            RootRenderer documentRenderer = doc.GetRenderer();
            Text text = new Text("hello");
            text.SetNextRenderer(new TextRenderer(text));
            IRenderer textRenderer1 = text.GetRenderer().SetParent(documentRenderer);
            IRenderer textRenderer2 = text.GetRenderer().SetParent(documentRenderer);
            LayoutArea area = new LayoutArea(1, new Rectangle(100, 100, 100, 100));
            LayoutContext layoutContext = new LayoutContext(area);
            doc.Close();
            LayoutResult result1 = textRenderer1.Layout(layoutContext);
            LayoutResult result2 = textRenderer2.Layout(layoutContext);
            NUnit.Framework.Assert.AreEqual(result1.GetOccupiedArea(), result2.GetOccupiedArea());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FONT_PROPERTY_MUST_BE_PDF_FONT_OBJECT)]
        public virtual void SetTextException() {
            String val = "other text";
            String fontName = "Helvetica";
            TextRenderer rend = (TextRenderer)new Text("basic text").GetRenderer();
            FontProvider fp = new FontProvider();
            fp.AddFont(fontName);
            rend.SetProperty(Property.FONT_PROVIDER, fp);
            rend.SetProperty(Property.FONT, new String[] { fontName });
            rend.SetText(val);
            NUnit.Framework.Assert.AreEqual(val, rend.GetText().ToString());
        }

        /// <summary>
        /// This test assumes that absolute positioning for
        /// <see cref="iText.Layout.Element.Text"/>
        /// elements is
        /// not supported.
        /// </summary>
        /// <remarks>
        /// This test assumes that absolute positioning for
        /// <see cref="iText.Layout.Element.Text"/>
        /// elements is
        /// not supported. Adding this support is the subject of DEVSIX-1393.
        /// </remarks>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FONT_PROPERTY_MUST_BE_PDF_FONT_OBJECT)]
        public virtual void SetFontAsText() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            pdfDoc.AddNewPage();
            Document doc = new Document(pdfDoc);
            Text txt = new Text("text");
            txt.SetProperty(Property.POSITION, LayoutPosition.ABSOLUTE);
            txt.SetProperty(Property.TOP, 5f);
            FontProvider fp = new FontProvider();
            fp.AddFont("Helvetica");
            txt.SetProperty(Property.FONT_PROVIDER, fp);
            txt.SetFontFamily("Helvetica");
            doc.Add(new Paragraph().Add(txt));
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void GetDescentTest() {
            Document doc = CreateDocument();
            TextRenderer textRenderer = CreateLayoutedTextRenderer("hello", doc);
            textRenderer.SetProperty(Property.PADDING_TOP, UnitValue.CreatePointValue(20f));
            textRenderer.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(20f));
            NUnit.Framework.Assert.AreEqual(-2.980799674987793f, textRenderer.GetDescent(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void GetOccupiedAreaBBoxTest() {
            Document doc = CreateDocument();
            TextRenderer textRenderer = CreateLayoutedTextRenderer("hello", doc);
            textRenderer.SetProperty(Property.PADDING_TOP, UnitValue.CreatePointValue(20f));
            textRenderer.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(20f));
            textRenderer.SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePointValue(20f));
            textRenderer.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(0, 986.68f, 25.343998f, 13.32f).EqualsWithEpsilon(textRenderer
                .GetOccupiedAreaBBox()));
        }

        [NUnit.Framework.Test]
        public virtual void GetInnerAreaBBoxTest() {
            Document doc = CreateDocument();
            TextRenderer textRenderer = CreateLayoutedTextRenderer("hello", doc);
            textRenderer.SetProperty(Property.PADDING_TOP, UnitValue.CreatePointValue(20f));
            textRenderer.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(20f));
            textRenderer.SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePointValue(20f));
            textRenderer.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(0, 986.68f, 5.343998f, -26.68f).EqualsWithEpsilon(textRenderer
                .GetInnerAreaBBox()));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveFirstPdfFontWithGlyphsAvailableOnlyInSecondaryFont() {
            // Test that in TextRenderer the #resolveFirstPdfFont method is overloaded in such way
            // that yielded font contains at least some of the glyphs for the text characters.
            Text text = new Text("\u043A\u0456\u0440\u044B\u043B\u0456\u0446\u0430");
            // "кірыліца"
            // Puritan doesn't contain cyrillic symbols, while Noto Sans does.
            text.SetFontFamily(JavaUtil.ArraysAsList("Puritan 2.0", "Noto Sans"));
            FontProvider fontProvider = new FontProvider();
            fontProvider.AddFont(FONTS_FOLDER + "Puritan2.otf");
            fontProvider.AddFont(FONTS_FOLDER + "NotoSans-Regular.ttf");
            text.SetProperty(Property.FONT_PROVIDER, fontProvider);
            TextRenderer renderer = (TextRenderer)new TextRenderer(text);
            PdfFont pdfFont = renderer.ResolveFirstPdfFont();
            NUnit.Framework.Assert.AreEqual("NotoSans", pdfFont.GetFontProgram().GetFontNames().GetFontName());
        }
    }
}
