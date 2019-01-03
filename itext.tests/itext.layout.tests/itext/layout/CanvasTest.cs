using System;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class CanvasTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/CanvasTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/CanvasTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.UNABLE_TO_APPLY_PAGE_DEPENDENT_PROP_UNKNOWN_PAGE_ON_WHICH_ELEMENT_IS_DRAWN
            )]
        public virtual void CanvasNoPageLinkTest() {
            String testName = "canvasNoPageLinkTest";
            String @out = destinationFolder + testName + ".pdf";
            String cmp = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(@out));
            PdfPage page = pdf.AddNewPage();
            Rectangle pageSize = page.GetPageSize();
            PdfCanvas pdfCanvas = new PdfCanvas(page.GetLastContentStream(), page.GetResources(), pdf);
            Rectangle rectangle = new Rectangle(pageSize.GetX() + 36, pageSize.GetTop() - 80, pageSize.GetWidth() - 72
                , 50);
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(pdfCanvas, pdf, rectangle);
            canvas.Add(new Paragraph(new Link("Google link!", PdfAction.CreateURI("https://www.google.com")).SetUnderline
                ().SetFontColor(ColorConstants.BLUE)));
            canvas.Close();
            pdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(@out, cmp, destinationFolder));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CanvasWithPageLinkTest() {
            String testName = "canvasWithPageLinkTest";
            String @out = destinationFolder + testName + ".pdf";
            String cmp = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(@out));
            PdfPage page = pdf.AddNewPage();
            Rectangle pageSize = page.GetPageSize();
            Rectangle rectangle = new Rectangle(pageSize.GetX() + 36, pageSize.GetTop() - 80, pageSize.GetWidth() - 72
                , 50);
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(page, rectangle);
            canvas.Add(new Paragraph(new Link("Google link!", PdfAction.CreateURI("https://www.google.com")).SetUnderline
                ().SetFontColor(ColorConstants.BLUE)));
            canvas.Close();
            pdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(@out, cmp, destinationFolder));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CanvasWithPageEnableTaggingTest01() {
            String testName = "canvasWithPageEnableTaggingTest01";
            String @out = destinationFolder + testName + ".pdf";
            String cmp = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(@out));
            pdf.SetTagged();
            PdfPage page = pdf.AddNewPage();
            Rectangle pageSize = page.GetPageSize();
            Rectangle rectangle = new Rectangle(pageSize.GetX() + 36, pageSize.GetTop() - 80, pageSize.GetWidth() - 72
                , 50);
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(page, rectangle);
            canvas.Add(new Paragraph(new Link("Google link!", PdfAction.CreateURI("https://www.google.com")).SetUnderline
                ().SetFontColor(ColorConstants.BLUE)));
            canvas.Close();
            pdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(@out, cmp, destinationFolder));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.UNABLE_TO_APPLY_PAGE_DEPENDENT_PROP_UNKNOWN_PAGE_ON_WHICH_ELEMENT_IS_DRAWN
            )]
        [LogMessage(iText.IO.LogMessageConstant.PASSED_PAGE_SHALL_BE_ON_WHICH_CANVAS_WILL_BE_RENDERED)]
        public virtual void CanvasWithPageEnableTaggingTest02() {
            String testName = "canvasWithPageEnableTaggingTest02";
            String @out = destinationFolder + testName + ".pdf";
            String cmp = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(@out));
            pdf.SetTagged();
            PdfPage page = pdf.AddNewPage();
            Rectangle pageSize = page.GetPageSize();
            Rectangle rectangle = new Rectangle(pageSize.GetX() + 36, pageSize.GetTop() - 80, pageSize.GetWidth() - 72
                , 50);
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(page, rectangle);
            // This will disable tagging and also prevent annotations addition. Created tagged document is invalid. Expected log message.
            canvas.EnableAutoTagging(null);
            canvas.Add(new Paragraph(new Link("Google link!", PdfAction.CreateURI("https://www.google.com")).SetUnderline
                ().SetFontColor(ColorConstants.BLUE)));
            canvas.Close();
            pdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(@out, cmp, destinationFolder));
        }
    }
}
