/*
This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using System.Collections.Generic;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    public class TabsTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/TabTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/TabTest/";

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

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AnchorTabStopsTest() {
            String fileName = "anchorTabStopsTest.pdf";
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void OutOfPageBoundsTest() {
            String outFileName = destinationFolder + "outOfPageBoundsTest.pdf";
            String cmpFileName = sourceFolder + "cmp_outOfPageBoundsTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            //tabstops out of page bounds
            Paragraph p = new Paragraph();
            p.SetFontColor(Color.GREEN);
            p.Add("left tab stop out of page bounds:");
            doc.Add(p);
            p = new Paragraph();
            p.AddTabStops(new TabStop(1000, TabAlignment.LEFT, new DashedLine(.5f)));
            p.Add("text").Add(new Tab()).Add("some interesting text after left-tabstop");
            doc.Add(p);
            p = new Paragraph();
            p.SetFontColor(Color.GREEN);
            p.Add("right tab stop out of page bounds:");
            doc.Add(p);
            p = new Paragraph();
            p.AddTabStops(new TabStop(1000, TabAlignment.RIGHT, new DashedLine(.5f)));
            p.Add("text").Add(new Tab()).Add("some interesting text after right-tabstop");
            doc.Add(p);
            //text out of page bounds
            p = new Paragraph();
            p.SetFontColor(Color.GREEN);
            p.Add("text out of page bounds after left tab stop:");
            doc.Add(p);
            p = new Paragraph();
            p.AddTabStops(new TabStop(450, TabAlignment.LEFT, new DashedLine(.5f)));
            p.Add("text").Add(new Tab()).Add("some interesting text after left-tabstop\n");
            p.Add("text").Add(new Tab()).Add("someinterestingtextafterleft-tabstop");
            doc.Add(p);
            p = new Paragraph();
            p.SetFontColor(Color.GREEN);
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Ignore("1280")]
        [NUnit.Framework.Test]
        public virtual void TabsInParagraphTest01() {
            String outFileName = destinationFolder + "tabsInParagraphTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_tabsInParagraphTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            float tabWidth = pdfDoc.GetDefaultPageSize().GetWidth() - doc.GetLeftMargin() - doc.GetRightMargin();
            Paragraph p = new Paragraph();
            p.AddTabStops(new TabStop(tabWidth, TabAlignment.RIGHT)).Add("There is a tab after me. And then two texts."
                ).Add(new Tab()).Add("Text1").Add("Text2");
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.FileNotFoundException"/>
        private Document InitDocument(String outFileName) {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
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
            foreach (String line in iText.IO.Util.StringUtil.Split(text, "\n")) {
                foreach (String chunk in iText.IO.Util.StringUtil.Split(line, "\t")) {
                    p.Add(chunk).Add(new Tab());
                }
                p.Add("\n");
            }
        }
    }
}
