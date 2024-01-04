/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TabsTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/TabsTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/TabsTest/";

        private const String text0 = "The Po\u017Eega Valley is a geographic microregion\tof Croatia, located in central"
             + " Slavonia, enveloped by the Slavonian mountains. It consists of\tsouthern slopes of 984-metre (3,228 ft)"
             + " Psunj, 953-metre (3,127 ft) Papuk, and 792-metre (2,598 ft) The Krndija\tmountains, the northern slopes of "
             + "618-metre (2,028 ft) Po\u017Ee\u0161ka Gora and 461-metre\t(1,512 ft) the Dilj hills,\tand\tlowland is surrounded  by the "
             + "mountains and\thills, and occupying the eastern paaart\tof the Po\u017Eega-Slavonia County.";

        private const String text1 = "Sarehole Mill, Hall Green, Birmingham\t\"Inspired\" 1896\u20131900 (i. e. lived nearby)\t15 August 2002\tBirmingham Civic Society and The Tolkien Society\n"
             + "1 Duchess Place, Ladywood, Birmingham\tLived near here 1902\u20131910\tUnknown\tBirmingham Civic Society\n"
             + "4 Highfield Road, Edgbaston, Birmingham\tLived here 1910\u20131911\tUnknown\tBirmingham Civic Society and The Tolkien Society\n"
             + "Plough and Harrow, Hagley Road, Birmingham\tStayed here June 1916\tJune 1997\tThe Tolkien Society\n"
             + "2 Darnley Road, West Park, Leeds\tFirst academic appointment, Leeds\t1 October 2012\tThe Tolkien Society and the Leeds Civic Trust\n"
             + "20 Northmoor Road, North Oxford\tLived here 1930\u20131947\t3 December 2002\tOxfordshire Blue Plaques Board\n"
             + "Hotel Miramar, East Overcliff Drive, Bournemouth\tStayed here regularly from the 1950s until 1972\t10 June 1992 by Priscilla Tolkien\tBorough of Bournemouth";

        private const String text2 = "space anchor:\t222222222222222222222222222222222222222222222222 03\tslash anchor:\t2024\\12\tdot anchor:\t20421.32\n"
             + "space anchor:\t2012 203\tslash anchor:\t2024\\2\tdot anchor:\t20421.333452\n" + "space anchor:\t201212 0423\tslash anchor:\t2067867824\\67867812\tdot anchor:\t21.32131232\n"
             + "space anchor:\t2123123012 03\tslash anchor:\t202131224\\12\tdot anchor:\t202.32323232323232323223223223223232323232323232323232\n"
             + "space anchor:\t2012 0213133\tslash anchor:\t2024\\21312312\tdot anchor:\t131.292";

        // private static final String text3 = "\t0\n\t11#2.35\n\t813.2134#558914423\n\t3.37761#098\n\t#.715\n\t972#5844.18167\n\t";
        private const String text3 = "\t0\n\t11#2.35\n\t813.2134#558914423\n\t3.37761#098\n\t#.715\n\t972#5844.18167\n\t65#1094.6177##1128\n\t65.7#463\n\t68750.25121\n\t393#19.6#418#31\n\t7#811";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ChunkEndsAfterOrBeforeTabPosition() {
            String outFileName = destinationFolder + "chunkEndsAfterOrBeforeTabPosition.pdf";
            String cmpFileName = sourceFolder + "cmp_chunkEndsAfterOrBeforeTabPosition.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String textBeforeTab = "a";
            String textAfterTab = "tab stop's position = ";
            Paragraph paragraph;
            for (int i = 0; i < 20; i++) {
                paragraph = new Paragraph();
                paragraph.Add(new Text(textBeforeTab));
                TabStop[] tabStop = new TabStop[1];
                tabStop[0] = new TabStop(i);
                paragraph.AddTabStops(tabStop);
                paragraph.Add(new Tab());
                paragraph.Add(new Text(textAfterTab));
                paragraph.Add(JavaUtil.IntegerToString(i));
                doc.Add(paragraph);
            }
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void DefaultTabsTest() {
            String outFileName = destinationFolder + "defaultTabTest.pdf";
            String cmpFileName = sourceFolder + "cmp_defaultTabTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Paragraph p = new Paragraph();
            AddTabbedTextToParagraph(p, text0, new float[0], null, null, null);
            doc.Add(p);
            float left = doc.GetLeftMargin();
            float right = doc.GetRightMargin();
            float pageWidth = doc.GetPdfDocument().GetDefaultPageSize().GetWidth();
            float[] defaultStopPositions = new float[] { 0f, 50f, 100f, 150f, 200f, 250f, 300f, 350f, 400f, 450f, 500f
                , pageWidth - left - right };
            DrawTabStopsPositions(defaultStopPositions, doc, 1, 0, 120);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTabStopsTest() {
            String fileName = "simpleTabStopsTest.pdf";
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            Document doc = InitDocument(outFileName);
            float tabInterval = doc.GetPdfDocument().GetDefaultPageSize().GetWidth() / 8;
            //left alignments
            float[] positions1 = new float[] { tabInterval * 2, tabInterval * 4, tabInterval * 5 };
            TabAlignment[] alignments1 = new TabAlignment[] { TabAlignment.LEFT, TabAlignment.LEFT, TabAlignment.LEFT };
            ILineDrawer[] leaders1 = new ILineDrawer[] { null, null, null };
            char?[] anchors1 = new char?[] { null, null, null };
            Paragraph p = new Paragraph();
            p.SetFontSize(8);
            AddTabbedTextToParagraph(p, text1, positions1, alignments1, leaders1, anchors1);
            doc.Add(p);
            doc.Add(new Paragraph("\n"));
            //right alignments
            float[] positions2 = new float[] { tabInterval * 3, tabInterval * 4, tabInterval * 6 };
            TabAlignment[] alignments2 = new TabAlignment[] { TabAlignment.RIGHT, TabAlignment.RIGHT, TabAlignment.RIGHT
                 };
            ILineDrawer[] leaders2 = new ILineDrawer[] { null, null, null };
            char?[] anchors2 = new char?[] { null, null, null };
            p = new Paragraph();
            p.SetFontSize(8);
            AddTabbedTextToParagraph(p, text1, positions2, alignments2, leaders2, anchors2);
            doc.Add(p);
            doc.Add(new Paragraph("\n"));
            //center alignments
            float[] positions3 = new float[] { tabInterval * 3, tabInterval * 4, tabInterval * 6 };
            TabAlignment[] alignments3 = new TabAlignment[] { TabAlignment.CENTER, TabAlignment.CENTER, TabAlignment.CENTER
                 };
            ILineDrawer[] leaders3 = new ILineDrawer[] { null, null, null };
            char?[] anchors3 = new char?[] { null, null, null };
            p = new Paragraph();
            p.SetFontSize(8);
            AddTabbedTextToParagraph(p, text1, positions3, alignments3, leaders3, anchors3);
            doc.Add(p);
            DrawTabStopsPositions(positions1, doc, 1, 0, 120);
            DrawTabStopsPositions(positions2, doc, 1, 125, 95);
            DrawTabStopsPositions(positions3, doc, 1, 235, 95);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void AnchorTabStopsTest01() {
            String fileName = "anchorTabStopsTest01.pdf";
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            Document doc = InitDocument(outFileName);
            float tabInterval = doc.GetPdfDocument().GetDefaultPageSize().GetWidth() / 8;
            float[] positions1 = new float[] { tabInterval * 2, tabInterval * 3, tabInterval * 4, tabInterval * 5, tabInterval
                 * 6 };
            TabAlignment[] alignments1 = new TabAlignment[] { TabAlignment.ANCHOR, TabAlignment.CENTER, TabAlignment.ANCHOR
                , TabAlignment.RIGHT, TabAlignment.ANCHOR };
            ILineDrawer[] leaders1 = new ILineDrawer[] { new DottedLine(), null, new DashedLine(.5f), null, new SolidLine
                (.5f) };
            char?[] anchors1 = new char?[] { ' ', null, '\\', null, '.' };
            Paragraph p = new Paragraph();
            p.SetFontSize(8);
            AddTabbedTextToParagraph(p, text2, positions1, alignments1, leaders1, anchors1);
            doc.Add(p);
            DrawTabStopsPositions(positions1, doc, 1, 0, 120);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff" + outFileName));
        }

        [NUnit.Framework.Test]
        public virtual void AnchorTabStopsTest02() {
            String fileName = "anchorTabStopsTest02.pdf";
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            Document doc = InitDocument(outFileName, true);
            float tabInterval = doc.GetPdfDocument().GetDefaultPageSize().GetWidth() / 2;
            float[] positions1 = new float[] { tabInterval };
            TabAlignment[] alignments1 = new TabAlignment[] { TabAlignment.ANCHOR };
            ILineDrawer[] leaders1 = new ILineDrawer[] { new DottedLine() };
            char?[] anchors1 = new char?[] { '.' };
            Paragraph p = new Paragraph();
            p.SetFontSize(8);
            AddTabbedTextToParagraph(p, text3, positions1, alignments1, leaders1, anchors1);
            doc.Add(p);
            DrawTabStopsPositions(positions1, doc, 1, 0, 200);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff" + outFileName));
        }

        [NUnit.Framework.Test]
        public virtual void TablesAndTabInsideOfParagraph() {
            String testName = "tablesAndTabInsideOfParagraph.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            Document doc = InitDocument(outFileName, false);
            Table leftTable = new Table(1);
            for (int x = 0; x < 3; x++) {
                leftTable.AddCell("Table 1, Line " + (x + 1));
            }
            Table rightTable = new Table(1);
            for (int x = 0; x < 3; x++) {
                rightTable.AddCell("Table 2, Line " + (x + 1));
            }
            Paragraph p = new Paragraph().Add(leftTable);
            p.Add(new Tab());
            p.AddTabStops(new TabStop(300, TabAlignment.LEFT));
            p.Add(rightTable);
            doc.Add(new Paragraph("TabAlignment: LEFT"));
            doc.Add(p);
            p = new Paragraph().Add(leftTable);
            p.Add(new Tab());
            p.AddTabStops(new TabStop(300, TabAlignment.CENTER));
            p.Add(rightTable);
            doc.Add(new Paragraph("TabAlignment: CENTER"));
            doc.Add(p);
            p = new Paragraph().Add(leftTable);
            p.Add(new Tab());
            p.AddTabStops(new TabStop(300, TabAlignment.RIGHT));
            p.Add(rightTable);
            doc.Add(new Paragraph("TabAlignment: RIGHT"));
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TabPositionAbsoluteValueTest() {
            String outFileName = destinationFolder + "tabPositionAbsoluteValue.pdf";
            String cmpFileName = sourceFolder + "cmp_tabPositionAbsoluteValue.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph("x-coordinate = 100").SetFontColor(ColorConstants.RED).SetFirstLineIndent(100).SetFontSize
                (8));
            doc.Add(new Paragraph("x-coordinate = 200").SetFontColor(ColorConstants.GREEN).SetFirstLineIndent(200).SetFontSize
                (8));
            doc.Add(new Paragraph("x-coordinate = 300").SetFontColor(ColorConstants.BLUE).SetFirstLineIndent(300).SetFontSize
                (8));
            Paragraph p = new Paragraph().Add("Hello, iText!").Add(new Tab()).AddTabStops(new TabStop(100)).Add("Hi, iText!"
                ).Add(new Tab()).AddTabStops(new TabStop(200)).Add("Hello, iText!").Add(new Tab()).AddTabStops(new TabStop
                (300)).Add("Hello, iText!");
            doc.Add(p);
            float[] positions = new float[] { 100, 200, 300 };
            DrawTabStopsPositions(positions, doc, 1, 0, 120);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SeveralTabsInRowTest() {
            String fileName = "severalTabsInRowTest.pdf";
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            Document doc = InitDocument(outFileName);
            float tabInterval = doc.GetPdfDocument().GetDefaultPageSize().GetWidth() / 8;
            float[] positions = new float[] { tabInterval * 2, tabInterval * 4, tabInterval * 6 };
            TabAlignment[] alignments = new TabAlignment[] { TabAlignment.RIGHT, TabAlignment.CENTER, TabAlignment.CENTER
                 };
            //        Drawable[] leaders = {null, null, null};
            ILineDrawer[] leaders = new ILineDrawer[] { new DottedLine(), new DashedLine(.5f), new SolidLine(.5f) };
            Paragraph p = new Paragraph();
            p.SetFontSize(8);
            IList<TabStop> tabStops = new List<TabStop>();
            for (int i = 0; i < positions.Length; ++i) {
                TabStop tabStop = new TabStop(positions[i], alignments[i], leaders[i]);
                tabStops.Add(tabStop);
            }
            p.AddTabStops(tabStops);
            p.Add(new Tab()).Add("ttttttttttttttttttttttttttttttttttttttttttttt").Add(new Tab()).Add(new Tab()).Add("ttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt\n"
                );
            p.Add(new Tab()).Add(new Tab()).Add(new Tab()).Add("ttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt\n"
                );
            p.Add(new Tab()).Add(new Tab()).Add("ttttttttttttttttttttttttttttttttttttttttttttt").Add(new Tab()).Add("ttttttttttttttttttttttttttttttttttttttttttt"
                );
            doc.Add(p);
            DrawTabStopsPositions(positions, doc, 1, 0, 120);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff" + outFileName));
        }

        [NUnit.Framework.Test]
        public virtual void OutOfPageBoundsTest() {
            String outFileName = destinationFolder + "outOfPageBoundsTest.pdf";
            String cmpFileName = sourceFolder + "cmp_outOfPageBoundsTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            //tabstops out of page bounds
            Paragraph p = new Paragraph();
            p.SetFontColor(ColorConstants.GREEN);
            p.Add("left tab stop out of page bounds:");
            doc.Add(p);
            p = new Paragraph();
            p.AddTabStops(new TabStop(1000, TabAlignment.LEFT, new DashedLine(.5f)));
            p.Add("text").Add(new Tab()).Add("some interesting text after left-tabstop");
            doc.Add(p);
            p = new Paragraph();
            p.SetFontColor(ColorConstants.GREEN);
            p.Add("right tab stop out of page bounds:");
            doc.Add(p);
            p = new Paragraph();
            p.AddTabStops(new TabStop(1000, TabAlignment.RIGHT, new DashedLine(.5f)));
            p.Add("text").Add(new Tab()).Add("some interesting text after right-tabstop");
            doc.Add(p);
            //text out of page bounds
            p = new Paragraph();
            p.SetFontColor(ColorConstants.GREEN);
            p.Add("text out of page bounds after left tab stop:");
            doc.Add(p);
            p = new Paragraph();
            p.AddTabStops(new TabStop(450, TabAlignment.LEFT, new DashedLine(.5f)));
            p.Add("text").Add(new Tab()).Add("some interesting text after left-tabstop\n");
            p.Add("text").Add(new Tab()).Add("someinterestingtextafterleft-tabstop");
            doc.Add(p);
            p = new Paragraph();
            p.SetFontColor(ColorConstants.GREEN);
            p.Add("text out of page bounds after right tab stop:");
            doc.Add(p);
            p = new Paragraph();
            p.AddTabStops(new TabStop(450, TabAlignment.RIGHT, new DashedLine(.5f)));
            p.Add("teeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeext").Add(new Tab()).Add("some interesting text after right-tabstop\n"
                );
            p.Add("teeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeext").Add(new Tab()).Add("someinterestingtextafterright-tabstop\n"
                );
            p.Add("teeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeext").Add(new Tab()).Add("word.");
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TabsInParagraphTest01() {
            String outFileName = destinationFolder + "tabsInParagraphTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_tabsInParagraphTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            float tabWidth = pdfDoc.GetDefaultPageSize().GetWidth() - doc.GetLeftMargin() - doc.GetRightMargin();
            Paragraph p = new Paragraph();
            p.AddTabStops(new TabStop(tabWidth, TabAlignment.RIGHT)).Add("There is a right-aligned tab after me. And then three chunks of text."
                ).Add(new Tab()).Add("Text1").Add("Text2").Add("Text3");
            doc.Add(p);
            p = new Paragraph();
            p.AddTabStops(new TabStop(tabWidth, TabAlignment.RIGHT)).Add("There is a right-aligned tab after me. And then three chunks of text."
                ).Add(new Tab()).Add("Text1").Add("Tex\nt2").Add("Text3");
            doc.Add(p);
            p = new Paragraph();
            p.AddTabStops(new TabStop(tabWidth, TabAlignment.RIGHT)).Add("There is a right-aligned tab after me. And then three chunks of text."
                ).Add(new Tab()).Add("Long Long Long Long Long Long Long Text1").Add("Tex\nt2").Add("Text3");
            doc.Add(p);
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreateJpeg(UrlUtil.ToURL(sourceFolder + "Desert.jpg"
                )));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 100);
            p = new Paragraph();
            p.AddTabStops(new TabStop(tabWidth, TabAlignment.RIGHT)).Add("There is a right-aligned tab after me. And then texts and an image."
                ).Add(new Tab()).Add("Text1").Add(image).Add("Text3");
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TabsAnchorSemicolonTest01() {
            String outFileName = destinationFolder + "tabsAnchorSemicolonTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_tabsAnchorSemicolonTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDoc);
            float w = document.GetPageEffectiveArea(PageSize.A4).GetWidth();
            Paragraph p = new Paragraph();
            IList<TabStop> tabstops = new List<TabStop>();
            tabstops.Add(new TabStop(w / 2, TabAlignment.RIGHT));
            tabstops.Add(new TabStop(w / 2 + 1f, TabAlignment.LEFT));
            p.AddTabStops(tabstops);
            p.Add(new Tab()).Add("Test:").Add(new Tab()).Add("Answer");
            document.Add(p);
            p = new Paragraph();
            p.AddTabStops(tabstops);
            p.Add(new Tab()).Add("Test245454:").Add(new Tab()).Add("Answer2");
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TabsAnchorSemicolonTest02() {
            String outFileName = destinationFolder + "tabsAnchorSemicolonTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_tabsAnchorSemicolonTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDoc);
            float w = document.GetPageEffectiveArea(PageSize.A4).GetWidth();
            Paragraph p = new Paragraph();
            p.SetProperty(Property.TAB_DEFAULT, 0.01f);
            IList<TabStop> tabstops = new List<TabStop>();
            tabstops.Add(new TabStop(w / 2, TabAlignment.RIGHT));
            p.AddTabStops(tabstops);
            p.Add(new Tab()).Add("Test:").Add(new Tab()).Add("Answer");
            document.Add(p);
            p = new Paragraph();
            p.SetProperty(Property.TAB_DEFAULT, 0.01f);
            p.AddTabStops(tabstops);
            p.Add(new Tab()).Add("Test245454:").Add(new Tab()).Add("Answer2");
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TabsAnchorSemicolonTest03() {
            String outFileName = destinationFolder + "tabsAnchorSemicolonTest03.pdf";
            String cmpFileName = sourceFolder + "cmp_tabsAnchorSemicolonTest03.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDoc);
            float w = document.GetPageEffectiveArea(PageSize.A4).GetWidth();
            Paragraph p = new Paragraph();
            TabStop tabStop = new TabStop(w / 2, TabAlignment.ANCHOR);
            tabStop.SetTabAnchor(':');
            p.AddTabStops(tabStop);
            p.Add(new Tab()).Add("Test:Answer");
            document.Add(p);
            p = new Paragraph();
            p.AddTabStops(tabStop);
            p.Add(new Tab()).Add("Test245454:Answer2");
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FillParagraphWithTabsDifferently() {
            String outFileName = destinationFolder + "fillParagraphWithTabsDifferently.pdf";
            String cmpFileName = sourceFolder + "cmp_fillParagraphWithTabsDifferently.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph("a\tb"));
            doc.Add(new Paragraph().Add("a").Add("\t").Add("b"));
            doc.Add(new Paragraph().Add(new Text("a")).Add(new Text("\t")).Add(new Text("b")));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        private Document InitDocument(String outFileName) {
            return InitDocument(outFileName, false);
        }

        private Document InitDocument(String outFileName, bool tagged) {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            if (tagged) {
                pdfDoc.SetTagged();
            }
            pdfDoc.SetDefaultPageSize(PageSize.A4.Rotate());
            return new Document(pdfDoc);
        }

        private void DrawTabStopsPositions(float[] positions, Document doc, int pageNum, int yStart, int dy) {
            PdfCanvas canvas = new PdfCanvas(doc.GetPdfDocument().GetPage(pageNum));
            float left = doc.GetLeftMargin();
            float h = doc.GetPdfDocument().GetPage(pageNum).GetCropBox().GetHeight() - yStart;
            canvas.SaveState();
            canvas.SetLineDash(4, 2);
            canvas.SetLineWidth(0.5f);
            canvas.SetLineDash(4, 2);
            foreach (float f in positions) {
                canvas.MoveTo(left + f, h);
                canvas.LineTo(left + f, h - dy);
            }
            canvas.Stroke();
            canvas.RestoreState();
            canvas.Release();
        }

        private void AddTabbedTextToParagraph(Paragraph p, String text, float[] positions, TabAlignment[] alignments
            , ILineDrawer[] tabLeadings, char?[] tabAnchorCharacters) {
            IList<TabStop> tabStops = new List<TabStop>();
            for (int i = 0; i < positions.Length; ++i) {
                TabStop tabStop = new TabStop(positions[i], alignments[i], tabLeadings[i]);
                tabStop.SetTabAnchor(tabAnchorCharacters[i]);
                tabStops.Add(tabStop);
            }
            p.AddTabStops(tabStops);
            foreach (String line in iText.Commons.Utils.StringUtil.Split(text, "\n")) {
                foreach (String chunk in iText.Commons.Utils.StringUtil.Split(line, "\t")) {
                    foreach (String piece in iText.Commons.Utils.StringUtil.Split(chunk, "#")) {
                        if (!String.IsNullOrEmpty(piece)) {
                            p.Add(piece);
                        }
                    }
                    p.Add(new Tab());
                }
                p.Add("\n");
            }
        }
    }
}
