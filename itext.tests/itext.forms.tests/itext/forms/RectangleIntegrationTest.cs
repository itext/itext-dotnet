using System;
using iText.Forms.Fields;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class RectangleIntegrationTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/forms/RectangleTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/RectangleTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void InitDestinationFolder() {
            ITextTest.CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CreatePdfWithSignatureFields() {
            String outPdf = DESTINATION_FOLDER + "RectangleTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_RectangleTest.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + "RectangleTest.pdf");
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            for (int i = 0; i <= 3; i++) {
                int rotation = 90 * i;
                PdfPage page = pdfDoc.AddNewPage();
                page.SetRotation(rotation);
                float x = 20;
                float y = 500;
                float width = 100;
                float height = 50;
                float spacing = 50;
                for (int j = 1; j <= 3; j++) {
                    Rectangle rect = new Rectangle(x, y, width, height);
                    String fieldName = "page" + i + "_Signature" + j;
                    PdfFormField signatureField = new SignatureFormFieldBuilder(pdfDoc, fieldName).SetPage(page).SetWidgetRectangle
                        (rect).CreateSignature();
                    form.AddField(signatureField);
                    x += width + spacing;
                }
            }
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }
    }
}
