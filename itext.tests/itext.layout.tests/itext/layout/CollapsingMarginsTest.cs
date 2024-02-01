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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class CollapsingMarginsTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/CollapsingMarginsTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/CollapsingMarginsTest/";

        private const String TEXT_BYRON = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
             + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
             + "\n" + "To do good to Mankind is the chivalrous plan,\n" + "    And is always as nobly requited;\n"
             + "Then battle for Freedom wherever you can,\n" + "    And, if not shot or hanged, you'll get knighted.";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsTest01() {
            String outFileName = destinationFolder + "collapsingMarginsTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_collapsingMarginsTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            DrawPageBorders(pdfDocument, 4);
            Document doc = new Document(pdfDocument);
            doc.SetProperty(Property.COLLAPSING_MARGINS, true);
            doc.Add(new Paragraph("marker text").SetMargin(0));
            Paragraph p = new Paragraph(TEXT_BYRON);
            for (int i = 0; i < 5; i++) {
                p.Add(TEXT_BYRON);
            }
            Div div1 = new Div();
            Div div2 = new Div();
            div1.Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
            div2.Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
            div1.SetMarginBottom(20);
            div2.SetMarginTop(150);
            div2.SetMarginBottom(150);
            Div div = new Div().SetMarginTop(20).SetMarginBottom(10).SetBackgroundColor(new DeviceRgb(78, 151, 205));
            div.Add(div1);
            div.Add(div2);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsTest02() {
            String outFileName = destinationFolder + "collapsingMarginsTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_collapsingMarginsTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            DrawPageBorders(pdfDocument, 3);
            Document doc = new Document(pdfDocument);
            doc.SetProperty(Property.COLLAPSING_MARGINS, true);
            doc.Add(new Paragraph("marker text").SetMargin(0));
            Paragraph p = new Paragraph(TEXT_BYRON);
            for (int i = 0; i < 3; i++) {
                p.Add(TEXT_BYRON);
            }
            p.Add("When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "\n" + "To do good to Mankind is the chivalrous plan,\n");
            Div div1 = new Div();
            Div div2 = new Div();
            div1.Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
            div2.Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
            div1.SetMarginBottom(40);
            div2.SetMarginTop(20);
            div2.SetMarginBottom(150);
            Div div = new Div().SetMarginTop(20).SetMarginBottom(10).SetBackgroundColor(new DeviceRgb(78, 151, 205));
            div.Add(div1);
            div.Add(div2);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsTest03() {
            String outFileName = destinationFolder + "collapsingMarginsTest03.pdf";
            String cmpFileName = sourceFolder + "cmp_collapsingMarginsTest03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            DrawPageBorders(pdfDocument, 3);
            Document doc = new Document(pdfDocument);
            doc.SetProperty(Property.COLLAPSING_MARGINS, true);
            doc.Add(new Paragraph("marker text").SetMargin(0));
            Paragraph p = new Paragraph(TEXT_BYRON);
            for (int i = 0; i < 3; i++) {
                p.Add(TEXT_BYRON);
            }
            p.Add("When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "To do good to Mankind is the chivalrous plan,\n");
            Div div1 = new Div();
            Div div2 = new Div();
            div1.Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
            div2.Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
            div1.SetMarginBottom(80);
            div2.SetMarginTop(80);
            div2.SetMarginBottom(150);
            doc.Add(div1);
            doc.Add(div2);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsTest04() {
            String outFileName = destinationFolder + "collapsingMarginsTest04.pdf";
            String cmpFileName = sourceFolder + "cmp_collapsingMarginsTest04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            DrawPageBorders(pdfDocument, 3);
            Document doc = new Document(pdfDocument);
            doc.SetProperty(Property.COLLAPSING_MARGINS, true);
            doc.Add(new Paragraph("marker text").SetMargin(0));
            Paragraph p = new Paragraph(TEXT_BYRON);
            for (int i = 0; i < 3; i++) {
                p.Add(TEXT_BYRON);
            }
            p.Add("When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "To do good to Mankind is the chivalrous plan,\n");
            p.Add(new Text("small text").SetFontSize(5.1f));
            p.Add("\nAnd is always as nobly requited;\n" + "Then battle for Freedom wherever you can,\n" + "And, if not shot or hanged, you'll get knighted."
                );
            Div div1 = new Div();
            Div div2 = new Div();
            div1.Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
            div2.Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
            div1.SetMarginBottom(80);
            div2.SetMarginTop(80);
            div2.SetMarginBottom(150);
            doc.Add(div1);
            doc.Add(div2);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsTest05() {
            String outFileName = destinationFolder + "collapsingMarginsTest05.pdf";
            String cmpFileName = sourceFolder + "cmp_collapsingMarginsTest05.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            DrawPageBorders(pdfDocument, 2);
            Document doc = new Document(pdfDocument);
            doc.SetProperty(Property.COLLAPSING_MARGINS, true);
            Paragraph p = new Paragraph(TEXT_BYRON).SetBackgroundColor(ColorConstants.YELLOW);
            for (int i = 0; i < 3; i++) {
                p.Add(TEXT_BYRON);
            }
            doc.Add(p);
            p.SetMarginTop(80);
            Div div = new Div();
            div.Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ElementCollapsingMarginsTest01() {
            String outFileName = destinationFolder + "elementCollapsingMarginsTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_elementCollapsingMarginsTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            DrawPageBorders(pdfDocument, 1);
            Document doc = new Document(pdfDocument);
            Paragraph markerText = new Paragraph("Margin between this paragraph and next block is expected to be 170pt."
                ).SetBackgroundColor(new DeviceRgb(65, 151, 29));
            // greenish
            Div div = new Div();
            Paragraph p = new Paragraph(TEXT_BYRON);
            div.Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
            // yellowish
            div.SetProperty(Property.COLLAPSING_MARGINS, true);
            markerText.SetMarginBottom(20);
            p.SetMarginTop(50);
            div.SetMarginTop(150);
            doc.Add(markerText);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        private void DrawPageBorders(PdfDocument pdfDocument, int pageNum) {
            for (int i = 1; i <= pageNum; ++i) {
                while (pdfDocument.GetNumberOfPages() < i) {
                    pdfDocument.AddNewPage();
                }
                PdfCanvas canvas = new PdfCanvas(pdfDocument.GetPage(i));
                canvas.SaveState();
                canvas.SetLineDash(5, 10);
                canvas.Rectangle(36, 36, 595 - 36 * 2, 842 - 36 * 2);
                canvas.Stroke();
                canvas.RestoreState();
            }
        }

        [NUnit.Framework.Test]
        public virtual void ColumnRendererTest() {
            /* TODO DEVSIX-2901 the exception should not be thrown
            if after DEVSIX-2901 the exception persists,
            change the type of the expected exception to a more specific one to make the test stricter.
            */
            String outFileName = destinationFolder + "columnRendererTest.pdf";
            String cmpFileName = sourceFolder + "cmp_columnRendererTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            using (Document doc = new Document(pdfDocument)) {
                doc.SetProperty(Property.COLLAPSING_MARGINS, true);
                Paragraph p = new Paragraph();
                for (int i = 0; i < 10; i++) {
                    p.Add(TEXT_BYRON);
                }
                Div div = new Div().Add(p);
                IList<Rectangle> areas = new List<Rectangle>();
                areas.Add(new Rectangle(30, 30, 150, 600));
                areas.Add(new Rectangle(200, 30, 150, 600));
                areas.Add(new Rectangle(370, 30, 150, 600));
                div.SetNextRenderer(new CollapsingMarginsTest.CustomColumnDocumentRenderer(div, areas));
                NUnit.Framework.Assert.Catch(typeof(Exception), () => doc.Add(div));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        private class CustomColumnDocumentRenderer : DivRenderer {
            private IList<Rectangle> areas;

            public CustomColumnDocumentRenderer(Div modelElement, IList<Rectangle> areas)
                : base(modelElement) {
                this.areas = areas;
            }

            public override LayoutResult Layout(LayoutContext layoutContext) {
                LayoutResult result = base.Layout(layoutContext);
                return result;
            }

            public override IList<Rectangle> InitElementAreas(LayoutArea area) {
                return areas;
            }

            public override IRenderer GetNextRenderer() {
                return new CollapsingMarginsTest.CustomColumnDocumentRenderer((Div)modelElement, areas);
            }
        }
    }
}
