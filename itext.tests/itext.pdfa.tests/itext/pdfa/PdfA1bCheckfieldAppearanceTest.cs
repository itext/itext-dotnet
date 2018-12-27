using System;
using System.IO;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Pdfa {
    public class PdfA1bCheckfieldAppearanceTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String cmpFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/cmp/PdfA1bCheckfieldAppearanceTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA1bCheckfieldAppearanceTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PdfA1bCheckFieldOffAppearanceTest() {
            String name = "pdfA1b_checkFieldOffAppearance";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            PdfFormField chk = PdfFormField.CreateCheckBox(doc, new Rectangle(100, 500, 50, 50), "name", "Off", PdfFormField
                .TYPE_CHECK, PdfAConformanceLevel.PDF_A_1B);
            chk.SetBorderColor(ColorConstants.BLACK);
            chk.SetBorderWidth(1);
            form.AddField(chk);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PdfA1bCheckFieldOnAppearanceTest() {
            String name = "pdfA1b_checkFieldOnAppearance";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            PdfFormField chk = PdfFormField.CreateCheckBox(doc, new Rectangle(100, 500, 50, 50), "name", "On", PdfFormField
                .TYPE_CHECK, PdfAConformanceLevel.PDF_A_1B);
            chk.SetBorderColor(ColorConstants.BLACK);
            chk.SetBorderWidth(1);
            form.AddField(chk);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }
    }
}
