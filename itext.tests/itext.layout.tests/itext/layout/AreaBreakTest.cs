/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
