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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class AreaBreakTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/AreaBreakTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/AreaBreakTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void PageBreakTest1() {
            String outFileName = destinationFolder + "pageBreak1.pdf";
            String cmpFileName = sourceFolder + "cmp_pageBreak1.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new AreaBreak());
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void PageBreakTest2() {
            String outFileName = destinationFolder + "pageBreak2.pdf";
            String cmpFileName = sourceFolder + "cmp_pageBreak2.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph("Hello World!")).Add(new AreaBreak(new PageSize(200, 200)));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void PageBreakTest03() {
            String outFileName = destinationFolder + "pageBreak3.pdf";
            String cmpFileName = sourceFolder + "cmp_pageBreak3.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            document.SetRenderer(new ColumnDocumentRenderer(document, new Rectangle[] { new Rectangle(30, 30, 200, 600
                ), new Rectangle(300, 30, 200, 600) }));
            document.Add(new Paragraph("Hello World!")).Add(new AreaBreak(AreaBreakType.NEXT_PAGE)).Add(new Paragraph(
                "New page hello world"));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void LastPageAreaBreakTest01() {
            String inputFileName = sourceFolder + "input.pdf";
            String cmpFileName = sourceFolder + "cmp_lastPageAreaBreakTest01.pdf";
            String outFileName = destinationFolder + "lastPageAreaBreakTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputFileName), new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new AreaBreak(AreaBreakType.LAST_PAGE)).Add(new Paragraph("Hello there on the last page!").SetFontSize
                (30).SetWidth(200).SetMarginTop(250));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void LastPageAreaBreakTest02() {
            String cmpFileName = sourceFolder + "cmp_lastPageAreaBreakTest02.pdf";
            String outFileName = destinationFolder + "lastPageAreaBreakTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.AddNewPage();
            Document document = new Document(pdfDocument);
            document.Add(new AreaBreak(AreaBreakType.LAST_PAGE)).Add(new Paragraph("Hello there on the last page!").SetFontSize
                (30).SetWidth(200).SetMarginTop(250));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void LastPageAreaBreakTest03() {
            String cmpFileName = sourceFolder + "cmp_lastPageAreaBreakTest03.pdf";
            String outFileName = destinationFolder + "lastPageAreaBreakTest03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.AddNewPage();
            pdfDocument.AddNewPage();
            Document document = new Document(pdfDocument);
            document.Add(new AreaBreak(AreaBreakType.LAST_PAGE)).Add(new Paragraph("Hello there on the last page!").SetFontSize
                (30).SetWidth(200).SetMarginTop(250));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void LastPageAreaBreakTest04() {
            String inputFileName = sourceFolder + "input.pdf";
            String cmpFileName = sourceFolder + "cmp_lastPageAreaBreakTest04.pdf";
            String outFileName = destinationFolder + "lastPageAreaBreakTest04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputFileName), new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new AreaBreak(AreaBreakType.LAST_PAGE)).Add(new AreaBreak(AreaBreakType.LAST_PAGE)).Add(new Paragraph
                ("Hello there on the last page!").SetFontSize(30).SetWidth(200).SetMarginTop(250));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void AreaBreakInsideDiv01Test() {
            String outFileName = destinationFolder + "areaBreakInsideDiv01.pdf";
            String cmpFileName = sourceFolder + "cmp_areaBreakInsideDiv01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div div = new Div().Add(new Paragraph("Hello")).Add(new AreaBreak()).Add(new Paragraph("World"));
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void AreaBreakInsideDiv02Test() {
            String outFileName = destinationFolder + "areaBreakInsideDiv02.pdf";
            String cmpFileName = sourceFolder + "cmp_areaBreakInsideDiv02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div div = new Div().Add(new Paragraph("Hello")).Add(new AreaBreak(PageSize.A5)).Add(new AreaBreak(PageSize
                .A6)).Add(new Paragraph("World"));
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void AreaBreakInsideDiv03Test() {
            String outFileName = destinationFolder + "areaBreakInsideDiv03.pdf";
            String cmpFileName = sourceFolder + "cmp_areaBreakInsideDiv03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Div div = new Div().Add(new Paragraph("Hello")).Add(new AreaBreak()).Add(new Paragraph("World"));
            div.SetNextRenderer(new AreaBreakTest.DivRendererWithAreas(div));
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void AreaBreakInsideDiv04Test() {
            String outFileName = destinationFolder + "areaBreakInsideDiv04.pdf";
            String cmpFileName = sourceFolder + "cmp_areaBreakInsideDiv04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div div = new Div().Add(new Paragraph("Hello")).Add(new AreaBreak(AreaBreakType.NEXT_PAGE)).Add(new Paragraph
                ("World"));
            div.SetNextRenderer(new AreaBreakTest.DivRendererWithAreas(div));
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        private class DivRendererWithAreas : DivRenderer {
            public DivRendererWithAreas(Div modelElement)
                : base(modelElement) {
            }

            public override IList<Rectangle> InitElementAreas(LayoutArea area) {
                return JavaUtil.ArraysAsList(new Rectangle(area.GetBBox()).SetWidth(area.GetBBox().GetWidth() / 2), new Rectangle
                    (area.GetBBox()).SetWidth(area.GetBBox().GetWidth() / 2).MoveRight(area.GetBBox().GetWidth() / 2));
            }

            public override IRenderer GetNextRenderer() {
                return new AreaBreakTest.DivRendererWithAreas((Div)modelElement);
            }
        }
    }
}
