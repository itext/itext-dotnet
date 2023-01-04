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
using iText.Forms.Fields;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FormFieldsTaggingTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/FormFieldsTaggingTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/FormFieldsTaggingTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <summary>Form fields addition to the tagged document.</summary>
        [NUnit.Framework.Test]
        public virtual void FormFieldTaggingTest01() {
            String outFileName = destinationFolder + "taggedPdfWithForms01.pdf";
            String cmpFileName = sourceFolder + "cmp_taggedPdfWithForms01.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.SetTagged();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            AddFormFieldsToDocument(pdfDoc, form);
            pdfDoc.Close();
            CompareOutput(outFileName, cmpFileName);
        }

        /// <summary>Form fields copying from the tagged document.</summary>
        [NUnit.Framework.Test]
        public virtual void FormFieldTaggingTest02() {
            String outFileName = destinationFolder + "taggedPdfWithForms02.pdf";
            String cmpFileName = sourceFolder + "cmp_taggedPdfWithForms02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            pdfDoc.SetTagged();
            pdfDoc.InitializeOutlines();
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDoc, true);
            acroForm.AddField(new CheckBoxFormFieldBuilder(pdfDoc, "TestCheck").SetWidgetRectangle(new Rectangle(36, 560
                , 20, 20)).CreateCheckBox().SetValue("1", true));
            PdfDocument docToCopyFrom = new PdfDocument(new PdfReader(sourceFolder + "cmp_taggedPdfWithForms07.pdf"));
            docToCopyFrom.CopyPagesTo(1, docToCopyFrom.GetNumberOfPages(), pdfDoc, new PdfPageFormCopier());
            pdfDoc.Close();
            CompareOutput(outFileName, cmpFileName);
        }

        /// <summary>Form fields flattening in the tagged document.</summary>
        [NUnit.Framework.Test]
        public virtual void FormFieldTaggingTest03() {
            String outFileName = destinationFolder + "taggedPdfWithForms03.pdf";
            String cmpFileName = sourceFolder + "cmp_taggedPdfWithForms03.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "cmp_taggedPdfWithForms01.pdf"), new PdfWriter
                (outFileName));
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDoc, false);
            acroForm.FlattenFields();
            pdfDoc.Close();
            CompareOutput(outFileName, cmpFileName);
        }

        /// <summary>Removing fields from tagged document.</summary>
        [NUnit.Framework.Test]
        public virtual void FormFieldTaggingTest04() {
            String outFileName = destinationFolder + "taggedPdfWithForms04.pdf";
            String cmpFileName = sourceFolder + "cmp_taggedPdfWithForms04.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "cmp_taggedPdfWithForms01.pdf"), new PdfWriter
                (outFileName));
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDoc, false);
            acroForm.RemoveField("TestCheck");
            acroForm.RemoveField("push");
            pdfDoc.Close();
            CompareOutput(outFileName, cmpFileName);
        }

        /// <summary>Form fields flattening in the tagged document (writer mode).</summary>
        [NUnit.Framework.Test]
        public virtual void FormFieldTaggingTest05() {
            String outFileName = destinationFolder + "taggedPdfWithForms05.pdf";
            String cmpFileName = sourceFolder + "cmp_taggedPdfWithForms05.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            pdfDoc.SetTagged();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            AddFormFieldsToDocument(pdfDoc, form);
            form.FlattenFields();
            pdfDoc.Close();
            CompareOutput(outFileName, cmpFileName);
        }

        /// <summary>Removing fields from tagged document (writer mode).</summary>
        [NUnit.Framework.Test]
        public virtual void FormFieldTaggingTest06() {
            String outFileName = destinationFolder + "taggedPdfWithForms06.pdf";
            String cmpFileName = sourceFolder + "cmp_taggedPdfWithForms06.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            pdfDoc.SetTagged();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            AddFormFieldsToDocument(pdfDoc, form);
            form.RemoveField("TestCheck");
            form.RemoveField("push");
            pdfDoc.Close();
            CompareOutput(outFileName, cmpFileName);
        }

        /// <summary>Addition of the form field at the specific position in tag structure.</summary>
        [NUnit.Framework.Test]
        public virtual void FormFieldTaggingTest07() {
            String outFileName = destinationFolder + "taggedPdfWithForms07.pdf";
            String cmpFileName = sourceFolder + "cmp_taggedPdfWithForms07.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocWithFields.pdf");
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            // Original document is already tagged, so there is no need to mark it as tagged again
            //        pdfDoc.setTagged();
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfButtonFormField pushButton = new PushButtonFormFieldBuilder(pdfDoc, "push").SetWidgetRectangle(new Rectangle
                (36, 650, 40, 20)).SetCaption("Capcha").CreatePushButton();
            TagTreePointer tagPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer();
            tagPointer.MoveToKid(StandardRoles.DIV);
            acroForm.AddField(pushButton);
            pdfDoc.Close();
            CompareOutput(outFileName, cmpFileName);
        }

        private void AddFormFieldsToDocument(PdfDocument pdfDoc, PdfAcroForm acroForm) {
            Rectangle rect = new Rectangle(36, 700, 20, 20);
            Rectangle rect1 = new Rectangle(36, 680, 20, 20);
            PdfButtonFormField group = new RadioFormFieldBuilder(pdfDoc, "TestGroup").CreateRadioGroup();
            group.SetValue("1", true);
            new RadioFormFieldBuilder(pdfDoc).SetWidgetRectangle(rect).CreateRadioButton(group, "1");
            new RadioFormFieldBuilder(pdfDoc).SetWidgetRectangle(rect1).CreateRadioButton(group, "2");
            acroForm.AddField(group);
            PdfButtonFormField pushButton = new PushButtonFormFieldBuilder(pdfDoc, "push").SetWidgetRectangle(new Rectangle
                (36, 650, 40, 20)).SetCaption("Capcha").CreatePushButton();
            PdfButtonFormField checkBox = new CheckBoxFormFieldBuilder(pdfDoc, "TestCheck").SetWidgetRectangle(new Rectangle
                (36, 560, 20, 20)).CreateCheckBox();
            checkBox.SetValue("1", true);
            acroForm.AddField(pushButton);
            acroForm.AddField(checkBox);
        }

        private void CompareOutput(String outFileName, String cmpFileName) {
            CompareTool compareTool = new CompareTool();
            String compareResult = compareTool.CompareTagStructures(outFileName, cmpFileName);
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
            compareResult = compareTool.CompareByContent(outFileName, cmpFileName, destinationFolder, "diff" + outFileName
                );
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }
    }
}
