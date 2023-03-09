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
using iText.Commons.Utils;
using iText.Forms.Fields;
using iText.Forms.Fields.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfCheckBoxFieldTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfCheckBoxFieldTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfCheckBoxFieldTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxFontSizeTest01() {
            String outPdf = destinationFolder + "checkBoxFontSizeTest01.pdf";
            String cmpPdf = sourceFolder + "cmp_checkBoxFontSizeTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            pdfDoc.AddNewPage();
            AddCheckBox(pdfDoc, 6, 750, 7, 7);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxFontSizeTest02() {
            String outPdf = destinationFolder + "checkBoxFontSizeTest02.pdf";
            String cmpPdf = sourceFolder + "cmp_checkBoxFontSizeTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            pdfDoc.AddNewPage();
            AddCheckBox(pdfDoc, 0, 730, 7, 7);
            // fallback to default fontsize — 12 is expected.
            AddCheckBox(pdfDoc, -1, 710, 7, 7);
            AddCheckBox(pdfDoc, 0, 640, 20, 20);
            AddCheckBox(pdfDoc, 0, 600, 40, 20);
            AddCheckBox(pdfDoc, 0, 550, 20, 40);
            AddCheckBox(pdfDoc, 0, 520, 5, 5);
            AddCheckBox(pdfDoc, 0, 510, 5, 3);
            AddCheckBox(pdfDoc, 0, 500, 3, 5);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxFontSizeTest03() {
            String outPdf = destinationFolder + "checkBoxFontSizeTest03.pdf";
            String cmpPdf = sourceFolder + "cmp_checkBoxFontSizeTest03.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            pdfDoc.AddNewPage();
            AddCheckBox(pdfDoc, 2, 730, 7, 7);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxFontSizeTest04() {
            String outPdf = destinationFolder + "checkBoxFontSizeTest04.pdf";
            String cmpPdf = sourceFolder + "cmp_checkBoxFontSizeTest04.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            pdfDoc.AddNewPage();
            AddCheckBox(pdfDoc, 0, 730, 10, new CheckBoxFormFieldBuilder(pdfDoc, "cb_1").SetWidgetRectangle(new Rectangle
                (50, 730, 10, 10)).CreateCheckBox().SetCheckType(CheckBoxType.CIRCLE).SetValue("YES"));
            AddCheckBox(pdfDoc, 0, 700, 10, new CheckBoxFormFieldBuilder(pdfDoc, "cb_2").SetWidgetRectangle(new Rectangle
                (50, 700, 10, 10)).CreateCheckBox().SetCheckType(CheckBoxType.CROSS).SetValue("YES"));
            AddCheckBox(pdfDoc, 0, 670, 10, new CheckBoxFormFieldBuilder(pdfDoc, "cb_3").SetWidgetRectangle(new Rectangle
                (50, 670, 10, 10)).CreateCheckBox().SetCheckType(CheckBoxType.DIAMOND).SetValue("YES"));
            AddCheckBox(pdfDoc, 0, 640, 10, new CheckBoxFormFieldBuilder(pdfDoc, "cb_4").SetWidgetRectangle(new Rectangle
                (50, 640, 10, 10)).CreateCheckBox().SetCheckType(CheckBoxType.SQUARE).SetValue("YES"));
            AddCheckBox(pdfDoc, 0, 610, 10, new CheckBoxFormFieldBuilder(pdfDoc, "cb_5").SetWidgetRectangle(new Rectangle
                (50, 610, 10, 10)).CreateCheckBox().SetCheckType(CheckBoxType.STAR).SetValue("YES"));
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxFontSizeTest05() {
            String outPdf = destinationFolder + "checkBoxFontSizeTest05.pdf";
            String cmpPdf = sourceFolder + "cmp_checkBoxFontSizeTest05.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            pdfDoc.AddNewPage();
            AddCheckBox(pdfDoc, 0, 730, 40, 40);
            AddCheckBox(pdfDoc, 0, 600, 100, 100);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxToggleTest01() {
            String srcPdf = sourceFolder + "checkBoxToggledOn.pdf";
            String outPdf = destinationFolder + "checkBoxToggleTest01.pdf";
            String cmpPdf = sourceFolder + "cmp_checkBoxToggleTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfFormField checkBox = form.GetField("cb_fs_6_7_7");
            checkBox.SetValue("Off");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxToggleTest02() {
            String srcPdf = sourceFolder + "checkBoxToggledOn.pdf";
            String outPdf = destinationFolder + "checkBoxToggleTest02.pdf";
            String cmpPdf = sourceFolder + "cmp_checkBoxToggleTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfFormField checkBox = form.GetField("cb_fs_6_7_7");
            checkBox.SetValue("Off", false);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        private void AddCheckBox(PdfDocument pdfDoc, float fontSize, float yPos, float checkBoxW, float checkBoxH) {
            Rectangle rect = new Rectangle(50, yPos, checkBoxW, checkBoxH);
            AddCheckBox(pdfDoc, fontSize, yPos, checkBoxW, new CheckBoxFormFieldBuilder(pdfDoc, MessageFormatUtil.Format
                ("cb_fs_{0}_{1}_{2}", fontSize, checkBoxW, checkBoxH)).SetWidgetRectangle(rect).CreateCheckBox().SetCheckType
                (CheckBoxType.CHECK).SetValue("YES"));
        }

        private void AddCheckBox(PdfDocument pdfDoc, float fontSize, float yPos, float checkBoxW, PdfFormField checkBox
            ) {
            PdfPage page = pdfDoc.GetFirstPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            if (fontSize >= 0) {
                checkBox.SetFontSize(fontSize);
            }
            checkBox.GetFirstFormAnnotation().SetBorderWidth(1);
            checkBox.GetFirstFormAnnotation().SetBorderColor(ColorConstants.BLACK);
            form.AddField(checkBox, page);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(50 + checkBoxW + 10, yPos).SetFontAndSize(PdfFontFactory.CreateFont
                (), 12).ShowText("okay?").EndText().RestoreState();
        }
    }
}
