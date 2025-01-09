/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Text;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TextRendererIntegrationTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/TextRendererIntegrationTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/TextRendererIntegrationTest/";

        public static readonly String fontsFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void TrimFirstJapaneseCharactersTest() {
            String outFileName = destinationFolder + "trimFirstJapaneseCharacters.pdf";
            String cmpFileName = sourceFolder + "cmp_trimFirstJapaneseCharacters.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            // UTF-8 encoding table and Unicode characters
            byte[] bUtf16A = new byte[] { (byte)0xd8, (byte)0x40, (byte)0xdc, (byte)0x0b };
            // This String is U+2000B
            String strUtf16A = iText.Commons.Utils.JavaUtil.GetStringForBytes(bUtf16A, "UTF-16BE");
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoSansCJKjp-Bold.otf", PdfEncodings.IDENTITY_H);
            doc.Add(new Paragraph(strUtf16A).SetFont(font));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void WordSplitAcrossTwoTextRenderers() {
            String outFileName = destinationFolder + "wordSplitAcrossTwoTextRenderers.pdf";
            String cmpFileName = sourceFolder + "cmp_wordSplitAcrossTwoTextRenderers.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            doc.SetFontSize(20);
            Text placeho = new Text("placeho").SetFontColor(ColorConstants.MAGENTA).SetBackgroundColor(ColorConstants.
                YELLOW);
            Text lderInte = new Text("lder inte").SetFontColor(ColorConstants.RED).SetBackgroundColor(ColorConstants.YELLOW
                );
            Text gration = new Text("gration").SetFontColor(ColorConstants.ORANGE).SetBackgroundColor(ColorConstants.YELLOW
                );
            Paragraph fullWord = new Paragraph().Add(placeho).Add(lderInte).Add(gration).SetWidth(160).SetTextAlignment
                (TextAlignment.RIGHT).SetBorder(new SolidBorder(1));
            fullWord.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            doc.Add(fullWord);
            fullWord.DeleteOwnProperty(Property.RENDERING_MODE);
            doc.Add(fullWord);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void WordSplitAcrossMultipleRenderers() {
            String outFileName = destinationFolder + "wordSplitAcrossMultipleRenderers.pdf";
            String cmpFileName = sourceFolder + "cmp_wordSplitAcrossMultipleRenderers.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            doc.SetFontSize(20);
            Text placeho = new Text("placeho").SetFontColor(ColorConstants.MAGENTA).SetBackgroundColor(ColorConstants.
                YELLOW);
            Text lderIn = new Text("lder-in").SetFontColor(ColorConstants.RED).SetBackgroundColor(ColorConstants.YELLOW
                );
            Text te = new Text("te").SetFontColor(ColorConstants.ORANGE).SetBackgroundColor(ColorConstants.YELLOW);
            Text gra = new Text("gra").SetFontColor(ColorConstants.GREEN).SetBackgroundColor(ColorConstants.YELLOW);
            Text tionLoooooooooooooooong = new Text("tion loooooooooooooooong").SetFontColor(ColorConstants.BLUE).SetBackgroundColor
                (ColorConstants.YELLOW);
            Paragraph fullWord = new Paragraph().Add(placeho).Add(lderIn).Add(te).Add(gra).Add(tionLoooooooooooooooong
                ).SetWidth(180).SetTextAlignment(TextAlignment.RIGHT).SetBorder(new SolidBorder(1));
            fullWord.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            doc.Add(fullWord);
            fullWord.DeleteOwnProperty(Property.RENDERING_MODE);
            doc.Add(fullWord);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void WordEndsAndFollowingTextRendererStartsWithWhitespaces01() {
            String outFileName = destinationFolder + "wordEndsAndFollowingTextRendererStartsWithWhitespaces01.pdf";
            String cmpFileName = sourceFolder + "cmp_wordEndsAndFollowingTextRendererStartsWithWhitespaces01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            doc.SetFontSize(20);
            Text firstText = new Text("firstTextRenderer").SetFontColor(ColorConstants.MAGENTA).SetBackgroundColor(ColorConstants
                .YELLOW);
            Text whitespaces = new Text("  ").SetFontColor(ColorConstants.RED).SetBackgroundColor(ColorConstants.YELLOW
                );
            Text secondText = new Text("      secondTextRenderer").SetFontColor(ColorConstants.RED).SetBackgroundColor
                (ColorConstants.YELLOW);
            Paragraph paragraph = new Paragraph().Add(firstText).Add(whitespaces).Add(secondText).SetWidth(175).SetTextAlignment
                (TextAlignment.RIGHT).SetBorder(new SolidBorder(1));
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void WordEndsAndFollowingTextRendererStartsWithWhitespaces02() {
            String outFileName = destinationFolder + "wordEndsAndFollowingTextRendererStartsWithWhitespaces02.pdf";
            String cmpFileName = sourceFolder + "cmp_wordEndsAndFollowingTextRendererStartsWithWhitespaces02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            doc.SetFontSize(20);
            Text firstText = new Text("firstTextRenderer").SetFontColor(ColorConstants.MAGENTA).SetBackgroundColor(ColorConstants
                .YELLOW);
            Text secondText = new Text("      secondTextRenderer").SetFontColor(ColorConstants.RED).SetBackgroundColor
                (ColorConstants.YELLOW);
            Paragraph paragraph = new Paragraph().Add(firstText).Add(secondText).SetWidth(175).SetTextAlignment(TextAlignment
                .RIGHT).SetBorder(new SolidBorder(1));
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ForcedWordSplit() {
            String outFileName = destinationFolder + "forcedWordSplit.pdf";
            String cmpFileName = sourceFolder + "cmp_forcedWordSplit.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            doc.SetFontSize(20);
            Text forcedWordSplit = new Text("forcedWordSplit forcedWordSplit").SetFontColor(ColorConstants.MAGENTA).SetBackgroundColor
                (ColorConstants.YELLOW);
            //ColumnDocumentRenderer is applied here only to fit content on one page
            doc.SetRenderer(new ColumnDocumentRenderer(doc, new Rectangle[] { new Rectangle(10, 10, 70, 800), new Rectangle
                (90, 10, 100, 800), new Rectangle(210, 10, 130, 800), new Rectangle(360, 10, 250, 800) }));
            Paragraph paragraph = new Paragraph().Add(forcedWordSplit).SetTextAlignment(TextAlignment.RIGHT).SetBorder
                (new SolidBorder(1));
            for (int i = 50; i <= 150; i += 5) {
                paragraph.SetWidth(i);
                doc.Add(paragraph);
            }
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void WordSplitAcrossMultipleRenderersOverflowXVisibleWithPrecedingPlaceholder() {
            String outFileName = destinationFolder + "wordSplitAcrossMultipleRenderersOverflowXVisibleWithPrecedingPlaceholder.pdf";
            String cmpFileName = sourceFolder + "cmp_wordSplitAcrossMultipleRenderersOverflowXVisibleWithPrecedingPlaceholder.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            doc.SetFontSize(20);
            Text placeholder = new Text("placeholder ").SetFontColor(ColorConstants.MAGENTA).SetBackgroundColor(ColorConstants
                .YELLOW);
            Text oooooooooover = new Text("oooooooooover").SetFontColor(ColorConstants.RED).SetBackgroundColor(ColorConstants
                .YELLOW);
            Text flooooo = new Text("flooooo").SetFontColor(ColorConstants.RED).SetBackgroundColor(ColorConstants.YELLOW
                );
            Text ooooowNextWords = new Text("ooooow next words").SetFontColor(ColorConstants.RED).SetBackgroundColor(ColorConstants
                .YELLOW);
            Paragraph paragraph = new Paragraph().Add(placeholder).Add(oooooooooover).Add(flooooo).Add(ooooowNextWords
                ).SetTextAlignment(TextAlignment.RIGHT).SetBorder(new SolidBorder(1)).SetWidth(135);
            paragraph.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            paragraph.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void WordSplitAcrossMultipleRenderersOverflowXVisible() {
            String outFileName = destinationFolder + "wordSplitAcrossMultipleRenderersOverflowXVisible.pdf";
            String cmpFileName = sourceFolder + "cmp_wordSplitAcrossMultipleRenderersOverflowXVisible.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            doc.SetFontSize(20);
            Text oooooooooover = new Text("oooooooooover").SetFontColor(ColorConstants.RED).SetBackgroundColor(ColorConstants
                .YELLOW);
            Text flooooo = new Text("flooooo").SetFontColor(ColorConstants.RED).SetBackgroundColor(ColorConstants.YELLOW
                );
            Text ooooowNextWords = new Text("ooooow next words").SetFontColor(ColorConstants.RED).SetBackgroundColor(ColorConstants
                .YELLOW);
            Paragraph paragraph = new Paragraph().Add(oooooooooover).Add(flooooo).Add(ooooowNextWords).SetTextAlignment
                (TextAlignment.RIGHT).SetBorder(new SolidBorder(1));
            paragraph.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            paragraph.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            paragraph.SetWidth(60);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void WordSplitRenderersWithFittingFloatingElementInBetween() {
            String outFileName = destinationFolder + "wordSplitRenderersWithFittingFloatingElementInBetween.pdf";
            String cmpFileName = sourceFolder + "cmp_wordSplitRenderersWithFittingFloatingElementInBetween.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            doc.SetFontSize(20);
            Text reg = new Text("reg").SetFontColor(ColorConstants.LIGHT_GRAY);
            Text ul = new Text("ul").SetFontColor(ColorConstants.DARK_GRAY);
            Text aaaaaaaaaaaaaaaaaaaaaaati = new Text("aaaaaaaaaaaaaaaaaaaaaaati").SetFontColor(ColorConstants.GRAY);
            Text ngAndRestOfText = new Text("ng overflow text renderers with floating elements between them").SetFontColor
                (ColorConstants.RED);
            Div floatDiv = new Div().SetWidth(20).SetHeight(60).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetBorder
                (new SolidBorder(2));
            floatDiv.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            Paragraph p = new Paragraph().Add(reg).Add(ul).Add(floatDiv).Add(aaaaaaaaaaaaaaaaaaaaaaati).Add(ngAndRestOfText
                ).SetBackgroundColor(ColorConstants.CYAN).SetWidth(150).SetBorder(new SolidBorder(1));
            p.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            doc.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void WordSplitRenderersWithNotFittingFloatingElementInBetween() {
            String outFileName = destinationFolder + "wordSplitRenderersWithNotFittingFloatingElementInBetween.pdf";
            String cmpFileName = sourceFolder + "cmp_wordSplitRenderersWithNotFittingFloatingElementInBetween.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            doc.SetFontSize(20);
            Text loooooooooooo = new Text("loooooooooooo").SetFontColor(ColorConstants.GREEN);
            Text oooongWords = new Text("oooong words").SetFontColor(ColorConstants.BLUE);
            Text floating = new Text("floating").SetFontColor(ColorConstants.RED);
            floating.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            Paragraph paragraph = new Paragraph().Add(loooooooooooo).Add(floating).Add(oooongWords).SetBackgroundColor
                (ColorConstants.YELLOW).SetWidth(150).SetBorder(new SolidBorder(1));
            paragraph.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            paragraph.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void WordSplitRenderersWithFittingFloatingInBetweenInSecondWord() {
            String outFileName = destinationFolder + "wordSplitRenderersWithFittingFloatingInBetweenInSecondWord.pdf";
            String cmpFileName = sourceFolder + "cmp_wordSplitRenderersWithFittingFloatingInBetweenInSecondWord.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            doc.SetFontSize(20);
            Text itsAndSpace = new Text("It's ");
            Text reg = new Text("reg").SetFontColor(ColorConstants.LIGHT_GRAY);
            Text ul = new Text("ul").SetFontColor(ColorConstants.DARK_GRAY);
            Text aaaaaaaaaaaaaaaaaaaaaaati = new Text("aaaaaaaaaaaaaaaaaaaaaaati").SetFontColor(ColorConstants.GRAY);
            Text ngAndRestOfText = new Text("ng overflow text renderers with floating elements between them").SetFontColor
                (ColorConstants.RED);
            Div floatDiv = new Div().SetWidth(20).SetHeight(60).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetBorder
                (new SolidBorder(2));
            floatDiv.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            Paragraph p = new Paragraph().Add(itsAndSpace).Add(reg).Add(ul).Add(floatDiv).Add(aaaaaaaaaaaaaaaaaaaaaaati
                ).Add(ngAndRestOfText).SetBackgroundColor(ColorConstants.CYAN).SetWidth(150).SetBorder(new SolidBorder
                (1));
            p.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            doc.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void WordSplitRenderersWithOverflowedFloatingElementInBetween() {
            String outFileName = destinationFolder + "wordSplitRenderersWithOverflowedFloatingElementInBetween.pdf";
            String cmpFileName = sourceFolder + "cmp_wordSplitRenderersWithOverflowedFloatingElementInBetween.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            doc.SetFontSize(20);
            Text reg = new Text("reg").SetFontColor(ColorConstants.LIGHT_GRAY);
            Text ul = new Text("ul").SetFontColor(ColorConstants.DARK_GRAY);
            Text aaaaaaaaaaaaaaaaaaaaaaati = new Text("aaaaaaaaaaaaaaaaaaaaaaati").SetFontColor(ColorConstants.GRAY);
            Text ngAndRestOfText = new Text("ng overflow text renderers with floating elements between them").SetFontColor
                (ColorConstants.RED);
            Div floatDiv = new Div().SetWidth(20).SetHeight(60).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetBorder
                (new SolidBorder(2));
            floatDiv.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            Paragraph p = new Paragraph().Add(reg).Add(ul).Add(aaaaaaaaaaaaaaaaaaaaaaati).Add(floatDiv).Add(ngAndRestOfText
                ).SetBackgroundColor(ColorConstants.CYAN).SetWidth(150).SetBorder(new SolidBorder(1));
            p.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            doc.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void WordSplitAcrossMutipleTextRenderersWithinFloatingContainer() {
            String outFileName = destinationFolder + "wordSplitAcrossMutipleTextRenderersWithinFloatingContainer.pdf";
            String cmpFileName = sourceFolder + "cmp_wordSplitAcrossMutipleTextRenderersWithinFloatingContainer.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            doc.SetFontSize(20);
            Text oooooooooover = new Text("oooooooooover").SetFontColor(ColorConstants.LIGHT_GRAY);
            Text flooooo = new Text("flooooo").SetFontColor(ColorConstants.GRAY);
            Text ooooowNextWords = new Text("ooooow next words").SetFontColor(ColorConstants.DARK_GRAY);
            Paragraph floatingParagraph = new Paragraph().Add(oooooooooover).Add(flooooo).Add(ooooowNextWords).SetBackgroundColor
                (ColorConstants.CYAN).SetWidth(150).SetBorder(new SolidBorder(1));
            floatingParagraph.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            floatingParagraph.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            Text regularText = new Text("regular words regular words regular words regular words regular words regular "
                 + "words regular words regular words regular words");
            Paragraph regularParagraph = new Paragraph(regularText).SetBackgroundColor(ColorConstants.MAGENTA);
            Div div = new Div().Add(floatingParagraph).Add(regularParagraph).SetMaxWidth(300).SetHeight(300).SetBackgroundColor
                (ColorConstants.YELLOW);
            div.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void WordSplitAcrossRenderersWithPrecedingImageRenderer() {
            String outFileName = destinationFolder + "wordSplitAcrossRenderersWithPrecedingImageRenderer.pdf";
            String cmpFileName = sourceFolder + "cmp_wordSplitAcrossRenderersWithPrecedingImageRenderer.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            doc.SetFontSize(20);
            iText.Layout.Element.Image image = new Image(ImageDataFactory.Create(sourceFolder + "bulb.gif"));
            image.SetWidth(30);
            Text loooooooooooo = new Text("loooooooooooo").SetFontColor(ColorConstants.GREEN);
            Text oooooooo = new Text("oooooooo").SetFontColor(ColorConstants.RED);
            Text oooongWords = new Text("oooong words").SetFontColor(ColorConstants.BLUE);
            Paragraph paragraph = new Paragraph().Add(image).Add(loooooooooooo).Add(oooooooo).Add(oooongWords).SetBackgroundColor
                (ColorConstants.YELLOW).SetWidth(300).SetBorder(new SolidBorder(1));
            paragraph.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            paragraph.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            doc.Add(paragraph);
            paragraph.SetProperty(Property.RENDERING_MODE, RenderingMode.DEFAULT_LAYOUT_MODE);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.TABLE_WIDTH_IS_MORE_THAN_EXPECTED_DUE_TO_MIN_WIDTH)]
        public virtual void MinMaxWidthWordSplitAcrossMultipleTextRenderers() {
            String outFileName = destinationFolder + "minMaxWidthWordSplitAcrossMultipleTextRenderers.pdf";
            String cmpFileName = sourceFolder + "cmp_minMaxWidthWordSplitAcrossMultipleTextRenderers.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            doc.SetFontSize(20);
            Text wissen = new Text("Wissen").SetFontColor(ColorConstants.PINK).SetBackgroundColor(ColorConstants.YELLOW
                );
            Text schaft = new Text("schaft").SetFontColor(ColorConstants.MAGENTA).SetBackgroundColor(ColorConstants.YELLOW
                );
            Text ler = new Text("ler is a long German word!").SetFontColor(ColorConstants.RED).SetBackgroundColor(ColorConstants
                .YELLOW);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "bulb.gif"
                ));
            image.SetWidth(30);
            Paragraph text = new Paragraph().Add(wissen).Add(schaft).Add(ler);
            float[] colWidth = new float[] { 10, 20, 30, 40, 50 };
            Table table = new Table(UnitValue.CreatePercentArray(colWidth));
            for (int i = 0; i < colWidth.Length; i++) {
                table.AddCell(new Cell().Add(text));
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.TABLE_WIDTH_IS_MORE_THAN_EXPECTED_DUE_TO_MIN_WIDTH)]
        public virtual void MinWidthForWordInMultipleTextRenderersFollowedByFloatTest() {
            String outFileName = destinationFolder + "minWidthForSpanningWordFollowedByFloat.pdf";
            String cmpFileName = sourceFolder + "cmp_minWidthForSpanningWordFollowedByFloat.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            doc.SetFontSize(40);
            // add elements to the table in narrow parent div, so table width would be completely based on min-width
            Div narrowDivWithTable = new Div().SetBorder(new DashedBorder(ColorConstants.DARK_GRAY, 3)).SetWidth(10);
            Table table = new Table(1);
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
            Div floatingDiv = new Div();
            floatingDiv.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            floatingDiv.SetWidth(40).SetHeight(20).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            Paragraph paragraph = new Paragraph().Add(new Text("s")).Add(new Text("i")).Add(new Text("n")).Add(new Text
                ("g")).Add(new Text("l")).Add(new Text("e")).Add(floatingDiv).SetBorder(new SolidBorder(1));
            paragraph.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            paragraph.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            table.AddCell(paragraph);
            narrowDivWithTable.Add(table);
            doc.Add(narrowDivWithTable);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void OverflowWrapBreakWordWithOverflowXTest() {
            String outFileName = destinationFolder + "overflowWrapBreakWordWithOverflowXTest.pdf";
            String cmpFileName = sourceFolder + "cmp_overflowWrapBreakWordWithOverflowXTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            doc.SetFontSize(40);
            Text text = new Text("wow");
            Paragraph paragraph = new Paragraph().Add(text).SetBackgroundColor(ColorConstants.YELLOW).SetWidth(10).SetBorder
                (new SolidBorder(1));
            paragraph.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            paragraph.SetProperty(Property.OVERFLOW_WRAP, OverflowWrapPropertyValue.BREAK_WORD);
            paragraph.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.GET_NEXT_RENDERER_SHOULD_BE_OVERRIDDEN, Count = 3)]
        public virtual void CustomTextRendererShouldOverrideGetNextRendererTest() {
            String outFileName = destinationFolder + "customTextRendererShouldOverrideGetNextRendererTest.pdf";
            String cmpFileName = sourceFolder + "cmp_customTextRendererShouldOverrideGetNextRendererTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Text text = new Text("If getNextRenderer() is not overridden and text overflows to the next line," + " then customizations are not applied. "
                );
            text.SetNextRenderer(new _TextRenderer_742(text));
            doc.Add(new Paragraph(text));
            text = new Text("If getNextRenderer() is overridden and text overflows to the next line, " + "then customizations are applied. "
                );
            text.SetNextRenderer(new _TextRenderer_758(text));
            doc.Add(new Paragraph(text));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        private sealed class _TextRenderer_742 : TextRenderer {
            public _TextRenderer_742(Text baseArg1)
                : base(baseArg1) {
            }

            public override void Draw(DrawContext drawContext) {
                drawContext.GetCanvas().SaveState().SetFillColor(ColorConstants.RED).Rectangle(this.occupiedArea.GetBBox()
                    ).Fill().RestoreState();
                base.Draw(drawContext);
            }
        }

        private sealed class _TextRenderer_758 : TextRenderer {
            public _TextRenderer_758(Text baseArg1)
                : base(baseArg1) {
            }

            public override void Draw(DrawContext drawContext) {
                drawContext.GetCanvas().SaveState().SetFillColor(ColorConstants.RED).Rectangle(this.occupiedArea.GetBBox()
                    ).Fill().RestoreState();
                base.Draw(drawContext);
            }

            public override IRenderer GetNextRenderer() {
                return new TextRendererIntegrationTest.TextRendererWithOverriddenGetNextRenderer((Text)this.modelElement);
            }
        }

        [NUnit.Framework.Test]
        public virtual void NbspCannotBeFitAndIsTheOnlySymbolTest() {
            String outFileName = destinationFolder + "nbspCannotBeFitAndIsTheOnlySymbolTest.pdf";
            String cmpFileName = sourceFolder + "cmp_nbspCannotBeFitAndIsTheOnlySymbolTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            // No place for any symbol (page width is fully occupied by margins)
            Document doc = new Document(pdfDocument, new PageSize(72, 1000));
            Paragraph paragraph = new Paragraph().Add(new Text("\u00A0"));
            paragraph.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void NbspCannotBeFitAndMakesTheFirstChunkTest() {
            String outFileName = destinationFolder + "nbspCannotBeFitAndMakesTheFirstChunkTest.pdf";
            String cmpFileName = sourceFolder + "cmp_nbspCannotBeFitAndMakesTheFirstChunkTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            // No place for any symbol (page width is fully occupied by margins)
            Document doc = new Document(pdfDocument, new PageSize(72, 1000));
            Paragraph paragraph = new Paragraph().Add(new Text("\u00A0")).Add(new Text("SecondChunk"));
            paragraph.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void NbspCannotBeFitAndIsTheFirstSymbolOfChunkTest() {
            String outFileName = destinationFolder + "nbspCannotBeFitAndIsTheFirstSymbolOfChunkTest.pdf";
            String cmpFileName = sourceFolder + "cmp_nbspCannotBeFitAndIsTheFirstSymbolOfChunkTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            // No place for any symbol (page width is fully occupied by margins)
            Document doc = new Document(pdfDocument, new PageSize(72, 1000));
            Paragraph paragraph = new Paragraph().Add(new Text("\u00A0First"));
            paragraph.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void NbspCannotBeFitAndIsTheLastSymbolOfFirstChunkTest() {
            String outFileName = destinationFolder + "nbspCannotBeFitAndIsTheLastSymbolOfFirstChunkTest.pdf";
            String cmpFileName = sourceFolder + "cmp_nbspCannotBeFitAndIsTheLastSymbolOfFirstChunkTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            // No place for the second symbol
            Document doc = new Document(pdfDocument, new PageSize(81, 1000));
            Paragraph paragraph = new Paragraph().Add(new Text("H\u00A0")).Add(new Text("ello"));
            paragraph.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CREATE_COPY_SHOULD_BE_OVERRIDDEN, Count = 8)]
        public virtual void CustomTextRendererShouldOverrideCreateCopyTest() {
            String outFileName = destinationFolder + "customTextRendererShouldOverrideCreateCopyTest.pdf";
            String cmpFileName = sourceFolder + "cmp_customTextRendererShouldOverrideCreateCopyTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            FontProvider fontProvider = new FontProvider();
            NUnit.Framework.Assert.IsTrue(fontProvider.AddFont(fontsFolder + "NotoSans-Regular.ttf"));
            doc.SetFontProvider(fontProvider);
            // To trigger font selector related logic one need to apply some font on a document
            doc.SetProperty(Property.FONT, new String[] { "SomeFont" });
            StringBuilder longTextBuilder = new StringBuilder();
            for (int i = 0; i < 4; i++) {
                longTextBuilder.Append("Дзень добры, свет! Hallo Welt! ");
            }
            iText.Layout.Element.Text text = new iText.Layout.Element.Text(longTextBuilder.ToString());
            text.SetNextRenderer(new _TextRenderer_893(text));
            doc.Add(new Paragraph(text));
            text.SetNextRenderer(new TextRendererIntegrationTest.TextRendererWithOverriddenGetNextRenderer(text));
            doc.Add(new Paragraph(text));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        private sealed class _TextRenderer_893 : TextRenderer {
            public _TextRenderer_893(iText.Layout.Element.Text baseArg1)
                : base(baseArg1) {
            }

            public override void Draw(DrawContext drawContext) {
                drawContext.GetCanvas().SaveState().SetFillColor(ColorConstants.RED).Rectangle(this.occupiedArea.GetBBox()
                    ).Fill().RestoreState();
                base.Draw(drawContext);
            }

            public override IRenderer GetNextRenderer() {
                return new TextRendererIntegrationTest.TextRendererWithOverriddenGetNextRenderer((iText.Layout.Element.Text
                    )this.modelElement);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED, Count = 1)]
        public virtual void DrawWithSkewAndHorizontalScalingTest() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                Document doc = new Document(pdfDocument);
                iText.Layout.Element.Text text = new iText.Layout.Element.Text("test string").SetTextRise(0).SetWordSpacing
                    (0).SetSkew(10, 10).SetHorizontalScaling(2);
                Paragraph paragraph = new Paragraph().Add(text);
                paragraph.SetNextRenderer(new _TextRenderer_932(text));
                doc.Add(paragraph);
                String contentstream = iText.Commons.Utils.JavaUtil.GetStringForBytes(doc.GetPdfDocument().GetPage(1).GetContentBytes
                    (), System.Text.Encoding.UTF8);
                NUnit.Framework.Assert.IsTrue(contentstream.Contains("test string"));
            }
        }

        private sealed class _TextRenderer_932 : TextRenderer {
            public _TextRenderer_932(iText.Layout.Element.Text baseArg1)
                : base(baseArg1) {
            }

            public override void Draw(DrawContext drawContext) {
                drawContext.GetCanvas().SaveState().Rectangle(this.occupiedArea.GetBBox()).Fill().RestoreState();
                base.Draw(drawContext);
            }

            public override IRenderer GetNextRenderer() {
                return new TextRendererIntegrationTest.TextRendererWithOverriddenGetNextRenderer((iText.Layout.Element.Text
                    )this.modelElement);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED, Count = 1)]
        public virtual void DrawTextRenderingModeFillStrokeTest() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                Document doc = new Document(pdfDocument);
                iText.Layout.Element.Text text = new iText.Layout.Element.Text("test string").SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode
                    .FILL_STROKE);
                Paragraph paragraph = new Paragraph().Add(text).SetBackgroundColor(ColorConstants.YELLOW).SetWidth(10).SetStrokeWidth
                    (1f).SetStrokeColor(null).SetBorder(new SolidBorder(1));
                paragraph.SetNextRenderer(new _TextRenderer_970(text));
                doc.Add(paragraph);
                String contentstream = iText.Commons.Utils.JavaUtil.GetStringForBytes(doc.GetPdfDocument().GetPage(1).GetContentBytes
                    (), System.Text.Encoding.UTF8);
                NUnit.Framework.Assert.IsTrue(contentstream.Contains("test string"));
            }
        }

        private sealed class _TextRenderer_970 : TextRenderer {
            public _TextRenderer_970(iText.Layout.Element.Text baseArg1)
                : base(baseArg1) {
            }

            public override void Draw(DrawContext drawContext) {
                drawContext.GetCanvas().SaveState().Rectangle(this.occupiedArea.GetBBox()).Fill().RestoreState();
                base.Draw(drawContext);
            }

            public override IRenderer GetNextRenderer() {
                return new TextRendererIntegrationTest.TextRendererWithOverriddenGetNextRenderer((iText.Layout.Element.Text
                    )this.modelElement);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED, Count = 1)]
        public virtual void FontColorNullTest() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                Document doc = new Document(pdfDocument);
                iText.Layout.Element.Text text = new iText.Layout.Element.Text("test string").SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode
                    .FILL_STROKE);
                Paragraph paragraph = new Paragraph().Add(text);
                paragraph.SetNextRenderer(new _TextRenderer_1003(text));
                doc.Add(paragraph);
                String contentstream = iText.Commons.Utils.JavaUtil.GetStringForBytes(doc.GetPdfDocument().GetPage(1).GetContentBytes
                    (), System.Text.Encoding.UTF8);
                NUnit.Framework.Assert.IsTrue(contentstream.Contains("test string"));
            }
        }

        private sealed class _TextRenderer_1003 : TextRenderer {
            public _TextRenderer_1003(iText.Layout.Element.Text baseArg1)
                : base(baseArg1) {
            }

            public override void Draw(DrawContext drawContext) {
                drawContext.GetCanvas().SaveState().Rectangle(this.occupiedArea.GetBBox()).Fill().RestoreState();
                this.SetProperty(Property.FONT_COLOR, new TransparentColor(ColorConstants.RED));
                base.Draw(drawContext);
            }

            public override IRenderer GetNextRenderer() {
                return new TextRendererIntegrationTest.TextRendererWithOverriddenGetNextRenderer((iText.Layout.Element.Text
                    )this.modelElement);
            }
        }

        private class TextRendererWithOverriddenGetNextRenderer : TextRenderer {
            public TextRendererWithOverriddenGetNextRenderer(iText.Layout.Element.Text textElement)
                : base(textElement) {
            }

            protected internal TextRendererWithOverriddenGetNextRenderer(TextRenderer other)
                : base(other) {
            }

            public override void Draw(DrawContext drawContext) {
                drawContext.GetCanvas().SaveState().SetFillColor(ColorConstants.RED).Rectangle(occupiedArea.GetBBox()).Fill
                    ().RestoreState();
                base.Draw(drawContext);
            }

            public override IRenderer GetNextRenderer() {
                return new TextRendererIntegrationTest.TextRendererWithOverriddenGetNextRenderer((iText.Layout.Element.Text
                    )modelElement);
            }

            protected internal override TextRenderer CreateCopy(GlyphLine gl, PdfFont font) {
                TextRenderer copy = new TextRendererIntegrationTest.TextRendererWithOverriddenGetNextRenderer(this);
                copy.SetProcessedGlyphLineAndFont(gl, font);
                return copy;
            }
        }
    }
}
