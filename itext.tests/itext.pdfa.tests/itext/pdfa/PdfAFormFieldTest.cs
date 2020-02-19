/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Renderer;
using iText.Test;
using iText.Test.Attributes;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    public class PdfAFormFieldTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfAFormFieldTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void PdfAButtonFieldTest() {
            PdfDocument pdf;
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            String file = "pdfAButtonField.pdf";
            String filename = destinationFolder + file;
            pdf = new PdfADocument(new PdfWriter(new FileStream(filename, FileMode.Create)), PdfAConformanceLevel.PDF_A_1B
                , new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB ICC preference", @is));
            PageSize pageSize = PageSize.LETTER;
            Document doc = new Document(pdf, pageSize);
            PdfFontFactory.Register(sourceFolder + "FreeSans.ttf", sourceFolder + "FreeSans.ttf");
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", true);
            PdfButtonFormField group = PdfFormField.CreateRadioGroup(pdf, "group", "", PdfAConformanceLevel.PDF_A_1B);
            group.SetReadOnly(true);
            Paragraph p = new Paragraph();
            Text t = new Text("supported");
            t.SetFont(font);
            p.Add(t);
            Image ph = new Image(new PdfFormXObject(new Rectangle(10, 10)));
            Paragraph pc = new Paragraph().Add(ph);
            PdfAFormFieldTest.PdfAButtonFieldTestRenderer r = new PdfAFormFieldTest.PdfAButtonFieldTestRenderer(pc, group
                , "v1");
            pc.SetNextRenderer(r);
            p.Add(pc);
            Paragraph pc1 = new Paragraph().Add(ph);
            PdfAFormFieldTest.PdfAButtonFieldTestRenderer r1 = new PdfAFormFieldTest.PdfAButtonFieldTestRenderer(pc, group
                , "v2");
            pc1.SetNextRenderer(r1);
            Paragraph p2 = new Paragraph();
            Text t2 = new Text("supported 2");
            t2.SetFont(font);
            p2.Add(t2).Add(pc1);
            doc.Add(p);
            doc.Add(p2);
            //set generateAppearance param to false to retain custom appearance
            group.SetValue("v1", false);
            PdfAcroForm.GetAcroForm(pdf, true).AddField(group);
            pdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder + "cmp/PdfAFormFieldTest/cmp_"
                 + file, destinationFolder, "diff_"));
        }

        internal class PdfAButtonFieldTestRenderer : ParagraphRenderer {
            private PdfButtonFormField _group;

            private String _value;

            public PdfAButtonFieldTestRenderer(Paragraph para, PdfButtonFormField group, String value)
                : base(para) {
                _group = group;
                _value = value;
            }

            public override void Draw(DrawContext context) {
                int pageNumber = GetOccupiedArea().GetPageNumber();
                Rectangle bbox = GetInnerAreaBBox();
                PdfDocument pdf = context.GetDocument();
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdf, true);
                PdfFormField chk = PdfFormField.CreateRadioButton(pdf, bbox, _group, _value, PdfAConformanceLevel.PDF_A_1B
                    );
                chk.SetPage(pageNumber);
                chk.SetVisibility(PdfFormField.VISIBLE);
                chk.SetBorderColor(ColorConstants.BLACK);
                chk.SetBackgroundColor(ColorConstants.WHITE);
                chk.SetReadOnly(true);
                PdfFormXObject appearance = new PdfFormXObject(bbox);
                PdfCanvas canvas = new PdfCanvas(appearance, pdf);
                canvas.SaveState().MoveTo(bbox.GetLeft(), bbox.GetBottom()).LineTo(bbox.GetRight(), bbox.GetBottom()).LineTo
                    (bbox.GetRight(), bbox.GetTop()).LineTo(bbox.GetLeft(), bbox.GetTop()).LineTo(bbox.GetLeft(), bbox.GetBottom
                    ()).SetLineWidth(1f).Stroke().RestoreState();
                form.AddFieldAppearanceToPage(chk, pdf.GetPage(pageNumber));
                //appearance stream was set, while AS has kept as is, i.e. in Off state.
                chk.SetAppearance(PdfName.N, "v1".Equals(_value) ? _value : "Off", appearance.GetPdfObject());
            }
        }

        [NUnit.Framework.Test]
        public virtual void PdfA1DocWithPdfA1ButtonFieldTest() {
            String name = "pdfA1DocWithPdfA1ButtonField";
            String fileName = destinationFolder + name + ".pdf";
            String cmp = sourceFolder + "cmp/PdfAFormFieldTest/cmp_pdfA1DocWithPdfA1ButtonField.pdf";
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfAConformanceLevel conformanceLevel = PdfAConformanceLevel.PDF_A_1B;
            PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), conformanceLevel, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfFormField emptyField = PdfFormField.CreateEmptyField(pdfDoc, conformanceLevel).SetFieldName("empty");
            emptyField.AddKid(PdfFormField.CreateButton(pdfDoc, new Rectangle(36, 756, 20, 20), PdfAnnotation.PRINT, conformanceLevel
                ).SetFieldName("button").SetValue("hello"));
            form.AddField(emptyField);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, cmp, destinationFolder));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA1DocWithPdfA1CheckBoxFieldTest() {
            String name = "pdfA1DocWithPdfA1CheckBoxField";
            String fileName = destinationFolder + name + ".pdf";
            String cmp = sourceFolder + "cmp/PdfAFormFieldTest/cmp_pdfA1DocWithPdfA1CheckBoxField.pdf";
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfAConformanceLevel conformanceLevel = PdfAConformanceLevel.PDF_A_1B;
            PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), conformanceLevel, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            form.AddField(PdfFormField.CreateCheckBox(pdfDoc, new Rectangle(36, 726, 20, 20), "checkBox", "1", PdfFormField
                .TYPE_STAR, conformanceLevel));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, cmp, destinationFolder));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FIELD_VALUE_IS_NOT_CONTAINED_IN_OPT_ARRAY)]
        public virtual void PdfA1DocWithPdfA1ChoiceFieldTest() {
            String name = "pdfA1DocWithPdfA1ChoiceField";
            String fileName = destinationFolder + name + ".pdf";
            String cmp = sourceFolder + "cmp/PdfAFormFieldTest/cmp_pdfA1DocWithPdfA1ChoiceField.pdf";
            PdfFont fontFreeSans = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", true);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfAConformanceLevel conformanceLevel = PdfAConformanceLevel.PDF_A_1B;
            PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), conformanceLevel, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfArray options = new PdfArray();
            options.Add(new PdfString("Name"));
            options.Add(new PdfString("Surname"));
            form.AddField(PdfFormField.CreateChoice(pdfDoc, new Rectangle(36, 696, 100, 70), "choice", "1", options, 0
                , fontFreeSans, conformanceLevel));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, cmp, destinationFolder));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA1DocWithPdfA1ComboBoxFieldTest() {
            String name = "pdfA1DocWithPdfA1ComboBoxField";
            String fileName = destinationFolder + name + ".pdf";
            String cmp = sourceFolder + "cmp/PdfAFormFieldTest/cmp_pdfA1DocWithPdfA1ComboBoxField.pdf";
            PdfFont fontCJK = PdfFontFactory.CreateFont(sourceFolder + "NotoSansCJKtc-Light.otf", PdfEncodings.IDENTITY_H
                , true);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfAConformanceLevel conformanceLevel = PdfAConformanceLevel.PDF_A_1B;
            PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), conformanceLevel, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            form.AddField(PdfFormField.CreateComboBox(pdfDoc, new Rectangle(156, 616, 70, 70), "combo", "用", new String
                [] { "用", "规", "表" }, fontCJK, conformanceLevel));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, cmp, destinationFolder));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.MULTIPLE_VALUES_ON_A_NON_MULTISELECT_FIELD)]
        public virtual void PdfA1DocWithPdfA1ListFieldTest() {
            String name = "pdfA1DocWithPdfA1ListField";
            String fileName = destinationFolder + name + ".pdf";
            String cmp = sourceFolder + "cmp/PdfAFormFieldTest/cmp_pdfA1DocWithPdfA1ListField.pdf";
            PdfFont fontFreeSans = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", true);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfAConformanceLevel conformanceLevel = PdfAConformanceLevel.PDF_A_1B;
            PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), conformanceLevel, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfChoiceFormField f = PdfFormField.CreateList(pdfDoc, new Rectangle(86, 556, 50, 200), "list", "9", new String
                [] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }, fontFreeSans, conformanceLevel);
            f.SetValue("4");
            f.SetTopIndex(2);
            f.SetListSelected(new String[] { "3", "5" });
            form.AddField(f);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, cmp, destinationFolder));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA1DocWithPdfA1PushButtonFieldTest() {
            String name = "pdfA1DocWithPdfA1PushButtonField";
            String fileName = destinationFolder + name + ".pdf";
            String cmp = sourceFolder + "cmp/PdfAFormFieldTest/cmp_pdfA1DocWithPdfA1PushButtonField.pdf";
            PdfFont fontFreeSans = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", true);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfAConformanceLevel conformanceLevel = PdfAConformanceLevel.PDF_A_1B;
            PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), conformanceLevel, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            form.AddField(PdfFormField.CreatePushButton(pdfDoc, new Rectangle(36, 526, 100, 20), "push button", "Push"
                , fontFreeSans, 12, conformanceLevel));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, cmp, destinationFolder));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA1DocWithPdfA1RadioButtonFieldTest() {
            String name = "pdfA1DocWithPdfA1RadioButtonField";
            String fileName = destinationFolder + name + ".pdf";
            String cmp = sourceFolder + "cmp/PdfAFormFieldTest/cmp_pdfA1DocWithPdfA1RadioButtonField.pdf";
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfAConformanceLevel conformanceLevel = PdfAConformanceLevel.PDF_A_1B;
            PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), conformanceLevel, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfButtonFormField radioGroup = PdfFormField.CreateRadioGroup(pdfDoc, "radio group", "", conformanceLevel);
            PdfFormField.CreateRadioButton(pdfDoc, new Rectangle(36, 496, 20, 20), radioGroup, "1", conformanceLevel).
                SetBorderWidth(2).SetBorderColor(ColorConstants.ORANGE);
            PdfFormField.CreateRadioButton(pdfDoc, new Rectangle(66, 496, 20, 20), radioGroup, "2", conformanceLevel).
                SetBorderWidth(2).SetBorderColor(ColorConstants.ORANGE);
            form.AddField(radioGroup);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, cmp, destinationFolder));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA1DocWithPdfA1TextFieldTest() {
            String name = "pdfA1DocWithPdfA1TextField";
            String fileName = destinationFolder + name + ".pdf";
            String cmp = sourceFolder + "cmp/PdfAFormFieldTest/cmp_pdfA1DocWithPdfA1TextField.pdf";
            PdfFont fontFreeSans = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", true);
            fontFreeSans.SetSubset(false);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfAConformanceLevel conformanceLevel = PdfAConformanceLevel.PDF_A_1B;
            PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), conformanceLevel, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            form.AddField(PdfFormField.CreateText(pdfDoc, new Rectangle(36, 466, 90, 20), "text", "textField", fontFreeSans
                , 12, false, conformanceLevel).SetValue("iText"));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, cmp, destinationFolder));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA1DocWithPdfA1SignatureFieldTest() {
            String name = "pdfA1DocWithPdfA1SignatureField";
            String fileName = destinationFolder + name + ".pdf";
            String cmp = sourceFolder + "cmp/PdfAFormFieldTest/cmp_pdfA1DocWithPdfA1SignatureField.pdf";
            PdfFont fontFreeSans = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", true);
            fontFreeSans.SetSubset(false);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfAConformanceLevel conformanceLevel = PdfAConformanceLevel.PDF_A_1B;
            PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), conformanceLevel, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            form.AddField(PdfFormField.CreateSignature(pdfDoc, conformanceLevel).SetFieldName("signature").SetFont(fontFreeSans
                ).SetFontSize(20));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, cmp, destinationFolder));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
        }
    }
}
