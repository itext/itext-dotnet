/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.IO.Font.Constants;
using iText.IO.Source;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class LineHeightHelperUnitTest : ExtendedITextTest {
        private const double EPS = 1e-5;

        private static readonly String FONTS = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        private static readonly String OPEN_SANS_FONTS = FONTS + "Open_Sans/";

        [NUnit.Framework.Test]
        public virtual void CalculateFontAscenderDescenderFromFontMetricsCourierTest() {
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
            float[] fontAscenderDescender = TextRenderer.CalculateAscenderDescender(font, RenderingMode.HTML_MODE);
            NUnit.Framework.Assert.AreEqual(629.0f * TextRenderer.TYPO_ASCENDER_SCALE_COEFF, fontAscenderDescender[0], 
                EPS);
            NUnit.Framework.Assert.AreEqual(-157.0f * TextRenderer.TYPO_ASCENDER_SCALE_COEFF, fontAscenderDescender[1]
                , EPS);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateFontAscenderDescenderFromFontMetricsTimesTest() {
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
            float[] fontAscenderDescender = TextRenderer.CalculateAscenderDescender(font, RenderingMode.HTML_MODE);
            NUnit.Framework.Assert.AreEqual(683.0f * TextRenderer.TYPO_ASCENDER_SCALE_COEFF, fontAscenderDescender[0], 
                EPS);
            NUnit.Framework.Assert.AreEqual(-217.0f * TextRenderer.TYPO_ASCENDER_SCALE_COEFF, fontAscenderDescender[1]
                , EPS);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateFontAscenderDescenderFromFontMetricsHelveticaTest() {
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            float[] fontAscenderDescender = TextRenderer.CalculateAscenderDescender(font, RenderingMode.HTML_MODE);
            NUnit.Framework.Assert.AreEqual(718.0f * TextRenderer.TYPO_ASCENDER_SCALE_COEFF, fontAscenderDescender[0], 
                EPS);
            NUnit.Framework.Assert.AreEqual(-207.0f * TextRenderer.TYPO_ASCENDER_SCALE_COEFF, fontAscenderDescender[1]
                , EPS);
        }

        [NUnit.Framework.Test]
        public virtual void GetFontAscenderDescenderNormalizedTextRendererTest() {
            Document document = new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())));
            TextRenderer textRenderer = new TextRenderer(new Text("Hello"));
            textRenderer.SetParent(document.GetRenderer());
            float[] ascenderDescender = LineHeightHelper.GetFontAscenderDescenderNormalized(textRenderer);
            NUnit.Framework.Assert.AreEqual(10.33920f, ascenderDescender[0], EPS);
            NUnit.Framework.Assert.AreEqual(-2.9808f, ascenderDescender[1], EPS);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateLineHeightTextRendererNullTest() {
            Document document = new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())));
            TextRenderer textRenderer = new TextRenderer(new Text("Hello"));
            textRenderer.SetParent(document.GetRenderer());
            float lineHeight = LineHeightHelper.CalculateLineHeight(textRenderer);
            NUnit.Framework.Assert.AreEqual(13.79999f, lineHeight, EPS);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateLineHeightTextRendererNormalTest() {
            Document document = new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())));
            TextRenderer textRenderer = new TextRenderer(new Text("Hello"));
            textRenderer.SetProperty(Property.LINE_HEIGHT, LineHeight.CreateNormalValue());
            textRenderer.SetParent(document.GetRenderer());
            float lineHeight = LineHeightHelper.CalculateLineHeight(textRenderer);
            NUnit.Framework.Assert.AreEqual(13.79999f, lineHeight, EPS);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateLineHeightTextRendererPointNegativeTest() {
            Document document = new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())));
            TextRenderer textRenderer = new TextRenderer(new Text("Hello"));
            textRenderer.SetProperty(Property.LINE_HEIGHT, LineHeight.CreateFixedValue(-10));
            textRenderer.SetParent(document.GetRenderer());
            float lineHeight = LineHeightHelper.CalculateLineHeight(textRenderer);
            NUnit.Framework.Assert.AreEqual(13.79999f, lineHeight, EPS);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateLineHeightTextRendererNormalNegativeTest() {
            Document document = new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())));
            TextRenderer textRenderer = new TextRenderer(new Text("Hello"));
            textRenderer.SetProperty(Property.LINE_HEIGHT, LineHeight.CreateNormalValue());
            textRenderer.SetParent(document.GetRenderer());
            float lineHeight = LineHeightHelper.CalculateLineHeight(textRenderer);
            NUnit.Framework.Assert.AreEqual(13.79999f, lineHeight, EPS);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateLineHeighttTextRendererPointZeroTest() {
            Document document = new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())));
            TextRenderer textRenderer = new TextRenderer(new Text("Hello"));
            textRenderer.SetProperty(Property.LINE_HEIGHT, LineHeight.CreateFixedValue(0));
            textRenderer.SetParent(document.GetRenderer());
            float lineHeight = LineHeightHelper.CalculateLineHeight(textRenderer);
            NUnit.Framework.Assert.AreEqual(0f, lineHeight, EPS);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateLineHeighttTextRendererNormalZeroTest() {
            Document document = new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())));
            TextRenderer textRenderer = new TextRenderer(new Text("Hello"));
            textRenderer.SetProperty(Property.LINE_HEIGHT, LineHeight.CreateNormalValue());
            textRenderer.SetParent(document.GetRenderer());
            float lineHeight = LineHeightHelper.CalculateLineHeight(textRenderer);
            NUnit.Framework.Assert.AreEqual(13.79999f, lineHeight, EPS);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateLineHeighttTextRendererPointTest() {
            Document document = new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())));
            TextRenderer textRenderer = new TextRenderer(new Text("Hello"));
            textRenderer.SetProperty(Property.LINE_HEIGHT, LineHeight.CreateFixedValue(200));
            textRenderer.SetParent(document.GetRenderer());
            float lineHeight = LineHeightHelper.CalculateLineHeight(textRenderer);
            NUnit.Framework.Assert.AreEqual(200, lineHeight, EPS);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateLineHeightTextRendererNormalAscenderDescenderSumForNotoSansFontTest() {
            Document document = new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())));
            PdfFont font = PdfFontFactory.CreateFont(FONTS + "NotoSans-Regular.ttf");
            TextRenderer textRenderer = new TextRenderer(new Text("Hello"));
            textRenderer.SetProperty(Property.FONT, font);
            textRenderer.SetProperty(Property.LINE_HEIGHT, LineHeight.CreateNormalValue());
            textRenderer.SetParent(document.GetRenderer());
            float lineHeight = LineHeightHelper.CalculateLineHeight(textRenderer);
            NUnit.Framework.Assert.AreEqual(16.31999f, lineHeight, EPS);
        }

        [NUnit.Framework.Test]
        public virtual void GetActualAscenderDescenderTextRenderer() {
            Document document = new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())));
            TextRenderer textRenderer = new TextRenderer(new Text("Hello"));
            textRenderer.SetParent(document.GetRenderer());
            float[] ascenderDescender = LineHeightHelper.GetActualAscenderDescender(textRenderer);
            NUnit.Framework.Assert.AreEqual(10.57919f, ascenderDescender[0], EPS);
            NUnit.Framework.Assert.AreEqual(-3.22079f, ascenderDescender[1], EPS);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateFontAscenderDescenderFromFontMetricsNotoEmojiFontTest() {
            PdfFont font = PdfFontFactory.CreateFont(FONTS + "NotoEmoji-Regular.ttf");
            float[] ascenderDescenderFromFontMetrics = TextRenderer.CalculateAscenderDescender(font, RenderingMode.HTML_MODE
                );
            NUnit.Framework.Assert.AreEqual(1068.0f, ascenderDescenderFromFontMetrics[0], EPS);
            NUnit.Framework.Assert.AreEqual(-292.0f, ascenderDescenderFromFontMetrics[1], EPS);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateFontAscenderDescenderFromFontMetricsNotoSansFontTest() {
            PdfFont font = PdfFontFactory.CreateFont(FONTS + "NotoSans-Regular.ttf");
            float[] ascenderDescenderFromFontMetrics = TextRenderer.CalculateAscenderDescender(font, RenderingMode.HTML_MODE
                );
            NUnit.Framework.Assert.AreEqual(1068.0f, ascenderDescenderFromFontMetrics[0], EPS);
            NUnit.Framework.Assert.AreEqual(-292.0f, ascenderDescenderFromFontMetrics[1], EPS);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateFontAscenderDescenderFromFontMetricsNotoColorEmojiFontTest() {
            PdfFont font = PdfFontFactory.CreateFont(FONTS + "NotoColorEmoji.ttf");
            float[] ascenderDescenderFromFontMetrics = TextRenderer.CalculateAscenderDescender(font, RenderingMode.HTML_MODE
                );
            System.Console.Out.WriteLine(ascenderDescenderFromFontMetrics[0]);
            System.Console.Out.WriteLine(ascenderDescenderFromFontMetrics[1]);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateFontAscenderDescenderFromFontMetricsNotoSansCJKscRegularFontTest() {
            PdfFont font = PdfFontFactory.CreateFont(FONTS + "NotoSansCJKsc-Regular.otf");
            float[] ascenderDescenderFromFontMetrics = TextRenderer.CalculateAscenderDescender(font, RenderingMode.HTML_MODE
                );
            NUnit.Framework.Assert.AreEqual(1160.0f, ascenderDescenderFromFontMetrics[0], EPS);
            NUnit.Framework.Assert.AreEqual(-320.0f, ascenderDescenderFromFontMetrics[1], EPS);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateFontAscenderDescenderFromFontMetricsPuritan2FontTest() {
            PdfFont font = PdfFontFactory.CreateFont(FONTS + "Puritan2.otf");
            float[] ascenderDescenderFromFontMetrics = TextRenderer.CalculateAscenderDescender(font, RenderingMode.HTML_MODE
                );
            NUnit.Framework.Assert.AreEqual(860.0f, ascenderDescenderFromFontMetrics[0], EPS);
            NUnit.Framework.Assert.AreEqual(-232.0f, ascenderDescenderFromFontMetrics[1], EPS);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateFontAscenderDescenderFromFontMetricsNotoSansCJKjpBoldFontTest() {
            PdfFont font = PdfFontFactory.CreateFont(FONTS + "NotoSansCJKjp-Bold.otf");
            float[] ascenderDescenderFromFontMetrics = TextRenderer.CalculateAscenderDescender(font, RenderingMode.HTML_MODE
                );
            NUnit.Framework.Assert.AreEqual(1160.0f, ascenderDescenderFromFontMetrics[0], EPS);
            NUnit.Framework.Assert.AreEqual(-320.0f, ascenderDescenderFromFontMetrics[1], EPS);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateFontAscenderDescenderFromFontMetricsFreeSansFontTest() {
            PdfFont font = PdfFontFactory.CreateFont(FONTS + "FreeSans.ttf");
            float[] ascenderDescenderFromFontMetrics = TextRenderer.CalculateAscenderDescender(font, RenderingMode.HTML_MODE
                );
            NUnit.Framework.Assert.AreEqual(800.0f, ascenderDescenderFromFontMetrics[0], EPS);
            NUnit.Framework.Assert.AreEqual(-200.0f, ascenderDescenderFromFontMetrics[1], EPS);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateFontAscenderDescenderFromFontMetricsOpenSansRegularFontTest() {
            PdfFont font = PdfFontFactory.CreateFont(OPEN_SANS_FONTS + "OpenSans-Regular.ttf");
            float[] ascenderDescenderFromFontMetrics = TextRenderer.CalculateAscenderDescender(font, RenderingMode.HTML_MODE
                );
            NUnit.Framework.Assert.AreEqual(1068.0f, ascenderDescenderFromFontMetrics[0], EPS);
            NUnit.Framework.Assert.AreEqual(-292.0f, ascenderDescenderFromFontMetrics[1], EPS);
        }
    }
}
