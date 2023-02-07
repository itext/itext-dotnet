/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.IO;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
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

        [NUnit.Framework.Test]
        public virtual void PdfA1bButtonAppearanceTest() {
            // TODO: DEVSIX-3913 update this test after the ticket will be resolved
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
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfFormField button = new PushButtonFormFieldBuilder(doc, "push button").SetWidgetRectangle(rect).SetCaption
                ("push").SetConformanceLevel(PdfAConformanceLevel.PDF_A_1B).CreatePushButton();
            button.SetFont(font).SetFontSize(12);
            form.AddField(button);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException.ALL_THE_FONTS_MUST_BE_EMBEDDED_THIS_ONE_IS_NOT_0
                , "Helvetica"), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA1bButtonAppearanceRegenerateTest() {
            // TODO: DEVSIX-3913 update this test after the ticket will be resolved
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
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfFormField button = new PushButtonFormFieldBuilder(doc, "push button").SetWidgetRectangle(rect).SetCaption
                ("push").SetConformanceLevel(PdfAConformanceLevel.PDF_A_1B).CreatePushButton();
            button.SetFont(font).SetFontSize(12);
            button.RegenerateField();
            form.AddField(button);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException.ALL_THE_FONTS_MUST_BE_EMBEDDED_THIS_ONE_IS_NOT_0
                , "Helvetica"), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA1bButtonAppearanceSetValueTest() {
            // TODO: DEVSIX-3913 update this test after the ticket will be resolved
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
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfFormField button = new PushButtonFormFieldBuilder(doc, "push button").SetWidgetRectangle(rect).SetCaption
                ("push").SetConformanceLevel(PdfAConformanceLevel.PDF_A_1B).CreatePushButton();
            button.SetFont(font).SetFontSize(12);
            button.SetValue("button");
            form.AddField(button);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException.ALL_THE_FONTS_MUST_BE_EMBEDDED_THIS_ONE_IS_NOT_0
                , "Helvetica"), exception.Message);
        }
    }
}
