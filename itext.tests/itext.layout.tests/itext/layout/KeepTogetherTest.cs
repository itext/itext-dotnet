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
using System.Text;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class KeepTogetherTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/KeepTogetherTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/KeepTogetherTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void KeepTogetherParagraphTest01() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherParagraphTest01.pdf";
            String outFile = destinationFolder + "keepTogetherParagraphTest01.pdf";
            PdfWriter writer = new PdfWriter(outFile);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            for (int i = 0; i < 28; i++) {
                doc.Add(new Paragraph("String number" + i));
            }
            String str = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaanasdadasdadaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            Paragraph p1 = new Paragraph(str);
            p1.SetKeepTogether(true);
            doc.Add(p1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherParagraphTest02() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherParagraphTest02.pdf";
            String outFile = destinationFolder + "keepTogetherParagraphTest02.pdf";
            PdfWriter writer = new PdfWriter(outFile);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            for (int i = 0; i < 28; i++) {
                doc.Add(new Paragraph("String number" + i));
            }
            String str = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaanasdadasdadaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            for (int i = 0; i < 5; i++) {
                str += str;
            }
            Paragraph p1 = new Paragraph(str);
            p1.SetKeepTogether(true);
            doc.Add(p1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void KeepTogetherListTest01() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherListTest01.pdf";
            String outFile = destinationFolder + "keepTogetherListTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            for (int i = 0; i < 28; i++) {
                doc.Add(new Paragraph("String number" + i));
            }
            List list = new List();
            list.Add("firstItem").Add("secondItem").Add("thirdItem").SetKeepTogether(true).SetListSymbol(ListNumberingType
                .DECIMAL);
            doc.Add(list);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void KeepTogetherDivTest01() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherDivTest01.pdf";
            String outFile = destinationFolder + "keepTogetherDivTest01.pdf";
            PdfWriter writer = new PdfWriter(outFile);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            Paragraph p = new Paragraph("Test String");
            for (int i = 0; i < 28; i++) {
                doc.Add(p);
            }
            Div div = new Div();
            div.Add(new Paragraph("first paragraph"));
            div.Add(new Paragraph("second paragraph"));
            div.Add(new Paragraph("third paragraph"));
            div.SetKeepTogether(true);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void KeepTogetherMinHeightTest() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherMinHeightTest.pdf";
            String outFile = destinationFolder + "keepTogetherMinHeightTest.pdf";
            PdfWriter writer = new PdfWriter(outFile);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            Paragraph p = new Paragraph("Test String");
            for (int i = 0; i < 15; i++) {
                doc.Add(p);
            }
            Div div = new Div();
            div.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            div.SetMinHeight(500);
            div.SetKeepTogether(true);
            div.Add(new Paragraph("Hello"));
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherDivTest02() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherDivTest02.pdf";
            String outFile = destinationFolder + "keepTogetherDivTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            Rectangle[] columns = new Rectangle[] { new Rectangle(100, 100, 100, 500), new Rectangle(400, 100, 100, 500
                ) };
            doc.SetRenderer(new ColumnDocumentRenderer(doc, columns));
            Div div = new Div();
            doc.Add(new Paragraph("first string"));
            for (int i = 0; i < 130; i++) {
                div.Add(new Paragraph("String number " + i));
            }
            div.SetKeepTogether(true);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherDefaultTest01() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherDefaultTest01.pdf";
            String outFile = destinationFolder + "keepTogetherDefaultTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            Div div = new KeepTogetherTest.KeepTogetherDiv();
            doc.Add(new Paragraph("first string"));
            for (int i = 0; i < 130; i++) {
                div.Add(new Paragraph("String number " + i));
            }
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-1837: NPE")]
        public virtual void KeepTogetherInlineDiv01() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherInlineDiv01.pdf";
            String outFile = destinationFolder + "keepTogetherInlineDiv01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph("first string"));
            Div div = new Div().SetWidth(200);
            for (int i = 0; i < 130; i++) {
                div.Add(new Paragraph("Part of inline div; string number " + i));
            }
            div.SetKeepTogether(true);
            doc.Add(new Paragraph().Add(div));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherInlineDiv02() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherInlineDiv02.pdf";
            String outFile = destinationFolder + "keepTogetherInlineDiv02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph("first string"));
            Div div = new Div().SetWidth(200);
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < 130; i++) {
                buffer.Append("Part #" + i + " of inline div");
            }
            div.Add(new Paragraph(buffer.ToString()));
            div.SetKeepTogether(true);
            doc.Add(new Paragraph().Add(div));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.CLIP_ELEMENT, Count = 8)]
        public virtual void NarrowPageTest01() {
            String testName = "narrowPageTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table tbl = new Table(UnitValue.CreatePointArray(new float[] { 30.0F, 30.0F, 30.0F, 30.0F }));
            tbl.SetWidth(120.0F);
            tbl.SetFont(PdfFontFactory.CreateFont(StandardFonts.COURIER));
            tbl.SetFontSize(8.0F);
            for (int x = 0; x < 12; x++) {
                for (int y = 0; y < 4; y++) {
                    Cell cell = new Cell();
                    cell.Add(new Paragraph("row " + x));
                    cell.SetHeight(10.5f);
                    cell.SetMaxHeight(10.5f);
                    cell.SetKeepTogether(true);
                    tbl.AddCell(cell);
                }
            }
            doc.Add(tbl);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.CLIP_ELEMENT, Count = 2)]
        public virtual void NarrowPageTest02() {
            String testName = "narrowPageTest02.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.SetRenderer(new KeepTogetherTest.SpecialOddPagesDocumentRenderer(doc, new PageSize(102.0F, 132.0F)));
            Paragraph p = new Paragraph("row 10");
            Div div = new Div();
            div.Add(p);
            div.SetKeepTogether(true);
            doc.Add(new Paragraph("a"));
            doc.Add(div);
            doc.Add(new AreaBreak());
            div.SetHeight(30);
            doc.Add(new Paragraph("a"));
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Add(new AreaBreak());
            div.DeleteOwnProperty(Property.HEIGHT);
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Add(new AreaBreak());
            div.SetHeight(30);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void NarrowPageTest02A() {
            String testName = "narrowPageTest02A.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.SetRenderer(new KeepTogetherTest.SpecialOddPagesDocumentRenderer(doc, new PageSize(102.0F, 102.0F)));
            Paragraph p = new Paragraph("row 10");
            p.SetKeepTogether(true);
            doc.Add(new Paragraph("a"));
            doc.Add(p);
            doc.Add(new AreaBreak());
            p.SetHeight(30);
            doc.Add(new Paragraph("a"));
            doc.Add(p);
            doc.Add(new AreaBreak());
            doc.Add(new AreaBreak());
            p.DeleteOwnProperty(Property.HEIGHT);
            doc.Add(p);
            doc.Add(new AreaBreak());
            doc.Add(new AreaBreak());
            p.SetHeight(30);
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
        public virtual void UpdateHeightTest01() {
            String testName = "updateHeightTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            pdfDoc.SetDefaultPageSize(new PageSize(102.0F, 102.0F));
            Document doc = new Document(pdfDoc);
            Div div = new Div();
            div.SetBackgroundColor(ColorConstants.RED);
            div.Add(new Paragraph("row"));
            div.Add(new Paragraph("row 10"));
            div.SetKeepTogether(true);
            div.SetHeight(100);
            doc.Add(new Paragraph("a"));
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.CLIP_ELEMENT, Count = 1)]
        [LogMessage(iText.IO.LogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED, Count = 22)]
        public virtual void PartialTest01() {
            //TODO DEVSIX-1977
            String testName = "partialTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            pdfDoc.SetDefaultPageSize(PageSize.A7);
            Document doc = new Document(pdfDoc);
            Div div = new Div();
            div.SetBackgroundColor(ColorConstants.RED);
            div.SetKeepTogether(true);
            div.SetHeight(200);
            for (int i = 0; i < 30; i++) {
                div.Add(new Paragraph("row " + i));
            }
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void FixedHeightOverflowTest01() {
            String cmpFileName = sourceFolder + "cmp_fixedHeightOverflowTest01.pdf";
            String outFile = destinationFolder + "fixedHeightOverflowTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph("first string"));
            // specifying height definitely bigger than page height
            int divHeight = 1000;
            // test keep-together processing on height-only overflow for blocks
            Div div = new Div().SetHeight(divHeight).SetBorder(new SolidBorder(3));
            div.SetKeepTogether(true);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void MarginCollapseKeptTogetherDivGoesBackTest01() {
            String cmpFileName = sourceFolder + "cmp_marginCollapseKeptTogetherDivGoesBackTest01.pdf";
            String outFile = destinationFolder + "marginCollapseKeptTogetherDivGoesBackTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            doc.SetProperty(Property.COLLAPSING_MARGINS, true);
            Div div1 = new Div().SetMarginBottom(100).SetBackgroundColor(ColorConstants.RED).SetHeight(300).Add(new Paragraph
                ("Bottom margin: 100"));
            doc.Add(div1);
            Div div2 = new Div().SetMarginTop(300).SetHeight(1000).SetBackgroundColor(ColorConstants.RED).Add(new Paragraph
                ("Top margin: 300"));
            div2.SetKeepTogether(true);
            doc.Add(div2);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void MarginCollapseKeptTogetherDivGoesBackTest02() {
            // TODO DEVSIX-3995 The margin between the divs occupies 100 points instead of 300. After a fix the cmp should be updated
            String cmpFileName = sourceFolder + "cmp_marginCollapseKeptTogetherDivGoesBackTest02.pdf";
            String outFile = destinationFolder + "marginCollapseKeptTogetherDivGoesBackTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            doc.SetProperty(Property.COLLAPSING_MARGINS, true);
            Div div1 = new Div().SetMarginBottom(300).SetBackgroundColor(ColorConstants.RED).SetHeight(300).Add(new Paragraph
                ("Bottom margin: 300"));
            doc.Add(div1);
            Div div2 = new Div().SetMarginTop(100).SetHeight(1000).SetBackgroundColor(ColorConstants.RED).Add(new Paragraph
                ("Top margin: 100"));
            div2.SetKeepTogether(true);
            doc.Add(div2);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherNotEmptyPageTest() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherNotEmptyPageTest.pdf";
            String outFile = destinationFolder + "keepTogetherNotEmptyPageTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            doc.SetProperty(Property.COLLAPSING_MARGINS, true);
            // Make page not empty to trigger KEEP_TOGETHER actual processing
            doc.Add(new Paragraph("Just some content to make this page not empty."));
            // Specifying height definitely bigger than page height
            float innerDivHeight = pdfDoc.GetDefaultPageSize().GetHeight() + 200;
            Div innerDiv = new Div();
            innerDiv.SetBackgroundColor(ColorConstants.RED);
            innerDiv.SetHeight(innerDivHeight);
            // Set KEEP_TOGETHER on inner div
            innerDiv.SetKeepTogether(true);
            doc.Add(innerDiv);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherOnFirstInnerElementNotEmptyPageTest() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherOnFirstInnerElementNotEmptyPageTest.pdf";
            String outFile = destinationFolder + "keepTogetherOnFirstInnerElementNotEmptyPageTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            // Make page not empty to trigger KEEP_TOGETHER actual processing
            doc.Add(new Paragraph("Just some content to make this page not empty."));
            // Specifying height definitely bigger than page height
            float innerDivHeight = pdfDoc.GetDefaultPageSize().GetHeight() + 200;
            Div innerDiv = new Div();
            innerDiv.SetBackgroundColor(ColorConstants.RED);
            innerDiv.SetHeight(innerDivHeight);
            // Set KEEP_TOGETHER on inner div
            innerDiv.SetKeepTogether(true);
            Div outerDiv = new Div();
            outerDiv.Add(innerDiv);
            outerDiv.Add(new Div().SetHeight(200).SetBackgroundColor(ColorConstants.BLUE));
            doc.Add(outerDiv);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void MarginCollapseKeptTogetherGoesOnNextAreaTest01() {
            String cmpFileName = sourceFolder + "cmp_marginCollapseKeptTogetherGoesOnNextAreaTest01.pdf";
            String outFile = destinationFolder + "marginCollapseKeptTogetherGoesOnNextAreaTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            doc.SetProperty(Property.COLLAPSING_MARGINS, true);
            Div div1 = new Div().SetMarginBottom(300).SetBackgroundColor(ColorConstants.RED).SetHeight(300).Add(new Paragraph
                ("Bottom margin: 300"));
            doc.Add(div1);
            Div div2 = new Div().SetMarginTop(100).SetHeight(300).SetBackgroundColor(ColorConstants.RED).Add(new Paragraph
                ("Top margin: 100"));
            div2.SetKeepTogether(true);
            doc.Add(div2);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void MarginCollapseKeptTogetherGoesOnNextAreaTest02() {
            String cmpFileName = sourceFolder + "cmp_marginCollapseKeptTogetherGoesOnNextAreaTest02.pdf";
            String outFile = destinationFolder + "marginCollapseKeptTogetherGoesOnNextAreaTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            doc.SetProperty(Property.COLLAPSING_MARGINS, true);
            Div div1 = new Div().SetMarginBottom(100).SetBackgroundColor(ColorConstants.RED).SetHeight(300).Add(new Paragraph
                ("Bottom margin: 100"));
            doc.Add(div1);
            Div div2 = new Div().SetMarginTop(300).SetHeight(300).SetBackgroundColor(ColorConstants.RED).Add(new Paragraph
                ("Top margin: 300"));
            div2.SetKeepTogether(true);
            doc.Add(div2);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherOnSecondInnerElementNotEmptyPageTest() {
            // TODO DEVSIX-4023 cmp should be updated
            String cmpFileName = sourceFolder + "cmp_keepTogetherOnSecondInnerElementNotEmptyPageTest.pdf";
            String outFile = destinationFolder + "keepTogetherOnSecondInnerElementNotEmptyPageTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            // Make page not empty to trigger KEEP_TOGETHER actual processing
            doc.Add(new Paragraph("Just some content to make this page not empty."));
            // Specifying height definitely bigger than page height
            float innerDivHeight = pdfDoc.GetDefaultPageSize().GetHeight() + 200;
            Div innerDiv = new Div();
            innerDiv.SetBackgroundColor(ColorConstants.RED);
            innerDiv.SetHeight(innerDivHeight);
            // Set KEEP_TOGETHER on inner div
            innerDiv.SetKeepTogether(true);
            Div outerDiv = new Div();
            outerDiv.Add(new Div().SetHeight(200).SetBackgroundColor(ColorConstants.BLUE));
            outerDiv.Add(innerDiv);
            doc.Add(outerDiv);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void SmallFloatInsideKeptTogetherDivTest01() {
            String cmpFileName = sourceFolder + "cmp_smallFloatInsideKeptTogetherDivTest01.pdf";
            String outFile = destinationFolder + "smallFloatInsideKeptTogetherDivTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            // specifying height definitely bigger than page height
            int divHeight = 1000;
            doc.Add(CreateKeptTogetherDivWithSmallFloat(divHeight));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void SmallFloatInsideKeptTogetherDivTest02() {
            String cmpFileName = sourceFolder + "cmp_smallFloatInsideKeptTogetherDivTest02.pdf";
            String outFile = destinationFolder + "smallFloatInsideKeptTogetherDivTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            // add some content, so that the following kept together div will be forced to move forward (and then forced to move back)
            doc.Add(new Paragraph("Hello"));
            // specifying height definitely bigger than page height
            int divHeight = 1000;
            doc.Add(CreateKeptTogetherDivWithSmallFloat(divHeight));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void SmallFloatInsideKeptTogetherParagraphTest01() {
            String cmpFileName = sourceFolder + "cmp_smallFloatInsideKeptTogetherParagraphTest01.pdf";
            String outFile = destinationFolder + "smallFloatInsideKeptTogetherParagraphTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            // specifying height definitely bigger than page height
            int paragraphHeight = 1000;
            doc.Add(CreateKeptTogetherParagraphWithSmallFloat(paragraphHeight));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void SmallFloatInsideKeptTogetherParagraphTest02() {
            String cmpFileName = sourceFolder + "cmp_smallFloatInsideKeptTogetherParagraphTest02.pdf";
            String outFile = destinationFolder + "smallFloatInsideKeptTogetherParagraphTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            // add some content, so that the following kept together div will be forced to move forward (and then forced to move back)
            doc.Add(new Paragraph("Hello"));
            // specifying height definitely bigger than page height
            int paragraphHeight = 1000;
            doc.Add(CreateKeptTogetherParagraphWithSmallFloat(paragraphHeight));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherOnInnerElementTestEmptyPageTest() {
            // TODO DEVSIX-4023 cmp should be updated
            String cmpFileName = sourceFolder + "cmp_keepTogetherOnInnerElementTestEmptyPageTest.pdf";
            String outFile = destinationFolder + "keepTogetherOnInnerElementTestEmptyPageTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            doc.SetProperty(Property.COLLAPSING_MARGINS, true);
            bool first = false;
            AddDivs(doc, 200, new Style(), new Style(), first);
            // Specifying height definitely bigger than page height
            float innerDivHeight = pdfDoc.GetDefaultPageSize().GetHeight() + 200;
            AddDivs(doc, innerDivHeight, new Style(), new Style(), first);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherOnInnerElementMargin01EmptyPageTest() {
            // TODO DEVSIX-4023 cmp should be updated
            String cmpFileName = sourceFolder + "cmp_keepTogetherOnInnerElementMargin01EmptyPageTest.pdf";
            String outFile = destinationFolder + "keepTogetherOnInnerElementMargin01EmptyPageTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            doc.SetProperty(Property.COLLAPSING_MARGINS, true);
            bool first = false;
            Style inner = new Style().SetMargin(40);
            Style predefined = new Style().SetMargin(20);
            AddDivs(doc, 200, inner, predefined, first);
            // Specifying height definitely bigger than page height
            float innerDivHeight = pdfDoc.GetDefaultPageSize().GetHeight() + 200;
            AddDivs(doc, innerDivHeight, inner, predefined, first);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherOnInnerElementMargin02EmptyPageTest() {
            // TODO DEVSIX-4023 cmp should be updated
            String cmpFileName = sourceFolder + "cmp_keepTogetherOnInnerElementMargin02EmptyPageTest.pdf";
            String outFile = destinationFolder + "keepTogetherOnInnerElementMargin02EmptyPageTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            doc.SetProperty(Property.COLLAPSING_MARGINS, true);
            bool first = false;
            Style inner = new Style().SetMargin(20);
            Style predefined = new Style().SetMargin(40);
            AddDivs(doc, 200, inner, predefined, first);
            // Specifying height definitely bigger than page height
            float innerDivHeight = pdfDoc.GetDefaultPageSize().GetHeight() + 200;
            AddDivs(doc, innerDivHeight, inner, predefined, first);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        [LogMessage(iText.IO.LogMessageConstant.RECTANGLE_HAS_NEGATIVE_OR_ZERO_SIZES)]
        public virtual void SmallFloatInsideKeptTogetherTableTest01() {
            String cmpFileName = sourceFolder + "cmp_smallFloatInsideKeptTogetherTableTest01.pdf";
            String outFile = destinationFolder + "smallFloatInsideKeptTogetherTableTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            // specifying num of rows which will definitely occupy more space than page height
            int numOfRows = 20;
            doc.Add(CreateKeptTogetherTableWithSmallFloat(numOfRows));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void SmallFloatInsideKeptTogetherTableTest02() {
            String cmpFileName = sourceFolder + "cmp_smallFloatInsideKeptTogetherTableTest02.pdf";
            String outFile = destinationFolder + "smallFloatInsideKeptTogetherTableTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            // add some content, so that the following kept together div will be forced to move forward (and then forced to move back)
            doc.Add(new Paragraph("Hello"));
            // specifying num of rows which will definitely occupy more space than page height
            int numOfRows = 20;
            doc.Add(CreateKeptTogetherTableWithSmallFloat(numOfRows));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        private static Div CreateKeptTogetherDivWithSmallFloat(int divHeight) {
            // test keep-together processing on height-only overflow for blocks
            Div div = new Div().SetHeight(divHeight).SetBorder(new SolidBorder(3));
            div.SetKeepTogether(true);
            Div floatDiv = new Div();
            floatDiv.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            floatDiv.SetHeight(50);
            floatDiv.SetWidth(50);
            floatDiv.SetBackgroundColor(ColorConstants.RED);
            div.Add(floatDiv);
            return div;
        }

        private static Paragraph CreateKeptTogetherParagraphWithSmallFloat(int paragraphHeight) {
            // test keep-together processing on height-only overflow for blocks
            Paragraph paragraph = new Paragraph().SetHeight(paragraphHeight).SetBorder(new SolidBorder(3));
            paragraph.SetKeepTogether(true);
            Div floatDiv = new Div();
            floatDiv.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            floatDiv.SetHeight(50);
            floatDiv.SetWidth(50);
            floatDiv.SetBackgroundColor(ColorConstants.RED);
            paragraph.Add(floatDiv);
            return paragraph;
        }

        private static Table CreateKeptTogetherTableWithSmallFloat(int numOfRows) {
            // test keep-together processing on height-only overflow for blocks
            Table table = new Table(1).SetBorder(new SolidBorder(3)).UseAllAvailableWidth();
            table.SetKeepTogether(true);
            Div floatDiv = new Div();
            floatDiv.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            floatDiv.SetHeight(50);
            floatDiv.SetWidth(50);
            floatDiv.SetBackgroundColor(ColorConstants.RED);
            for (int i = 0; i < numOfRows; i++) {
                table.AddCell(new Cell().Add(floatDiv));
            }
            return table;
        }

        private static void AddDivs(Document doc, float innerDivHeight, Style inner, Style predefined, bool first) {
            // Make page not empty to trigger KEEP_TOGETHER actual processing
            doc.Add(new Paragraph("Just some content to make this page not empty."));
            Div innerDiv = new Div();
            innerDiv.SetBackgroundColor(ColorConstants.RED);
            innerDiv.SetHeight(innerDivHeight);
            // Set KEEP_TOGETHER on inner div
            innerDiv.SetKeepTogether(true);
            innerDiv.SetHeight(innerDivHeight);
            innerDiv.AddStyle(inner);
            Div outerDiv = new Div();
            outerDiv.SetBorder(new SolidBorder(50));
            if (first) {
                outerDiv.Add(innerDiv);
            }
            outerDiv.Add(new Div().SetHeight(200).SetBackgroundColor(ColorConstants.BLUE).AddStyle(predefined));
            if (!first) {
                outerDiv.Add(innerDiv);
            }
            doc.Add(outerDiv);
            doc.Add(new AreaBreak());
        }

        private class KeepTogetherDiv : Div {
            public override T1 GetDefaultProperty<T1>(int property) {
                if (property == Property.KEEP_TOGETHER) {
                    return (T1)(Object)true;
                }
                return base.GetDefaultProperty<T1>(property);
            }
        }

        private class SpecialOddPagesDocumentRenderer : DocumentRenderer {
            private PageSize firstPageSize;

            public SpecialOddPagesDocumentRenderer(Document document, PageSize firstPageSize)
                : base(document) {
                this.firstPageSize = new PageSize(firstPageSize);
            }

            protected internal override PageSize AddNewPage(PageSize customPageSize) {
                PageSize newPageSize = null;
                switch (currentPageNumber % 2) {
                    case 1: {
                        newPageSize = firstPageSize;
                        break;
                    }

                    case 0:
                    default: {
                        newPageSize = PageSize.A4;
                        break;
                    }
                }
                return base.AddNewPage(newPageSize);
            }
        }
    }
}
