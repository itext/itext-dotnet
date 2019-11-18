using System;
using iText.Forms.Fields;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    public class PdfChoiceFieldTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfCheckBoxFieldTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfCheckBoxFieldTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ChoiceFieldsWithUnicodeTest() {
            String outPdf = destinationFolder + "choiceFieldsWithUnicodeTest.pdf";
            String cmpPdf = sourceFolder + "cmp_choiceFieldsWithUnicodeTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "NotoSansCJKjp-Bold.otf", "Identity-H");
            font.SetSubset(false);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            pdfDoc.AddNewPage();
            // 规
            form.AddField(PdfFormField.CreateComboBox(pdfDoc, new Rectangle(36, 666, 40, 80), "combo1", "\u89c4", new 
                String[] { "\u89c4", "\u89c9" }, font, null).SetBorderColor(ColorConstants.BLACK));
            // 觉
            form.AddField(PdfFormField.CreateComboBox(pdfDoc, new Rectangle(136, 666, 40, 80), "combo2", "\u89c4", new 
                String[] { "\u89c4", "\u89c9" }, font, null).SetValue("\u89c9").SetBorderColor(ColorConstants.BLACK));
            // 规
            form.AddField(PdfFormField.CreateList(pdfDoc, new Rectangle(236, 666, 50, 80), "list1", "\u89c4", new String
                [] { "\u89c4", "\u89c9" }, font, null).SetBorderColor(ColorConstants.BLACK));
            // 觉
            form.AddField(PdfFormField.CreateList(pdfDoc, new Rectangle(336, 666, 50, 80), "list2", "\u89c4", new String
                [] { "\u89c4", "\u89c9" }, font, null).SetValue("\u89c9").SetBorderColor(ColorConstants.BLACK));
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }
    }
}
