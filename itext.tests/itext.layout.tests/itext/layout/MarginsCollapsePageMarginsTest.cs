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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Properties.Margins;
using iText.Layout.Testutil;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class MarginsCollapsePageMarginsTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/MarginsCollapsePageMarginsTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/MarginsCollapsePageMarginsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsWithSectionBreakTest() {
            String fileName = "collapsingMarginsSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetProperty(Property.COLLAPSING_MARGINS, true);
                    document.Add(MarginedDiv("TOP SIBLING", new DeviceRgb(65, 151, 29), 40, 40));
                    document.Add(MarginedDiv("BOTTOM SIBLING", new DeviceRgb(209, 247, 29), 60, 40));
                    document.Add(MarginedDiv("THIRD SIBLING", new DeviceRgb(78, 151, 205), 30, 30));
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    document.Add(new Paragraph("Page 2 — margins1 active; collapsing still on."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsTwoSectionBreaksTest() {
            String fileName = "collapsingTwoSectionBreaks";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetProperty(Property.COLLAPSING_MARGINS, true);
                    AddSiblingBlock(document, "SECTION 1");
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    AddSiblingBlock(document, "SECTION 2");
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2())));
                    AddSiblingBlock(document, "SECTION 3");
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void ElementLevelCollapsingWithSectionBreakTest() {
            String fileName = "elemCollapsingWithSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div collapsing = new Div();
                    collapsing.SetProperty(Property.COLLAPSING_MARGINS, true);
                    collapsing.Add(MarginedDiv("COLLAPSED A", new DeviceRgb(65, 151, 29), 50, 50));
                    collapsing.Add(MarginedDiv("COLLAPSED B", new DeviceRgb(209, 247, 29), 30, 30));
                    collapsing.Add(MarginedDiv("COLLAPSED C", new DeviceRgb(78, 151, 205), 40, 40));
                    document.Add(collapsing);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2())));
                    Div nonCollapsing = new Div();
                    nonCollapsing.Add(MarginedDiv("NON-COLLAPSED A", new DeviceRgb(65, 151, 29), 50, 50));
                    nonCollapsing.Add(MarginedDiv("NON-COLLAPSED B", new DeviceRgb(209, 247, 29), 30, 30));
                    nonCollapsing.Add(MarginedDiv("NON-COLLAPSED C", new DeviceRgb(78, 151, 205), 40, 40));
                    document.Add(nonCollapsing);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void ParentChildCollapsingWithSectionBreakTest() {
            String fileName = "parentChildCollapsingSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetProperty(Property.COLLAPSING_MARGINS, true);
                    Div parent = new Div().SetMarginTop(60).SetBackgroundColor(new DeviceRgb(220, 220, 220));
                    Div child = MarginedDiv("CHILD (40pt top margin)", new DeviceRgb(65, 151, 29), 40, 20);
                    parent.Add(child);
                    parent.Add(MarginedDiv("SIBLING IN PARENT", new DeviceRgb(209, 247, 29), 30, 30));
                    document.Add(parent);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    document.Add(new Paragraph("Page 2 — margins1 active."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsSameSectionBreakTwiceTest() {
            String fileName = "collapsingSameSectionBreakTwice";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetProperty(Property.COLLAPSING_MARGINS, true);
                    AddSiblingBlock(document, "SECTION 1");
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    AddSiblingBlock(document, "SECTION 2");
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    AddSiblingBlock(document, "SECTION 3");
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsAcrossAreaBreakTest() {
            String fileName = "collapsingAcrossAreaBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetProperty(Property.COLLAPSING_MARGINS, true);
                    document.SetPageMargins((pageNum) => true, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1()));
                    AddSiblingBlock(document, "PAGE 1");
                    document.Add(new AreaBreak());
                    AddSiblingBlock(document, "PAGE 2");
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsAlternatingSectionAndAreaBreaksTest() {
            String fileName = "collapsingAltBreaks";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetProperty(Property.COLLAPSING_MARGINS, true);
                    AddSiblingBlock(document, "PAGE 1 — no margins");
                    document.Add(new AreaBreak());
                    AddSiblingBlock(document, "PAGE 2 — no margins");
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    AddSiblingBlock(document, "PAGE 3 — margins1");
                    document.Add(new AreaBreak());
                    AddSiblingBlock(document, "PAGE 4 — still margins1");
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2())));
                    AddSiblingBlock(document, "PAGE 5 — margins2");
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsAreaBreakWithPageSizeTest() {
            String fileName = "collapsingAreaBreakPageSize";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetProperty(Property.COLLAPSING_MARGINS, true);
                    document.SetPageMargins((pageNum) => true, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2()));
                    AddSiblingBlock(document, "A4 PAGE");
                    document.Add(new AreaBreak(PageSize.A5));
                    AddSiblingBlock(document, "A5 PAGE");
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsWithDocumentPageMarginsTest() {
            String fileName = "collapsingDocPageMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetProperty(Property.COLLAPSING_MARGINS, true);
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2
                        ()));
                    for (int i = 0; i < 5; i++) {
                        document.Add(MarginedDiv("BLOCK " + i, CellColor(i), 50, 50));
                        document.Add(new Paragraph(TestResourceUtil.GetByronStanza()));
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsWithPerPageDocumentMarginsTest() {
            String fileName = "collapsingPerPageDocMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetProperty(Property.COLLAPSING_MARGINS, true);
                    document.SetPageMargins((pageNum) => {
                        IList<PageMarginContent> margins = new List<PageMarginContent>();
                        margins.Add(new PageMarginContent(MarginBoxName.TOP, new Div().Add(new Paragraph("Page " + pageNum)).SetBackgroundColor
                            (ColorConstants.PINK).SetTextAlignment(TextAlignment.CENTER)));
                        return new PageMarginBoxes(margins);
                    }
                    );
                    for (int i = 0; i < 8; i++) {
                        document.Add(MarginedDiv("BLOCK " + i, CellColor(i), 40, 40));
                        document.Add(new Paragraph(TestResourceUtil.GetByronStanza()));
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsDocumentMarginsOverriddenBySectionBreakTest() {
            String fileName = "collapsingDocMarginsOverriddenBySectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetProperty(Property.COLLAPSING_MARGINS, true);
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2
                        ()));
                    AddSiblingBlock(document, "SECTION 1 — even-page margins2 active");
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    AddSiblingBlock(document, "SECTION 2 — margins1 override");
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsWithStaticDocumentMarginsTest() {
            String fileName = "collapsingStaticDocMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetProperty(Property.COLLAPSING_MARGINS, true);
                    document.SetMargins(80, 80, 80, 80);
                    for (int i = 0; i < 3; i++) {
                        document.Add(MarginedDiv("BLOCK " + i, CellColor(i), 50, 50));
                        document.Add(new Paragraph(TestResourceUtil.GetByronStanza()));
                    }
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    for (int i = 3; i < 6; i++) {
                        document.Add(MarginedDiv("BLOCK " + i, CellColor(i), 50, 50));
                        document.Add(new Paragraph(TestResourceUtil.GetByronStanza()));
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void CollapsingOnVsOffWithPageMarginsThrowsTest() {
            String fileName = "collapsingOnVsOff";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => true, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1()));
                    document.SetProperty(Property.COLLAPSING_MARGINS, false);
                    document.Add(new Paragraph("COLLAPSING OFF").SetFontSize(14));
                    AddSiblingBlock(document, "NO COLLAPSE");
                    document.Add(new AreaBreak());
                    document.SetProperty(Property.COLLAPSING_MARGINS, true);
                    Paragraph collapsingOn = new Paragraph("COLLAPSING ON").SetFontSize(14);
                    NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => document.Add(collapsingOn), "Expected NPE when adding a new p element with changed collapse property."
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void NestedDivsCollapsingWithSectionBreakTest() {
            String fileName = "nestedDivsCollapsingSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetProperty(Property.COLLAPSING_MARGINS, true);
                    Div level1 = new Div().SetMarginTop(60);
                    Div level2 = new Div().SetMarginTop(40);
                    Div level3 = MarginedDiv("DEEPEST CHILD", new DeviceRgb(65, 151, 29), 30, 30);
                    level2.Add(level3);
                    level2.Add(MarginedDiv("SIBLING IN L2", new DeviceRgb(209, 247, 29), 20, 20));
                    level1.Add(level2);
                    level1.Add(MarginedDiv("SIBLING IN L1", new DeviceRgb(78, 151, 205), 25, 25));
                    document.Add(level1);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2())));
                    document.Add(new Paragraph("Page 2 — margins2 active."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void NestedDivsCollapsingWithAreaBreakAndDocumentMarginsTest() {
            String fileName = "nestedDivsCollapsingAreaBreakDocMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetProperty(Property.COLLAPSING_MARGINS, true);
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1
                        ()));
                    Div outer = new Div().SetMarginTop(50);
                    outer.Add(MarginedDiv("NESTED A", new DeviceRgb(65, 151, 29), 40, 40));
                    outer.Add(MarginedDiv("NESTED B", new DeviceRgb(209, 247, 29), 30, 30));
                    document.Add(outer);
                    document.Add(new AreaBreak());
                    Div outer2 = new Div().SetMarginTop(50);
                    outer2.Add(MarginedDiv("NESTED C", new DeviceRgb(78, 151, 205), 40, 40));
                    outer2.Add(MarginedDiv("NESTED D", new DeviceRgb(255, 165, 0), 30, 30));
                    document.Add(outer2);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        private static void AddSiblingBlock(Document document, String label) {
            document.Add(MarginedDiv(label + " — A (50/50)", new DeviceRgb(65, 151, 29), 50, 50));
            document.Add(MarginedDiv(label + " — B (20/20)", new DeviceRgb(209, 247, 29), 20, 20));
            document.Add(MarginedDiv(label + " — C (40/40)", new DeviceRgb(78, 151, 205), 40, 40));
        }

        private static Div MarginedDiv(String label, DeviceRgb color, float marginTop, float marginBottom) {
            return new Div().Add(new Paragraph(label)).SetBackgroundColor(color).SetMarginTop(marginTop).SetMarginBottom
                (marginBottom).SetPadding(6);
        }

        private static DeviceRgb CellColor(int index) {
            DeviceRgb[] palette = new DeviceRgb[] { new DeviceRgb(65, 151, 29), new DeviceRgb(209, 247, 29), new DeviceRgb
                (78, 151, 205), new DeviceRgb(255, 165, 0), new DeviceRgb(200, 100, 100), new DeviceRgb(100, 200, 100)
                 };
            return palette[index % palette.Length];
        }
    }
}
