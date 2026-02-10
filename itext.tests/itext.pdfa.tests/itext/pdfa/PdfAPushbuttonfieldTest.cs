/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.IO;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAPushbuttonfieldTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        private static readonly String CMP_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/cmp/PdfAPushbuttonfieldTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfa/PdfAPushbuttonfieldTest/";

        private static readonly String FONTS_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/fonts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA1bButtonAppearanceTest() {
            String name = "pdfA1b_ButtonAppearanceTest";
            String outPath = DESTINATION_FOLDER + name + ".pdf";
            String cmpPath = CMP_FOLDER + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, outputIntent);
            doc.SetTagged();
            doc.GetCatalog().SetLang(new PdfString("en-US"));
            doc.AddNewPage();
            PdfAcroForm form = PdfFormCreator.GetAcroForm(doc, true);
            Rectangle rect = new Rectangle(36, 626, 100, 40);
            PdfFont font = PdfFontFactory.CreateFont(FONTS_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfFormField button = new PushButtonFormFieldBuilder(doc, "push button").SetWidgetRectangle(rect).SetCaption
                ("push").SetConformance(PdfConformance.PDF_A_1B).CreatePushButton();
            button.SetFont(font).SetFontSize(12);
            form.AddField(button);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, DESTINATION_FOLDER, diff
                ));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPath));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfA1bButtonAppearanceRegenerateTest() {
            String name = "pdfA1b_ButtonAppearanceRegenerateTest";
            String outPath = DESTINATION_FOLDER + name + ".pdf";
            String cmpPath = CMP_FOLDER + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, outputIntent);
            doc.SetTagged();
            doc.GetCatalog().SetLang(new PdfString("en-US"));
            doc.AddNewPage();
            PdfAcroForm form = PdfFormCreator.GetAcroForm(doc, true);
            Rectangle rect = new Rectangle(36, 626, 100, 40);
            PdfFont font = PdfFontFactory.CreateFont(FONTS_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfFormField button = new PushButtonFormFieldBuilder(doc, "push button").SetWidgetRectangle(rect).SetCaption
                ("push").SetConformance(PdfConformance.PDF_A_1B).CreatePushButton();
            button.SetFont(font).SetFontSize(12);
            button.RegenerateField();
            form.AddField(button);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, DESTINATION_FOLDER, diff
                ));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPath));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfA1bButtonAppearanceSetValueTest() {
            String name = "pdfA1b_ButtonAppearanceSetValueTest";
            String outPath = DESTINATION_FOLDER + name + ".pdf";
            String cmpPath = CMP_FOLDER + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, outputIntent);
            doc.SetTagged();
            doc.GetCatalog().SetLang(new PdfString("en-US"));
            doc.AddNewPage();
            PdfAcroForm form = PdfFormCreator.GetAcroForm(doc, true);
            Rectangle rect = new Rectangle(36, 626, 100, 40);
            PdfFont font = PdfFontFactory.CreateFont(FONTS_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfFormField button = new PushButtonFormFieldBuilder(doc, "push button").SetWidgetRectangle(rect).SetCaption
                ("push").SetConformance(PdfConformance.PDF_A_1B).CreatePushButton();
            button.SetFont(font).SetFontSize(12);
            button.SetValue("button");
            form.AddField(button);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, DESTINATION_FOLDER, diff
                ));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPath));
        }
        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    }
}
