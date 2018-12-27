using System;
using System.IO;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Pdfa {
    public class PdfAPushbuttonfieldTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String cmpFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/cmp/PdfAPushbuttonfieldTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfAPushbuttonfieldTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PdfA1bButtonAppearanceTest() {
            String name = "pdfA1b_ButtonAppearanceTest";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent);
            doc.SetTagged();
            doc.GetCatalog().SetLang(new PdfString("en-US"));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            Rectangle rect = new Rectangle(36, 626, 100, 40);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", true);
            PdfFormField button = PdfFormField.CreatePushButton(doc, rect, "push button", "push", font, 12, PdfAConformanceLevel
                .PDF_A_1B);
            form.AddField(button);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PdfA1bButtonAppearanceRegenerateTest() {
            String name = "pdfA1b_ButtonAppearanceRegenerateTest";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent);
            doc.SetTagged();
            doc.GetCatalog().SetLang(new PdfString("en-US"));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            Rectangle rect = new Rectangle(36, 626, 100, 40);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", true);
            PdfFormField button = PdfFormField.CreatePushButton(doc, rect, "push button", "push", font, 12, PdfAConformanceLevel
                .PDF_A_1B);
            button.RegenerateField();
            form.AddField(button);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PdfA1bButtonAppearanceSetValueTest() {
            String name = "pdfA1b_ButtonAppearanceSetValueTest";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent);
            doc.SetTagged();
            doc.GetCatalog().SetLang(new PdfString("en-US"));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            Rectangle rect = new Rectangle(36, 626, 100, 40);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", true);
            PdfFormField button = PdfFormField.CreatePushButton(doc, rect, "push button", "push", font, 12, PdfAConformanceLevel
                .PDF_A_1B);
            button.SetValue("button");
            form.AddField(button);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }
    }
}
