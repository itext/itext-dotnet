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
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfACheckfieldTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String cmpFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/cmp/PdfACheckfieldTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfACheckfieldTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA1aCheckFieldOffAppearanceTest() {
            String name = "pdfA1a_checkFieldOffAppearance";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1A, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.SetTagged();
            doc.GetCatalog().SetLang(new PdfString("en-US"));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            PdfFormField chk = new CheckBoxFormFieldBuilder(doc, "name").SetWidgetRectangle(new Rectangle(100, 500, 50
                , 50)).SetCheckType(PdfFormField.TYPE_CHECK).SetConformanceLevel(PdfAConformanceLevel.PDF_A_1A).CreateCheckBox
                ().SetValue("Off");
            chk.SetBorderColor(ColorConstants.BLACK);
            chk.SetBorderWidth(1);
            form.AddField(chk);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA1aCheckFieldOnAppearanceTest() {
            String name = "pdfA1a_checkFieldOnAppearance";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1A, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.SetTagged();
            doc.GetCatalog().SetLang(new PdfString("en-US"));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            PdfFormField chk = new CheckBoxFormFieldBuilder(doc, "name").SetWidgetRectangle(new Rectangle(100, 500, 50
                , 50)).SetCheckType(PdfFormField.TYPE_CHECK).SetConformanceLevel(PdfAConformanceLevel.PDF_A_1A).CreateCheckBox
                ().SetValue("On");
            chk.SetBorderColor(ColorConstants.BLACK);
            chk.SetBorderWidth(1);
            form.AddField(chk);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

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
            PdfFormField chk = new CheckBoxFormFieldBuilder(doc, "name").SetWidgetRectangle(new Rectangle(100, 500, 50
                , 50)).SetCheckType(PdfFormField.TYPE_CHECK).SetConformanceLevel(PdfAConformanceLevel.PDF_A_1B).CreateCheckBox
                ().SetValue("Off");
            chk.SetBorderColor(ColorConstants.BLACK);
            chk.SetBorderWidth(1);
            form.AddField(chk);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

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
            PdfFormField chk = new CheckBoxFormFieldBuilder(doc, "name").SetWidgetRectangle(new Rectangle(100, 500, 50
                , 50)).SetCheckType(PdfFormField.TYPE_CHECK).SetConformanceLevel(PdfAConformanceLevel.PDF_A_1B).CreateCheckBox
                ().SetValue("On");
            chk.SetBorderColor(ColorConstants.BLACK);
            chk.SetBorderWidth(1);
            form.AddField(chk);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA2aCheckFieldOffAppearanceTest() {
            String name = "pdfA2a_checkFieldOffAppearance";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2A, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.SetTagged();
            doc.GetCatalog().SetLang(new PdfString("en-US"));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            PdfFormField chk = new CheckBoxFormFieldBuilder(doc, "name").SetWidgetRectangle(new Rectangle(100, 500, 50
                , 50)).SetCheckType(PdfFormField.TYPE_CHECK).SetConformanceLevel(PdfAConformanceLevel.PDF_A_2A).CreateCheckBox
                ().SetValue("Off");
            chk.SetBorderColor(ColorConstants.BLACK);
            chk.SetBorderWidth(1);
            form.AddField(chk);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA2aCheckFieldOnAppearanceTest() {
            String name = "pdfA2a_checkFieldOnAppearance";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2A, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.SetTagged();
            doc.GetCatalog().SetLang(new PdfString("en-US"));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            PdfFormField chk = new CheckBoxFormFieldBuilder(doc, "name").SetWidgetRectangle(new Rectangle(100, 500, 50
                , 50)).SetCheckType(PdfFormField.TYPE_CHECK).SetConformanceLevel(PdfAConformanceLevel.PDF_A_2A).CreateCheckBox
                ().SetValue("On");
            chk.SetBorderColor(ColorConstants.BLACK);
            chk.SetBorderWidth(1);
            form.AddField(chk);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA2bCheckFieldOffAppearanceTest() {
            String name = "pdfA2b_checkFieldOffAppearance";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            PdfFormField chk = new CheckBoxFormFieldBuilder(doc, "name").SetWidgetRectangle(new Rectangle(100, 500, 50
                , 50)).SetCheckType(PdfFormField.TYPE_CHECK).SetConformanceLevel(PdfAConformanceLevel.PDF_A_2B).CreateCheckBox
                ().SetValue("Off");
            chk.SetBorderColor(ColorConstants.BLACK);
            chk.SetBorderWidth(1);
            form.AddField(chk);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA2bCheckFieldOnAppearanceTest() {
            String name = "pdfA2b_checkFieldOnAppearance";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            PdfFormField chk = new CheckBoxFormFieldBuilder(doc, "name").SetWidgetRectangle(new Rectangle(100, 500, 50
                , 50)).SetCheckType(PdfFormField.TYPE_CHECK).SetConformanceLevel(PdfAConformanceLevel.PDF_A_2B).CreateCheckBox
                ().SetValue("On");
            chk.SetBorderColor(ColorConstants.BLACK);
            chk.SetBorderWidth(1);
            form.AddField(chk);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA2uCheckFieldOffAppearanceTest() {
            String name = "pdfA2u_checkFieldOffAppearance";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2U, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            PdfFormField chk = new CheckBoxFormFieldBuilder(doc, "name").SetWidgetRectangle(new Rectangle(100, 500, 50
                , 50)).SetCheckType(PdfFormField.TYPE_CHECK).SetConformanceLevel(PdfAConformanceLevel.PDF_A_2U).CreateCheckBox
                ().SetValue("Off");
            chk.SetBorderColor(ColorConstants.BLACK);
            chk.SetBorderWidth(1);
            form.AddField(chk);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA2uCheckFieldOnAppearanceTest() {
            String name = "pdfA2u_checkFieldOnAppearance";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2U, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            PdfFormField chk = new CheckBoxFormFieldBuilder(doc, "name").SetWidgetRectangle(new Rectangle(100, 500, 50
                , 50)).SetCheckType(PdfFormField.TYPE_CHECK).SetConformanceLevel(PdfAConformanceLevel.PDF_A_2U).CreateCheckBox
                ().SetValue("On");
            chk.SetBorderColor(ColorConstants.BLACK);
            chk.SetBorderWidth(1);
            form.AddField(chk);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA3aCheckFieldOffAppearanceTest() {
            String name = "pdfA3a_checkFieldOffAppearance";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3A, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.SetTagged();
            doc.GetCatalog().SetLang(new PdfString("en-US"));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            PdfFormField chk = new CheckBoxFormFieldBuilder(doc, "name").SetWidgetRectangle(new Rectangle(100, 500, 50
                , 50)).SetCheckType(PdfFormField.TYPE_CHECK).SetConformanceLevel(PdfAConformanceLevel.PDF_A_3A).CreateCheckBox
                ().SetValue("Off");
            chk.SetBorderColor(ColorConstants.BLACK);
            chk.SetBorderWidth(1);
            form.AddField(chk);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA3aCheckFieldOnAppearanceTest() {
            String name = "pdfA3a_checkFieldOnAppearance";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3A, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.SetTagged();
            doc.GetCatalog().SetLang(new PdfString("en-US"));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            PdfFormField chk = new CheckBoxFormFieldBuilder(doc, "name").SetWidgetRectangle(new Rectangle(100, 500, 50
                , 50)).SetCheckType(PdfFormField.TYPE_CHECK).SetConformanceLevel(PdfAConformanceLevel.PDF_A_3A).CreateCheckBox
                ().SetValue("On");
            chk.SetBorderColor(ColorConstants.BLACK);
            chk.SetBorderWidth(1);
            form.AddField(chk);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA3bCheckFieldOffAppearanceTest() {
            String name = "pdfA3b_checkFieldOffAppearance";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            PdfFormField chk = new CheckBoxFormFieldBuilder(doc, "name").SetWidgetRectangle(new Rectangle(100, 500, 50
                , 50)).SetCheckType(PdfFormField.TYPE_CHECK).SetConformanceLevel(PdfAConformanceLevel.PDF_A_3B).CreateCheckBox
                ().SetValue("Off");
            chk.SetBorderColor(ColorConstants.BLACK);
            chk.SetBorderWidth(1);
            form.AddField(chk);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA3bCheckFieldOnAppearanceTest() {
            String name = "pdfA3b_checkFieldOnAppearance";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            PdfFormField chk = new CheckBoxFormFieldBuilder(doc, "name").SetWidgetRectangle(new Rectangle(100, 500, 50
                , 50)).SetCheckType(PdfFormField.TYPE_CHECK).SetConformanceLevel(PdfAConformanceLevel.PDF_A_3B).CreateCheckBox
                ().SetValue("On");
            chk.SetBorderColor(ColorConstants.BLACK);
            chk.SetBorderWidth(1);
            form.AddField(chk);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA3uCheckFieldOffAppearanceTest() {
            String name = "pdfA3u_checkFieldOffAppearance";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3U, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            PdfFormField chk = new CheckBoxFormFieldBuilder(doc, "name").SetWidgetRectangle(new Rectangle(100, 500, 50
                , 50)).SetCheckType(PdfFormField.TYPE_CHECK).SetConformanceLevel(PdfAConformanceLevel.PDF_A_3U).CreateCheckBox
                ().SetValue("Off");
            chk.SetBorderColor(ColorConstants.BLACK);
            chk.SetBorderWidth(1);
            form.AddField(chk);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA3uCheckFieldOnAppearanceTest() {
            String name = "pdfA3u_checkFieldOnAppearance";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = cmpFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfWriter writer = new PdfWriter(outPath);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3U, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            PdfFormField chk = new CheckBoxFormFieldBuilder(doc, "name").SetWidgetRectangle(new Rectangle(100, 500, 50
                , 50)).SetCheckType(PdfFormField.TYPE_CHECK).SetConformanceLevel(PdfAConformanceLevel.PDF_A_3U).CreateCheckBox
                ().SetValue("On");
            chk.SetBorderColor(ColorConstants.BLACK);
            chk.SetBorderWidth(1);
            form.AddField(chk);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }
    }
}
