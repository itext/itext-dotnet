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
using System;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Font.Otf;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class TextRendererTest : RendererUnitTest {
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
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FONT_PROPERTY_MUST_BE_PDF_FONT_OBJECT)]
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

        [NUnit.Framework.Test]
        public virtual void SetTextGlyphLineAndFontParamTest() {
            TextRenderer renderer = new TextRenderer(new Text("Some text"));
            String text = "\t";
            PdfFont pdfFont = PdfFontFactory.CreateFont();
            GlyphLine glyphLine = new GlyphLine();
            for (int i = 0; i < text.Length; i++) {
                int codePoint = iText.IO.Util.TextUtil.IsSurrogatePair(text, i) ? iText.IO.Util.TextUtil.ConvertToUtf32(text
                    , i) : (int)text[i];
                Glyph glyph = pdfFont.GetGlyph(codePoint);
                glyphLine.Add(glyph);
            }
            renderer.SetText(glyphLine, pdfFont);
            GlyphLine actualLine = renderer.GetText();
            NUnit.Framework.Assert.IsFalse(actualLine == glyphLine);
            Glyph glyph_1 = actualLine.Get(0);
            Glyph space = pdfFont.GetGlyph('\u0020');
            // Check that the glyph line has been processed using the replaceSpecialWhitespaceGlyphs method
            NUnit.Framework.Assert.AreEqual(space.GetCode(), glyph_1.GetCode());
            NUnit.Framework.Assert.AreEqual(space.GetWidth(), glyph_1.GetWidth());
        }

        [NUnit.Framework.Test]
        public virtual void SetTextGlyphLineAndPositionsParamTest() {
            TextRenderer renderer = new TextRenderer(new Text("Some text"));
            String text = "\tsome";
            PdfFont pdfFont = PdfFontFactory.CreateFont();
            GlyphLine glyphLine = new GlyphLine();
            for (int i = 0; i < text.Length; i++) {
                int codePoint = iText.IO.Util.TextUtil.IsSurrogatePair(text, i) ? iText.IO.Util.TextUtil.ConvertToUtf32(text
                    , i) : (int)text[i];
                Glyph glyph = pdfFont.GetGlyph(codePoint);
                glyphLine.Add(glyph);
            }
            renderer.SetText(new GlyphLine(), pdfFont);
            glyphLine.start = 1;
            glyphLine.end = 2;
            renderer.SetText(glyphLine, pdfFont);
            GlyphLine actualLine = renderer.GetText();
            NUnit.Framework.Assert.IsFalse(actualLine == glyphLine);
            Glyph glyph_1 = actualLine.Get(0);
            Glyph space = pdfFont.GetGlyph('\u0020');
            // Check that the glyph line has been processed using the replaceSpecialWhitespaceGlyphs method
            NUnit.Framework.Assert.AreEqual(space.GetCode(), glyph_1.GetCode());
            NUnit.Framework.Assert.AreEqual(space.GetWidth(), glyph_1.GetWidth());
            NUnit.Framework.Assert.AreEqual(1, actualLine.start);
            NUnit.Framework.Assert.AreEqual(2, actualLine.end);
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
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FONT_PROPERTY_MUST_BE_PDF_FONT_OBJECT)]
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
            Document doc = CreateDummyDocument();
            TextRenderer textRenderer = CreateLayoutedTextRenderer("hello", doc);
            textRenderer.SetProperty(Property.PADDING_TOP, UnitValue.CreatePointValue(20f));
            textRenderer.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(20f));
            NUnit.Framework.Assert.AreEqual(-2.980799674987793f, textRenderer.GetDescent(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void GetOccupiedAreaBBoxTest() {
            Document doc = CreateDummyDocument();
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
            Document doc = CreateDummyDocument();
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

        [NUnit.Framework.Test]
        public virtual void MyanmarCharacterBelongsToSpecificScripts() {
            // u1042 MYANMAR DIGIT TWO
            NUnit.Framework.Assert.IsTrue(TextRenderer.CodePointIsOfSpecialScript(4162));
        }

        [NUnit.Framework.Test]
        public virtual void ThaiCharacterBelongsToSpecificScripts() {
            // u0E19 THAI CHARACTER NO NU
            NUnit.Framework.Assert.IsTrue(TextRenderer.CodePointIsOfSpecialScript(3609));
        }

        [NUnit.Framework.Test]
        public virtual void LaoCharacterBelongsToSpecificScripts() {
            // u0EC8 LAO TONE MAI EK
            NUnit.Framework.Assert.IsTrue(TextRenderer.CodePointIsOfSpecialScript(3784));
        }

        [NUnit.Framework.Test]
        public virtual void KhmerCharacterBelongsToSpecificScripts() {
            // u1789 KHMER LETTER NYO
            NUnit.Framework.Assert.IsTrue(TextRenderer.CodePointIsOfSpecialScript(6025));
        }

        [NUnit.Framework.Test]
        public virtual void CyrillicCharacterDoesntBelongToSpecificScripts() {
            // u0433 Cyrillic Small Letter U
            NUnit.Framework.Assert.IsFalse(TextRenderer.CodePointIsOfSpecialScript(1091));
        }

        [NUnit.Framework.Test]
        public virtual void OverflowWrapAnywhereProperty() {
            Text text = new Text("wow");
            text.SetProperty(Property.OVERFLOW_WRAP, OverflowWrapPropertyValue.ANYWHERE);
            TextRenderer textRenderer = (TextRenderer)text.GetRenderer();
            textRenderer.SetParent(CreateDummyDocument().GetRenderer());
            MinMaxWidth minMaxWidth = textRenderer.GetMinMaxWidth();
            NUnit.Framework.Assert.IsTrue(minMaxWidth.GetMinWidth() < minMaxWidth.GetMaxWidth());
        }

        [NUnit.Framework.Test]
        public virtual void OverflowWrapBreakWordProperty() {
            Text text = new Text("wooow");
            TextRenderer textRenderer = (TextRenderer)text.GetRenderer();
            RootRenderer parentRenderer = CreateDummyDocument().GetRenderer();
            textRenderer.SetParent(parentRenderer);
            // overflow is set here to mock LineRenderer#layout behavior
            parentRenderer.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            float fullWordWidth = textRenderer.GetMinMaxWidth().GetMaxWidth();
            LayoutArea layoutArea = new LayoutArea(1, new Rectangle(fullWordWidth / 2, AbstractRenderer.INF));
            TextLayoutResult result = (TextLayoutResult)textRenderer.Layout(new LayoutContext(layoutArea));
            NUnit.Framework.Assert.IsFalse(result.IsWordHasBeenSplit());
            textRenderer.SetProperty(Property.OVERFLOW_WRAP, OverflowWrapPropertyValue.BREAK_WORD);
            result = (TextLayoutResult)textRenderer.Layout(new LayoutContext(layoutArea));
            NUnit.Framework.Assert.IsTrue(result.IsWordHasBeenSplit());
        }

        [NUnit.Framework.Test]
        public virtual void OverflowWrapAnywhereBoldSimulationMaxWidth() {
            Text text = new Text("wow");
            text.SetBold();
            TextRenderer textRenderer = (TextRenderer)text.GetRenderer();
            textRenderer.SetParent(CreateDummyDocument().GetRenderer());
            float maxWidthNoOverflowWrap = textRenderer.GetMinMaxWidth().GetMaxWidth();
            text.SetProperty(Property.OVERFLOW_WRAP, OverflowWrapPropertyValue.ANYWHERE);
            float maxWidthAndOverflowWrap = textRenderer.GetMinMaxWidth().GetMaxWidth();
            NUnit.Framework.Assert.AreEqual(maxWidthAndOverflowWrap, maxWidthNoOverflowWrap, 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void OverflowWrapAnywhereItalicSimulationMaxWidth() {
            Text text = new Text("wow");
            text.SetItalic();
            TextRenderer textRenderer = (TextRenderer)text.GetRenderer();
            textRenderer.SetParent(CreateDummyDocument().GetRenderer());
            float maxWidthNoOverflowWrap = textRenderer.GetMinMaxWidth().GetMaxWidth();
            text.SetProperty(Property.OVERFLOW_WRAP, OverflowWrapPropertyValue.ANYWHERE);
            float maxWidthAndOverflowWrap = textRenderer.GetMinMaxWidth().GetMaxWidth();
            NUnit.Framework.Assert.AreEqual(maxWidthAndOverflowWrap, maxWidthNoOverflowWrap, 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void OverflowWrapAnywhereBoldSimulationMinWidth() {
            Text text = new Text("wow");
            text.SetProperty(Property.OVERFLOW_WRAP, OverflowWrapPropertyValue.ANYWHERE);
            TextRenderer textRenderer = (TextRenderer)text.GetRenderer();
            textRenderer.SetParent(CreateDummyDocument().GetRenderer());
            float minWidthNoBoldSimulation = textRenderer.GetMinMaxWidth().GetMinWidth();
            text.SetBold();
            float minWidthAndBoldSimulation = textRenderer.GetMinMaxWidth().GetMinWidth();
            NUnit.Framework.Assert.IsTrue(minWidthAndBoldSimulation > minWidthNoBoldSimulation);
        }

        [NUnit.Framework.Test]
        public virtual void OverflowWrapAnywhereItalicSimulationMinWidth() {
            Text text = new Text("wow");
            text.SetProperty(Property.OVERFLOW_WRAP, OverflowWrapPropertyValue.ANYWHERE);
            TextRenderer textRenderer = (TextRenderer)text.GetRenderer();
            textRenderer.SetParent(CreateDummyDocument().GetRenderer());
            float minWidthNoItalicSimulation = textRenderer.GetMinMaxWidth().GetMinWidth();
            text.SetItalic();
            float minWidthAndItalicSimulation = textRenderer.GetMinMaxWidth().GetMinWidth();
            NUnit.Framework.Assert.IsTrue(minWidthAndItalicSimulation > minWidthNoItalicSimulation);
        }

        [NUnit.Framework.Test]
        public virtual void FloatingRightMinMaxWidth() {
            String longestWord = "float:right";
            String wholeText = "text with " + longestWord;
            TextRenderer textRenderer = new TextRenderer(new Text(wholeText));
            textRenderer.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            textRenderer.SetParent(CreateDummyDocument().GetRenderer());
            PdfFont font = PdfFontFactory.CreateFont();
            int fontSize = 12;
            textRenderer.SetProperty(Property.FONT, font);
            textRenderer.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(fontSize));
            float expectedMaxWidth = font.GetWidth(wholeText, fontSize);
            float expectedMinWidth = font.GetWidth(longestWord, fontSize);
            MinMaxWidth minMaxWidth = textRenderer.GetMinMaxWidth();
            NUnit.Framework.Assert.AreEqual(expectedMinWidth, minMaxWidth.GetMinWidth(), 0.01f);
            NUnit.Framework.Assert.AreEqual(expectedMaxWidth, minMaxWidth.GetMaxWidth(), 0.01f);
        }
    }
}
