using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using iTextSharp.Forms;
using iTextSharp.Forms.Xfa;
using iTextSharp.Kernel.Pdf;

namespace iTextSharp.Samples.Sandbox.Acroforms {
    public class ReadXFA : GenericTest {
        public static readonly String DEST = NUnit.Framework.TestContext.CurrentContext.TestDirectory +
                                             "/test/resources/xml/xfa_form_poland.xml";

        public static readonly String SRC = NUnit.Framework.TestContext.CurrentContext.TestDirectory +
                                            "/../../resources/pdfs/xfa_form_poland.pdf";

        protected override void ManipulatePdf(string dest) {
            compareXml = true;

            PdfDocument pdfDoc = new PdfDocument(new PdfReader(SRC));

            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            XfaForm xfa = form.GetXfaForm();

            XElement node = xfa.GetDatasetsNode();
            IEnumerable<XNode> list = node.Nodes();
            foreach (XNode item in list) {
                if (item is XElement && "data".Equals(((XElement) item).Name.LocalName)) {
                    node = (XElement) item;
                    break;
                }
            }
            list = node.Nodes();
            foreach (XNode item in list) {
                if (item is XElement && "movies".Equals(((XElement) item).Name.LocalName)) {
                    node = (XElement) item;
                    break;
                }
            }

            XmlWriterSettings settings = new XmlWriterSettings {
                Indent = true,
                IndentChars = "    "
            };
            XmlWriter writer = XmlWriter.Create(DEST, settings);
            node.WriteTo(writer);
            writer.Close();

            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public override void Test()
        {
            base.Test();
        }
    }
}
