using System;
using System.IO;
using iTextSharp.Forms;
using iTextSharp.Forms.Xfa;
using iTextSharp.Kernel.Pdf;

namespace iTextSharp.Samples.Sandbox.Acroforms
{
    public class FillXFA2 : GenericTest
    {
        public static readonly String DEST = NUnit.Framework.TestContext.CurrentContext.TestDirectory +
                                             "/test/resources/sandbox/acroforms/xfa_form_poland_filled.pdf";

        public static readonly String SRC = NUnit.Framework.TestContext.CurrentContext.TestDirectory +
                                            "/../../resources/pdfs/xfa_form_poland.pdf";
        public static readonly String XML = NUnit.Framework.TestContext.CurrentContext.TestDirectory +
                                            "/../../resources/xml/xfa_form_poland.xml";

        protected override void ManipulatePdf(string dest) {
            PdfReader reader = new PdfReader(SRC);
            reader.SetUnethicalReading(true);
            PdfDocument pdfDoc = new PdfDocument(reader, new PdfWriter(DEST));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            XfaForm xfa = form.GetXfaForm();
            xfa.FillXfaForm(new FileStream(XML, FileMode.Open, FileAccess.Read));
            xfa.Write(pdfDoc);
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public override void Test()
        {
            base.Test();
        }
    }
}
