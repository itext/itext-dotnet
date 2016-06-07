using System;
using System.IO;
using iTextSharp.Forms;
using iTextSharp.Forms.Xfa;
using iTextSharp.Kernel.Pdf;

namespace iTextSharp.Samples.Sandbox.Acroforms
{
    public class FillXFA : GenericTest
    {
        public static readonly String DEST = NUnit.Framework.TestContext.CurrentContext.TestDirectory +
                                             "/test/resources/sandbox/acroforms/purchase_order_filled.pdf";

        public static readonly String SRC = NUnit.Framework.TestContext.CurrentContext.TestDirectory +
                                            "/../../resources/pdfs/purchase_order.pdf";
        public static readonly String XML = NUnit.Framework.TestContext.CurrentContext.TestDirectory +
                                            "/../../resources/xml/data.xml";

        protected override void ManipulatePdf(string dest) {
            PdfDocument pdfdoc = new PdfDocument(new PdfReader(SRC), new PdfWriter(DEST));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfdoc, true);
            XfaForm xfa = form.GetXfaForm();
            xfa.FillXfaForm(new FileStream(XML, FileMode.Open, FileAccess.Read));
            xfa.Write(pdfdoc);
            pdfdoc.Close();
        }

        [NUnit.Framework.Test]
        public override void Test()
        {
            base.Test();
        }
    }
}
