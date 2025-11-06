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
using iText.Kernel.Geom;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Kernel.Pdf {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PageResizerTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/kernel/pdf/PageResizerTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PageResizerTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Setup() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void TestPageResizeForTextOnlyDocumentResizer() {
            String inFileName = "simple_pdf.pdf";
            String outFileName = "pageResizeForTextOnlyDocument.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                PageResizer firstPageResizer = new PageResizer(PageSize.A6, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO);
                firstPageResizer.Resize(pdfDocument.GetPage(1));
                PageResizer secondPageResizer = new PageResizer(new PageSize(298, 120), PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO
                    );
                secondPageResizer.Resize(pdfDocument.GetPage(2));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestPageResizeForRotatePage() {
            String inFileName = "singlePageDocumentWithRotation.pdf";
            String outFileName = "pageResizeForRotatePage.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                PageResizer pageResizer = new PageResizer(PageSize.A6, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO);
                pageResizer.Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestPageResizeAspectRatios() {
            String inFileName = "10PagesDocumentWithLeafs.pdf";
            String outFileName = "pageResizeAspectRatios.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(PageSize.A6, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
                new PageResizer(PageSize.EXECUTIVE, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage
                    (2));
                new PageResizer(PageSize.EXECUTIVE, PageResizer.ResizeType.DEFAULT).Resize(pdfDocument.GetPage(3));
                new PageResizer(PageSize.LEGAL, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(4
                    ));
                new PageResizer(PageSize.LEGAL, PageResizer.ResizeType.DEFAULT).Resize(pdfDocument.GetPage(5));
                new PageResizer(PageSize.LEDGER, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(
                    6));
                new PageResizer(PageSize.LEDGER, PageResizer.ResizeType.DEFAULT).Resize(pdfDocument.GetPage(7));
                new PageResizer(PageSize.LETTER, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(
                    8));
                new PageResizer(PageSize.LETTER, PageResizer.ResizeType.DEFAULT).Resize(pdfDocument.GetPage(9));
                new PageResizer(new PageSize(PageSize.LEDGER.GetWidth() * 2, PageSize.LEDGER.GetHeight() * 2), PageResizer.ResizeType
                    .MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(10));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestGradients() {
            String inFileName = "gradient.pdf";
            String outFileName = "gradient.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(PageSize.A6, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestAnnotationBorder() {
            String inFileName = "annotationBorder.pdf";
            String outFileName = "annotationBorder.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestAnnotationCalloutLine() {
            String inFileName = "annotationCalloutLine.pdf";
            String outFileName = "annotationCalloutLine.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestAnnotationRichText() {
            String inFileName = "annotationRichText.pdf";
            String outFileName = "annotationRichText.pdf";
            String outFileNameReverted = "annotationRichTextReverted.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            // Reverting
            using (PdfDocument pdfDocument_1 = new PdfDocument(new PdfReader(SOURCE_FOLDER + outFileName), new PdfWriter
                (DESTINATION_FOLDER + outFileNameReverted))) {
                PageResizer resizer = new PageResizer(new PageSize(PageSize.A4), PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO
                    );
                resizer.Resize(pdfDocument_1.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileNameReverted, 
                SOURCE_FOLDER + "cmp_" + outFileNameReverted, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestAnnotationInkList() {
            String inFileName = "annotationInkList.pdf";
            String outFileName = "annotationInkList.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestAnnotationLineEndpoint() {
            String inFileName = "annotationLineEndpoint.pdf";
            String outFileName = "annotationLineEndpoint.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestAnnotationQuadpoints() {
            String inFileName = "annotationQuadpoints.pdf";
            String outFileName = "annotationQuadpoints.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestAnnotationRd() {
            String inFileName = "annotationRd.pdf";
            String outFileName = "annotationRd.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestAnnotationVertices() {
            String inFileName = "annotationVertices.pdf";
            String outFileName = "annotationVertices.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestGradientsWithAspectRatio() {
            String inFileName = "gradient.pdf";
            String outFileName = "gradientAspect.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(PageSize.LEDGER, PageResizer.ResizeType.DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestGradientsWithAspect2Ratio() {
            String inFileName = "gradient.pdf";
            String outFileName = "gradientAspect2.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestGradientsType0Function() {
            String inFileName = "gradientFct0.pdf";
            String outFileName = "gradientFct0.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(PageSize.A6, PageResizer.ResizeType.DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestAcroFormResizeShrink() {
            String inFileName = "datasheet.pdf";
            String outFileName = "datasheetShrink.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(PageSize.A6, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestAcroFormResizeGrow() {
            String inFileName = "datasheet.pdf";
            String outFileName = "datasheetGrow.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(PageSize.A3, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestAcroFormResizeStretch() {
            String inFileName = "datasheet.pdf";
            String outFileName = "datasheetStretch.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(PageSize.LEDGER, PageResizer.ResizeType.DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestGSManipulationPage() {
            String inFileName = "gsstackmanipulation.pdf";
            String outFileName = "gsstackmanipulation.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(PageSize.A6, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestHorizontalAnchoringLeft() {
            String inFileName = "squareSource.pdf";
            String outFileName = "haLeft.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                PageResizer resizer = new PageResizer(new PageSize(PageSize.A5.GetHeight(), PageSize.A5.GetWidth()), PageResizer.ResizeType
                    .MAINTAIN_ASPECT_RATIO);
                resizer.SetHorizontalAnchorPoint(PageResizer.HorizontalAnchorPoint.LEFT);
                NUnit.Framework.Assert.AreEqual(PageResizer.HorizontalAnchorPoint.LEFT, resizer.GetHorizontalAnchorPoint()
                    );
                resizer.Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestHorizontalAnchoringCenter() {
            String inFileName = "squareSource.pdf";
            String outFileName = "haCenter.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                PageResizer resizer = new PageResizer(new PageSize(PageSize.A5.GetHeight(), PageSize.A5.GetWidth()), PageResizer.ResizeType
                    .MAINTAIN_ASPECT_RATIO);
                resizer.SetHorizontalAnchorPoint(PageResizer.HorizontalAnchorPoint.CENTER);
                resizer.Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestHorizontalAnchoringRight() {
            String inFileName = "squareSource.pdf";
            String outFileName = "haRight.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                PageResizer resizer = new PageResizer(new PageSize(PageSize.A5.GetHeight(), PageSize.A5.GetWidth()), PageResizer.ResizeType
                    .MAINTAIN_ASPECT_RATIO);
                resizer.SetHorizontalAnchorPoint(PageResizer.HorizontalAnchorPoint.RIGHT);
                resizer.Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestVerticalAnchoringTop() {
            String inFileName = "squareSource.pdf";
            String outFileName = "vaTop.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                PageResizer resizer = new PageResizer(PageSize.A4, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO);
                resizer.SetVerticalAnchorPoint(PageResizer.VerticalAnchorPoint.TOP);
                NUnit.Framework.Assert.AreEqual(PageResizer.VerticalAnchorPoint.TOP, resizer.GetVerticalAnchorPoint());
                resizer.Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestVerticalAnchoringCenter() {
            String inFileName = "squareSource.pdf";
            String outFileName = "vaCenter.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                PageResizer resizer = new PageResizer(PageSize.A4, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO);
                resizer.SetVerticalAnchorPoint(PageResizer.VerticalAnchorPoint.CENTER);
                resizer.Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestVerticalAnchoringBottom() {
            String inFileName = "squareSource.pdf";
            String outFileName = "vaBottom.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                PageResizer resizer = new PageResizer(PageSize.A4, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO);
                resizer.SetVerticalAnchorPoint(PageResizer.VerticalAnchorPoint.BOTTOM);
                resizer.Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestFormFieldsDA() {
            String inFileName = "formFieldsDA.pdf";
            String outFileName = "formFieldsDA.pdf";
            String outFileNameReverted = "formFieldsDAReverted.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                PageResizer resizer = new PageResizer(new PageSize(1200, 1200), PageResizer.ResizeType.DEFAULT);
                resizer.SetVerticalAnchorPoint(PageResizer.VerticalAnchorPoint.BOTTOM);
                resizer.Resize(pdfDocument.GetPage(1));
                resizer = new PageResizer(new PageSize(1200, 1200), PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO);
                resizer.SetVerticalAnchorPoint(PageResizer.VerticalAnchorPoint.BOTTOM);
                resizer.Resize(pdfDocument.GetPage(2));
                resizer = new PageResizer(new PageSize(400, 400), PageResizer.ResizeType.DEFAULT);
                resizer.SetVerticalAnchorPoint(PageResizer.VerticalAnchorPoint.BOTTOM);
                resizer.Resize(pdfDocument.GetPage(3));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            // Reverting
            using (PdfDocument pdfDocument_1 = new PdfDocument(new PdfReader(SOURCE_FOLDER + outFileName), new PdfWriter
                (DESTINATION_FOLDER + outFileNameReverted))) {
                PageResizer resizer = new PageResizer(new PageSize(PageSize.A4), PageResizer.ResizeType.DEFAULT);
                resizer.SetVerticalAnchorPoint(PageResizer.VerticalAnchorPoint.BOTTOM);
                resizer.Resize(pdfDocument_1.GetPage(1));
                resizer = new PageResizer(new PageSize(PageSize.A4), PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO);
                resizer.SetVerticalAnchorPoint(PageResizer.VerticalAnchorPoint.BOTTOM);
                resizer.Resize(pdfDocument_1.GetPage(2));
                resizer = new PageResizer(new PageSize(PageSize.A4), PageResizer.ResizeType.DEFAULT);
                resizer.SetVerticalAnchorPoint(PageResizer.VerticalAnchorPoint.BOTTOM);
                resizer.Resize(pdfDocument_1.GetPage(3));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileNameReverted, 
                SOURCE_FOLDER + "cmp_" + outFileNameReverted, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationsRightAnchoringTest() {
            String[] pdfFiles = new String[] { "annotationVertices.pdf", "annotationBorder.pdf", "annotationQuadpoints.pdf"
                , "annotationRd.pdf" };
            foreach (String pdfFileName in pdfFiles) {
                String outPdf = pdfFileName.JSubstring(0, pdfFileName.Length - 4) + "Right.pdf";
                using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + pdfFileName), CompareTool.CreateTestPdfWriter
                    (DESTINATION_FOLDER + outPdf))) {
                    PageResizer pr = new PageResizer(new PageSize(PageSize.A4.GetWidth() * 2, PageSize.A4.GetHeight()), PageResizer.ResizeType
                        .MAINTAIN_ASPECT_RATIO);
                    pr.SetVerticalAnchorPoint(PageResizer.VerticalAnchorPoint.CENTER);
                    pr.SetHorizontalAnchorPoint(PageResizer.HorizontalAnchorPoint.RIGHT);
                    pr.Resize(pdfDocument.GetPage(1));
                }
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outPdf, SOURCE_FOLDER
                     + "cmp_" + outPdf, DESTINATION_FOLDER, "diff"));
            }
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationsTopAnchoringTest() {
            String[] pdfFiles = new String[] { "annotationVertices.pdf", "annotationBorder.pdf", "annotationQuadpoints.pdf"
                , "annotationRd.pdf" };
            foreach (String pdfFileName in pdfFiles) {
                String outPdf = pdfFileName.JSubstring(0, pdfFileName.Length - 4) + "Top.pdf";
                using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + pdfFileName), CompareTool.CreateTestPdfWriter
                    (DESTINATION_FOLDER + outPdf))) {
                    PageResizer pr = new PageResizer(new PageSize(PageSize.A4.GetWidth(), PageSize.A4.GetHeight() * 2), PageResizer.ResizeType
                        .MAINTAIN_ASPECT_RATIO);
                    pr.SetVerticalAnchorPoint(PageResizer.VerticalAnchorPoint.TOP);
                    pr.SetHorizontalAnchorPoint(PageResizer.HorizontalAnchorPoint.CENTER);
                    pr.Resize(pdfDocument.GetPage(1));
                }
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outPdf, SOURCE_FOLDER
                     + "cmp_" + outPdf, DESTINATION_FOLDER, "diff"));
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfASignatureFieldDefault() {
            String inFileName = "pdfASignatureFieldDefault.pdf";
            String outFileName = "pdfASignatureFieldDefault.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void TestPdfASignatureFieldAspect() {
            String inFileName = "pdfASignatureFieldAspect.pdf";
            String outFileName = "pdfASignatureFieldAspect.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void TestPdfAFormFieldsDefault() {
            String inFileName = "pdfAFormFieldsDefault.pdf";
            String outFileName = "pdfAFormFieldsDefault.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void TestPdfAFormFieldsAspect() {
            String inFileName = "pdfAFormFieldsAspect.pdf";
            String outFileName = "pdfAFormFieldsAspect.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void TestPdfUA1ButtonDefault() {
            String inFileName = "pdfUA1ButtonDefault.pdf";
            String outFileName = "pdfUA1ButtonDefault.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void TestPdfUA1ButtonAspect() {
            String inFileName = "pdfUA1ButtonAspect.pdf";
            String outFileName = "pdfUA1ButtonAspect.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void TestPdfUA2RadioButtonDefault() {
            String inFileName = "pdfUA2RadioButtonDefault.pdf";
            String outFileName = "pdfUA2RadioButtonDefault.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void TestPdfUA2RadioButtonAspect() {
            String inFileName = "pdfUA2RadioButtonAspect.pdf";
            String outFileName = "pdfUA2RadioButtonAspect.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void TestPdfUA1SignatureField() {
            String inFileName = "pdfUA1SignatureField.pdf";
            String outFileName = "pdfUA1SignatureField.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void TestPdfUA2SignatureField() {
            String inFileName = "pdfUA2SignatureField.pdf";
            String outFileName = "pdfUA2SignatureField.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void TestNestedForms() {
            String inFileName = "nestedForms.pdf";
            String outFileName = "nestedForms.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestNestedMixedXObjectsDefault() {
            String inFileName = "nestedMixedXObjectsDefault.pdf";
            String outFileName = "nestedMixedXObjectsDefault.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestNestedMixedXObjectsAspect() {
            String inFileName = "nestedMixedXObjectsAspect.pdf";
            String outFileName = "nestedMixedXObjectsAspect.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestImageDefault() {
            String inFileName = "imageDefault.pdf";
            String outFileName = "imageDefault.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestImageAspect() {
            String inFileName = "imageAspect.pdf";
            String outFileName = "imageAspect.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }
    }
}
