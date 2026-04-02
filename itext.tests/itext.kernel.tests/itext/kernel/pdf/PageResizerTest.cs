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
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PageResizerTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/kernel/pdf/PageResizerTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PageResizerTest/";

        private static ICollection<Object[]> AppendModes() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { true }, new Object[] { false } });
        }

        [NUnit.Framework.OneTimeSetUp]
        public static void Setup() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestPageResizeForTextOnlyDocumentResizer(bool appendMode) {
            String inFileName = "simple_pdf.pdf";
            String outFileName = "pageResizeForTextOnlyDocument.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                PageResizer firstPageResizer = new PageResizer(PageSize.A6, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO);
                firstPageResizer.Resize(pdfDocument.GetPage(1));
                PageResizer secondPageResizer = new PageResizer(new PageSize(298, 120), PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO
                    );
                secondPageResizer.Resize(pdfDocument.GetPage(2));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestPageResizeForRotatePage(bool appendMode) {
            String inFileName = "singlePageDocumentWithRotation.pdf";
            String outFileName = "pageResizeForRotatePage.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                PageResizer pageResizer = new PageResizer(PageSize.A6, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO);
                pageResizer.Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestPageResizeAspectRatios(bool appendMode) {
            String inFileName = "10PagesDocumentWithLeafs.pdf";
            String outFileName = "pageResizeAspectRatios.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
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

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestGradients(bool appendMode) {
            String inFileName = "gradient.pdf";
            String outFileName = "gradient.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(PageSize.A6, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestAnnotationBorder(bool appendMode) {
            String inFileName = "annotationBorder.pdf";
            String outFileName = "annotationBorder.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestAnnotationCalloutLine(bool appendMode) {
            String inFileName = "annotationCalloutLine.pdf";
            String outFileName = "annotationCalloutLine.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestAnnotationRichText(bool appendMode) {
            String inFileName = "annotationRichText.pdf";
            String outFileName = "annotationRichText.pdf";
            String outFileNameReverted = "annotationRichTextReverted.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
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

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestAnnotationInkList(bool appendMode) {
            String inFileName = "annotationInkList.pdf";
            String outFileName = "annotationInkList.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestAnnotationLineEndpoint(bool appendMode) {
            String inFileName = "annotationLineEndpoint.pdf";
            String outFileName = "annotationLineEndpoint.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestAnnotationQuadpoints(bool appendMode) {
            String inFileName = "annotationQuadpoints.pdf";
            String outFileName = "annotationQuadpoints.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestAnnotationRd(bool appendMode) {
            String inFileName = "annotationRd.pdf";
            String outFileName = "annotationRd.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestAnnotationVertices(bool appendMode) {
            String inFileName = "annotationVertices.pdf";
            String outFileName = "annotationVertices.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestGradientsWithAspectRatio(bool appendMode) {
            String inFileName = "gradient.pdf";
            String outFileName = "gradientAspect.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(PageSize.LEDGER, PageResizer.ResizeType.DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestGradientsWithAspect2Ratio(bool appendMode) {
            String inFileName = "gradient.pdf";
            String outFileName = "gradientAspect2.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestGradientsType0Function(bool appendMode) {
            String inFileName = "gradientFct0.pdf";
            String outFileName = "gradientFct0.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(PageSize.A6, PageResizer.ResizeType.DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestAcroFormResizeShrink(bool appendMode) {
            String inFileName = "datasheet.pdf";
            String outFileName = "datasheetShrink.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(PageSize.A6, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestAcroFormResizeGrow(bool appendMode) {
            String inFileName = "datasheet.pdf";
            String outFileName = "datasheetGrow.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(PageSize.A3, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestAcroFormResizeStretch(bool appendMode) {
            String inFileName = "datasheet.pdf";
            String outFileName = "datasheetStretch.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(PageSize.LEDGER, PageResizer.ResizeType.DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestGSManipulationPage(bool appendMode) {
            String inFileName = "gsstackmanipulation.pdf";
            String outFileName = "gsstackmanipulation.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(PageSize.A6, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestHorizontalAnchoringLeft(bool appendMode) {
            String inFileName = "squareSource.pdf";
            String outFileName = "haLeft.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
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

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestHorizontalAnchoringCenter(bool appendMode) {
            String inFileName = "squareSource.pdf";
            String outFileName = "haCenter.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                PageResizer resizer = new PageResizer(new PageSize(PageSize.A5.GetHeight(), PageSize.A5.GetWidth()), PageResizer.ResizeType
                    .MAINTAIN_ASPECT_RATIO);
                resizer.SetHorizontalAnchorPoint(PageResizer.HorizontalAnchorPoint.CENTER);
                resizer.Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestHorizontalAnchoringRight(bool appendMode) {
            String inFileName = "squareSource.pdf";
            String outFileName = "haRight.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                PageResizer resizer = new PageResizer(new PageSize(PageSize.A5.GetHeight(), PageSize.A5.GetWidth()), PageResizer.ResizeType
                    .MAINTAIN_ASPECT_RATIO);
                resizer.SetHorizontalAnchorPoint(PageResizer.HorizontalAnchorPoint.RIGHT);
                resizer.Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestVerticalAnchoringTop(bool appendMode) {
            String inFileName = "squareSource.pdf";
            String outFileName = "vaTop.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                PageResizer resizer = new PageResizer(PageSize.A4, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO);
                resizer.SetVerticalAnchorPoint(PageResizer.VerticalAnchorPoint.TOP);
                NUnit.Framework.Assert.AreEqual(PageResizer.VerticalAnchorPoint.TOP, resizer.GetVerticalAnchorPoint());
                resizer.Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestVerticalAnchoringCenter(bool appendMode) {
            String inFileName = "squareSource.pdf";
            String outFileName = "vaCenter.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                PageResizer resizer = new PageResizer(PageSize.A4, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO);
                resizer.SetVerticalAnchorPoint(PageResizer.VerticalAnchorPoint.CENTER);
                resizer.Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestVerticalAnchoringBottom(bool appendMode) {
            String inFileName = "squareSource.pdf";
            String outFileName = "vaBottom.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                PageResizer resizer = new PageResizer(PageSize.A4, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO);
                resizer.SetVerticalAnchorPoint(PageResizer.VerticalAnchorPoint.BOTTOM);
                resizer.Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestFormFieldsDA(bool appendMode) {
            String inFileName = "formFieldsDA.pdf";
            String outFileName = "formFieldsDA.pdf";
            String outFileNameReverted = "formFieldsDAReverted.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
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

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void AnnotationsRightAnchoringTest(bool appendMode) {
            String[] pdfFiles = new String[] { "annotationVertices.pdf", "annotationBorder.pdf", "annotationQuadpoints.pdf"
                , "annotationRd.pdf" };
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            foreach (String pdfFileName in pdfFiles) {
                String outPdf = pdfFileName.JSubstring(0, pdfFileName.Length - 4) + "Right.pdf";
                using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + pdfFileName), CompareTool.CreateTestPdfWriter
                    (DESTINATION_FOLDER + outPdf), props)) {
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

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void AnnotationsTopAnchoringTest(bool appendMode) {
            String[] pdfFiles = new String[] { "annotationVertices.pdf", "annotationBorder.pdf", "annotationQuadpoints.pdf"
                , "annotationRd.pdf" };
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            foreach (String pdfFileName in pdfFiles) {
                String outPdf = pdfFileName.JSubstring(0, pdfFileName.Length - 4) + "Top.pdf";
                using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + pdfFileName), CompareTool.CreateTestPdfWriter
                    (DESTINATION_FOLDER + outPdf), props)) {
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

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestPdfASignatureFieldDefault(bool appendMode) {
            String inFileName = "pdfASignatureFieldDefault.pdf";
            String outFileName = "pdfASignatureFieldDefault.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestPdfASignatureFieldAspect(bool appendMode) {
            String inFileName = "pdfASignatureFieldAspect.pdf";
            String outFileName = "pdfASignatureFieldAspect.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestPdfAFormFieldsDefault(bool appendMode) {
            String inFileName = "pdfAFormFieldsDefault.pdf";
            String outFileName = "pdfAFormFieldsDefault.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestPdfAFormFieldsAspect(bool appendMode) {
            String inFileName = "pdfAFormFieldsAspect.pdf";
            String outFileName = "pdfAFormFieldsAspect.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestPdfUA1ButtonDefault(bool appendMode) {
            String inFileName = "pdfUA1ButtonDefault.pdf";
            String outFileName = "pdfUA1ButtonDefault.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestPdfUA1ButtonAspect(bool appendMode) {
            String inFileName = "pdfUA1ButtonAspect.pdf";
            String outFileName = "pdfUA1ButtonAspect.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestPdfUA2RadioButtonDefault(bool appendMode) {
            String inFileName = "pdfUA2RadioButtonDefault.pdf";
            String outFileName = "pdfUA2RadioButtonDefault.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestPdfUA2RadioButtonAspect(bool appendMode) {
            String inFileName = "pdfUA2RadioButtonAspect.pdf";
            String outFileName = "pdfUA2RadioButtonAspect.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestPdfUA1SignatureField(bool appendMode) {
            String inFileName = "pdfUA1SignatureField.pdf";
            String outFileName = "pdfUA1SignatureField.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestPdfUA2SignatureField(bool appendMode) {
            String inFileName = "pdfUA2SignatureField.pdf";
            String outFileName = "pdfUA2SignatureField.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + outFileName));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestNestedForms(bool appendMode) {
            String inFileName = "nestedForms.pdf";
            String outFileName = "nestedForms.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestNestedMixedXObjectsDefault(bool appendMode) {
            String inFileName = "nestedMixedXObjectsDefault.pdf";
            String outFileName = "nestedMixedXObjectsDefault.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestNestedMixedXObjectsAspect(bool appendMode) {
            String inFileName = "nestedMixedXObjectsAspect.pdf";
            String outFileName = "nestedMixedXObjectsAspect.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestImageDefault(bool appendMode) {
            String inFileName = "imageDefault.pdf";
            String outFileName = "imageDefault.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.TestCaseSource("AppendModes")]
        public virtual void TestImageAspect(bool appendMode) {
            String inFileName = "imageAspect.pdf";
            String outFileName = "imageAspect.pdf";
            StampingProperties props = new StampingProperties();
            if (appendMode) {
                props.UseAppendMode();
            }
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName), props)) {
                new PageResizer(new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight()), PageResizer.ResizeType.
                    MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }
    }
}
