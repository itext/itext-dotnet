using System;
using iText.Kernel.Geom;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PageResizerTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/kernel/pdf/PageResizerTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PageResizerTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Setup() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void TestPageResizeForTextOnlyDocumentResizer() {
            String inFileName = "simple_pdf.pdf";
            String outFileName = "testPageResizeForTextOnlyDocument.pdf";
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
            String outFileName = "testPageResizeForRotatePage.pdf";
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
            String outFileName = "testPageResizeAspectRatios.pdf";
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
            String inFileName = "gradientTest.pdf";
            String outFileName = "gradientTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(PageSize.A6, PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestGradientsWithAspectRatio() {
            String inFileName = "gradientTest.pdf";
            String outFileName = "gradientAspectTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + inFileName), new PdfWriter(
                DESTINATION_FOLDER + outFileName))) {
                new PageResizer(PageSize.LEDGER, PageResizer.ResizeType.DEFAULT).Resize(pdfDocument.GetPage(1));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + outFileName, SOURCE_FOLDER
                 + "cmp_" + outFileName, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestGradientsWithAspect2Ratio() {
            String inFileName = "gradientTest.pdf";
            String outFileName = "gradientAspect2Test.pdf";
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
    }
}
