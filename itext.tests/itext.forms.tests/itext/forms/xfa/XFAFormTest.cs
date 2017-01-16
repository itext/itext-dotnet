using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms.Xfa {
    public class XFAFormTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/xfa/XFAFormTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/xfa/XFAFormTest/";

        public static readonly String XML = sourceFolder + "xfa.xml";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateEmptyXFAFormTest01() {
            String outFileName = destinationFolder + "createEmptyXFAFormTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_createEmptyXFAFormTest01.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(outFileName));
            XfaForm xfa = new XfaForm(doc);
            XfaForm.SetXfaForm(xfa, doc);
            doc.AddNewPage();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateEmptyXFAFormTest02() {
            String outFileName = destinationFolder + "createEmptyXFAFormTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_createEmptyXFAFormTest02.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(outFileName));
            XfaForm xfa = new XfaForm();
            XfaForm.SetXfaForm(xfa, doc);
            doc.AddNewPage();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateXFAFormTest() {
            String outFileName = destinationFolder + "createXFAFormTest.pdf";
            String cmpFileName = sourceFolder + "cmp_createXFAFormTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(outFileName));
            XfaForm xfa = new XfaForm(new FileStream(XML, FileMode.Open, FileAccess.Read));
            xfa.Write(doc);
            doc.AddNewPage();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}
