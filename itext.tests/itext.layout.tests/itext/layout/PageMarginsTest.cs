/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Event;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Properties.Margins;
using iText.Layout.Renderer;
using iText.Layout.Testutil;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PageMarginsTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/PageMarginsTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/PageMarginsTest/";

        private const String TEXT_BYRON = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
             + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
             + "\n" + "To do good to Mankind is the chivalrous plan,\n" + "    And is always as nobly requited;\n"
             + "Then battle for Freedom wherever you can,\n" + "    And, if not shot or hanged, you'll get knighted.";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void PageMarginsComplexTest() {
            String fileName = "pageMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.AddNewPage();
                    IList<PageMarginContent> elements = PageMarginsTestUtil.GetPageMargins1();
                    IList<PageMarginContent> elements2 = PageMarginsTestUtil.GetPageMargins2();
                    Paragraph p = new Paragraph(TEXT_BYRON);
                    for (int i = 0; i < 5; i++) {
                        p.Add(TEXT_BYRON);
                    }
                    SectionBreak sectionBreak = new SectionBreak().SetPageMargins(new PageMarginBoxes(elements));
                    SectionBreak sectionBreak2 = new SectionBreak(new PageMarginBoxes(elements2));
                    Div div1 = new Div();
                    Div div2 = new Div();
                    div1.Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
                    div2.Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
                    document.Add(sectionBreak);
                    document.Add(div1);
                    document.Add(sectionBreak2);
                    document.Add(div2);
                    Div div = new Div().SetBackgroundColor(new DeviceRgb(78, 151, 205)).Add(p).Add(new SectionBreak()).Add(div1
                        ).Add(sectionBreak2).Add(sectionBreak).Add(div2);
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PageSizesTest() {
            String fileName = "pageSizes";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    Paragraph p = new Paragraph(TEXT_BYRON);
                    for (int i = 0; i < 5; i++) {
                        p.Add(TEXT_BYRON);
                    }
                    SectionBreak sectionBreak = new SectionBreak().SetPageSize(PageSize.A4.Rotate());
                    SectionBreak sectionBreak2 = new SectionBreak().SetPageSize(PageSize.A5);
                    Div div1 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
                    Div div2 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
                    document.Add(sectionBreak);
                    document.Add(div1);
                    document.Add(sectionBreak2);
                    document.Add(div2);
                    Div div = new Div().SetBackgroundColor(new DeviceRgb(78, 151, 205)).Add(p).Add(new SectionBreak()).Add(div1
                        ).Add(sectionBreak2).Add(sectionBreak).Add(div2);
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void SectionBreakAfterAreaBreakTest() {
            String fileName = "sectionBreakAfterAreaBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    IList<PageMarginContent> elements = PageMarginsTestUtil.GetPageMargins1();
                    IList<PageMarginContent> elements2 = PageMarginsTestUtil.GetPageMargins2();
                    Paragraph p = new Paragraph(TEXT_BYRON);
                    SectionBreak sectionBreak = new SectionBreak(new PageMarginBoxes(elements));
                    SectionBreak sectionBreak2 = new SectionBreak(new PageMarginBoxes(elements2));
                    Div div1 = new Div();
                    Div div2 = new Div();
                    div1.Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
                    div2.Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
                    document.Add(sectionBreak);
                    document.Add(div1);
                    document.Add(new AreaBreak());
                    document.Add(sectionBreak2);
                    document.Add(div2);
                    Div div = new Div().SetBackgroundColor(new DeviceRgb(78, 151, 205)).Add(sectionBreak).Add(div1).Add(new AreaBreak
                        ()).Add(sectionBreak2).Add(div2);
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void SectionBreakAfterAreaBreakPageSizeTest() {
            String fileName = "sectionBreakAfterAreaBreakPageSize";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    Paragraph p = new Paragraph(TEXT_BYRON);
                    SectionBreak sectionBreak = new SectionBreak(PageSize.A4.Rotate());
                    SectionBreak sectionBreak2 = new SectionBreak(PageSize.A5.Rotate());
                    Div div1 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
                    Div div2 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
                    // Page 1 will be created with the PageSize from sectionBreak.
                    document.Add(sectionBreak);
                    document.Add(div1);
                    // Page 2 will be created with the PageSize from AreaBreak.
                    document.Add(new AreaBreak(PageSize.A5));
                    // Page 3 will be created with the PageSize from sectionBreak2.
                    document.Add(sectionBreak2);
                    document.Add(div2);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void TwoSectionBreaksInARowTest() {
            String fileName = "twoSectionBreaksInARow";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    IList<PageMarginContent> elements = PageMarginsTestUtil.GetPageMargins1();
                    IList<PageMarginContent> elements2 = PageMarginsTestUtil.GetPageMargins2();
                    Paragraph p = new Paragraph(TEXT_BYRON);
                    SectionBreak sectionBreak = new SectionBreak(PageSize.A4.Rotate(), new PageMarginBoxes(elements));
                    SectionBreak sectionBreak2 = new SectionBreak(PageSize.A5, new PageMarginBoxes(elements2));
                    Div div1 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
                    Div div2 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
                    // In such cases we'll add new empty page with page size and page margins from the 1st sectionBreak,
                    // and after one more new page with page size and page margins from the 2nd sectionBreak2.
                    document.Add(div1).Add(sectionBreak).Add(sectionBreak2).Add(div2);
                    Div div = new Div().Add(div1).Add(sectionBreak).Add(sectionBreak2).Add(div2);
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void SectionBreakAfterContentTest() {
            String fileName = "sectionBreakAfterContent";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    IList<PageMarginContent> elements = PageMarginsTestUtil.GetPageMargins1();
                    Paragraph p = new Paragraph(TEXT_BYRON);
                    for (int i = 0; i < 5; i++) {
                        p.Add(TEXT_BYRON);
                    }
                    SectionBreak sectionBreak = new SectionBreak(PageSize.A3.Rotate(), new PageMarginBoxes(elements));
                    Div div1 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
                    Div div2 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
                    document.Add(div1).Add(sectionBreak).Add(div2).Add(new SectionBreak());
                    Div div = new Div().Add(div1).Add(sectionBreak).Add(div2);
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void SectionBreakWithSameMarginsAfterContentTest() {
            String fileName = "sectionBreakWithSameMarginsAfterContent";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    Paragraph p = new Paragraph(TEXT_BYRON);
                    for (int i = 0; i < 5; i++) {
                        p.Add(TEXT_BYRON);
                    }
                    IList<PageMarginContent> pageMargins = PageMarginsTestUtil.GetPageMargins1();
                    SectionBreak sectionBreak = new SectionBreak(new PageMarginBoxes(pageMargins));
                    SectionBreak sectionBreak1 = new SectionBreak(new PageMarginBoxes(pageMargins));
                    SectionBreak sectionBreak2 = new SectionBreak(PageSize.A3, new PageMarginBoxes(pageMargins));
                    Div div1 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
                    Div div2 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
                    document.Add(sectionBreak).Add(div1).Add(sectionBreak1).Add(div2).Add(sectionBreak2).Add(div1);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DifferentSectionBreaksTest() {
            String fileName = "differentSectionBreaks";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    IList<PageMarginContent> elements = PageMarginsTestUtil.GetPageMargins1();
                    IList<PageMarginContent> elements2 = PageMarginsTestUtil.GetPageMargins2();
                    Paragraph p = new Paragraph(TEXT_BYRON);
                    SectionBreak sectionBreak = new SectionBreak(new PageMarginBoxes(elements));
                    SectionBreak sectionBreak2 = new SectionBreak(PageSize.A4.Rotate(), new PageMarginBoxes(elements2));
                    Div div1 = new Div();
                    Div div2 = new Div();
                    div1.Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
                    div2.Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
                    document.Add(sectionBreak).Add(div1).Add(sectionBreak2).Add(div2);
                    Div div = new Div().Add(sectionBreak).Add(div1).Add(sectionBreak2).Add(div2);
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void StaticMarginsTest() {
            String fileName = "staticMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    // Set static margins
                    document.SetMargins(100, 100, 100, 100);
                    IList<PageMarginContent> elements = PageMarginsTestUtil.GetPageMargins1();
                    IList<PageMarginContent> elements3 = new List<PageMarginContent>();
                    elements3.Add(new PageMarginContent(MarginBoxName.BOTTOM, new Div().Add(new Paragraph("TEST BOTTOM MARGIN\nWITH SOME FOOTNOTE"
                        )).SetBackgroundColor(ColorConstants.CYAN).SetMinHeight(50)));
                    Paragraph p = new Paragraph(TEXT_BYRON);
                    for (int i = 0; i < 5; i++) {
                        p.Add(TEXT_BYRON);
                    }
                    Div div1 = new Div();
                    Div div2 = new Div();
                    div1.Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
                    div2.Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
                    document.Add(div1).Add(new SectionBreak(new PageMarginBoxes(elements))).Add(div2).Add(new SectionBreak()).
                        Add(div1).Add(new SectionBreak(new PageMarginBoxes(elements3))).Add(div2);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PageMarginsViaDocumentTest() {
            String fileName = "pageMarginsViaDocument";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    IList<PageMarginContent> elements = PageMarginsTestUtil.GetPageMargins1();
                    IList<PageMarginContent> elements2 = PageMarginsTestUtil.GetPageMargins2();
                    document.SetPageMargins(1, new PageMarginBoxes(elements));
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(elements2));
                    document.SetPageMargins((pageNum) => {
                        if (pageNum % 2 != 0) {
                            IList<PageMarginContent> margins = new List<PageMarginContent>();
                            margins.Add(new PageMarginContent(MarginBoxName.TOP, new Div().Add(new Paragraph("Function is used for Page Margins"
                                )).SetBackgroundColor(ColorConstants.PINK).SetTextAlignment(TextAlignment.CENTER)));
                            margins.Add(new PageMarginContent(MarginBoxName.BOTTOM, new Div().Add(new Paragraph("Page " + pageNum)).SetBackgroundColor
                                (ColorConstants.PINK).SetTextAlignment(TextAlignment.CENTER)));
                            return new PageMarginBoxes(margins);
                        }
                        return null;
                    }
                    );
                    Paragraph p = new Paragraph(TEXT_BYRON);
                    for (int i = 0; i < 5; i++) {
                        p.Add(TEXT_BYRON);
                    }
                    Div div1 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
                    Div div2 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
                    document.Add(div1);
                    document.Add(div2);
                    pdfDocument.AddNewPage();
                    pdfDocument.AddNewPage();
                    pdfDocument.AddNewPage(PageSize.A4.Rotate());
                    pdfDocument.AddNewPage(PageSize.A4.Rotate());
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PageMarginsViaDocumentAndSectionBreakTest() {
            String fileName = "pageMarginsViaDocumentAndSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    IList<PageMarginContent> elements = PageMarginsTestUtil.GetPageMargins1();
                    IList<PageMarginContent> elements2 = PageMarginsTestUtil.GetPageMargins2();
                    document.SetPageMargins((pageNum) => pageNum > 0 && pageNum % 2 == 0, new PageMarginBoxes(elements));
                    SectionBreak sectionBreak = new SectionBreak(new PageMarginBoxes(elements2));
                    Paragraph p = new Paragraph(TEXT_BYRON);
                    for (int i = 0; i < 7; i++) {
                        p.Add(TEXT_BYRON);
                    }
                    Div div1 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
                    Div div2 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
                    document.Add(new Paragraph(TEXT_BYRON));
                    document.Add(sectionBreak);
                    document.Add(div1);
                    document.Add(div2);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PageSizeViaAreaBreakAndSectionBreakTest() {
            String fileName = "pageSizeViaAreaBreakAndSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    SectionBreak sectionBreak = new SectionBreak(PageSize.A5);
                    AreaBreak areaBreak = new AreaBreak(PageSize.A5.Rotate());
                    Paragraph p = new Paragraph(TEXT_BYRON);
                    for (int i = 0; i < 7; i++) {
                        p.Add(TEXT_BYRON);
                    }
                    Div div1 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
                    Div div2 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
                    document.Add(sectionBreak);
                    document.Add(div1);
                    document.Add(areaBreak);
                    document.Add(div2);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FixedPositionTest() {
            String fileName = "fixedPosition";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    IList<PageMarginContent> elements = PageMarginsTestUtil.GetPageMargins1();
                    Paragraph p = new Paragraph(TEXT_BYRON);
                    SectionBreak sectionBreak = new SectionBreak(new PageMarginBoxes(elements));
                    Div div1 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
                    div1.SetFixedPosition(0, 100, 300);
                    Div div2 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
                    document.Add(div1).Add(sectionBreak).Add(div2);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void RelativePositionTest() {
            String fileName = "relativePosition";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.SetTagged();
                    IList<PageMarginContent> elements = PageMarginsTestUtil.GetPageMargins1();
                    Paragraph p = new Paragraph(TEXT_BYRON);
                    SectionBreak sectionBreak = new SectionBreak(new PageMarginBoxes(elements));
                    Div div1 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
                    div1.SetRelativePosition(50, 50, 0, 0);
                    Div div2 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
                    document.Add(div1).Add(sectionBreak).Add(div2);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void StaticPageMarginContentTest() {
            String fileName = "staticPageMarginContent";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    IList<PageMarginContent> elements = JavaUtil.ArraysAsList(new PageMarginContent(MarginBoxName.TOP, 30), new 
                        PageMarginContent(MarginBoxName.RIGHT, 60), new PageMarginContent(MarginBoxName.BOTTOM, 200.5f), new PageMarginContent
                        (MarginBoxName.LEFT, 150));
                    Paragraph p = new Paragraph(TEXT_BYRON);
                    for (int i = 0; i < 5; i++) {
                        p.Add(TEXT_BYRON);
                    }
                    SectionBreak sectionBreak = new SectionBreak(new PageMarginBoxes(elements));
                    Div div1 = new Div();
                    div1.Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
                    document.Add(sectionBreak).Add(div1);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void StaticAndDynamicPageMarginContentTest() {
            String fileName = "staticAndDynamicPageMarginContent";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    IList<PageMarginContent> elements = JavaUtil.ArraysAsList(new PageMarginContent(MarginBoxName.TOP, new Div
                        ().Add(new Paragraph("TEST TOP MARGIN")).SetBackgroundColor(ColorConstants.PINK).SetHeight(100)), new 
                        PageMarginContent(MarginBoxName.RIGHT, new Div().Add(new Paragraph("TEST RIGHT MARGIN").SetBackgroundColor
                        (ColorConstants.YELLOW).SetWidth(150))), new PageMarginContent(MarginBoxName.BOTTOM, 200), new PageMarginContent
                        (MarginBoxName.LEFT, 50));
                    Paragraph p = new Paragraph(TEXT_BYRON);
                    for (int i = 0; i < 5; i++) {
                        p.Add(TEXT_BYRON);
                    }
                    SectionBreak sectionBreak = new SectionBreak(new PageMarginBoxes(elements));
                    Div div1 = new Div();
                    div1.Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
                    document.Add(sectionBreak).Add(div1);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void RegisterPageMarginsHeaderTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            int columnNum = 1;
            String values = "red;loop;long";
            AddFooterTable(columnNum, values, document);
            columnNum = 2;
            values = "footertext;blurb";
            AddFooterTable(columnNum, values, document);
            NUnit.Framework.Assert.DoesNotThrow(() => document.Close());
        }

        private void AddFooterTable(int numColumns, String values, Document document) {
            String[] tableValues = iText.Commons.Utils.StringUtil.Split(values, ";");
            Table tab = new Table(numColumns, false);
            foreach (String tableVal in tableValues) {
                tab.AddCell(tableVal);
            }
            tab.UseAllAvailableWidth();
            PdfDocument pdfDocument = document.GetPdfDocument();
            AbstractPdfDocumentEventHandler handler = new PageMarginsTest.TableHandler(tab, document);
            document.SetMargins(document.GetTopMargin(), 36, 36, 36);
            pdfDocument.AddEventHandler(PdfDocumentEvent.INSERT_PAGE, handler);
        }

        private class TableHandler : AbstractPdfDocumentEventHandler {
            private readonly Table table;

            private readonly Document doc;

            public TableHandler(Table table, Document doc) {
                this.table = table;
                this.doc = doc;
            }

            /// <summary>Returns the table height in float.</summary>
            /// <returns>table height</returns>
            public virtual float GetTableHeight() {
                float height = 0.0f;
                if (table != null) {
                    TableRenderer renderer = (TableRenderer)table.CreateRendererSubTree();
                    renderer.SetParent(new DocumentRenderer(doc));
                    LayoutResult result = renderer.Layout(new LayoutContext(new LayoutArea(0, PageSize.A4)));
                    height = result.GetOccupiedArea().GetBBox().GetHeight();
                }
                return height;
            }

            private void AddTable(Rectangle pageSize, PdfCanvas pdfCanvas) {
                Rectangle rect = new Rectangle((pageSize.GetX() + doc.GetLeftMargin()), (pageSize.GetBottom() + doc.GetBottomMargin
                    ()), (pageSize.GetWidth() - doc.GetRightMargin() - doc.GetLeftMargin()), GetTableHeight());
                using (iText.Layout.Canvas canvasForTable = new iText.Layout.Canvas(pdfCanvas, rect)) {
                    canvasForTable.Add(table);
                }
            }

            /// <summary>Adds content to the bottom of the page.</summary>
            /// <param name="event">event data</param>
            protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event) {
                PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
                PdfDocument pdf = docEvent.GetDocument();
                PdfPage page = docEvent.GetPage();
                Rectangle pageSize = page.GetPageSize();
                PdfCanvas pdfCanvas = new PdfCanvas(page.GetLastContentStream(), page.GetResources(), pdf);
                if (table != null) {
                    AddTable(pageSize, pdfCanvas);
                }
            }
        }
    }
}
