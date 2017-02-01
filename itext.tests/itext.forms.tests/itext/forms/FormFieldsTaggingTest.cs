using System;
using iText.Forms.Fields;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
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
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
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
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void FormFieldTaggingTest02() {
            String outFileName = destinationFolder + "taggedPdfWithForms02.pdf";
            String cmpFileName = sourceFolder + "cmp_taggedPdfWithForms02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            pdfDoc.SetTagged();
            pdfDoc.InitializeOutlines();
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDoc, true);
            acroForm.AddField(PdfFormField.CreateCheckBox(pdfDoc, new Rectangle(36, 560, 20, 20), "TestCheck", "1"));
            PdfDocument docToCopyFrom = new PdfDocument(new PdfReader(sourceFolder + "cmp_taggedPdfWithForms07.pdf"));
            docToCopyFrom.CopyPagesTo(1, docToCopyFrom.GetNumberOfPages(), pdfDoc, new PdfPageFormCopier());
            pdfDoc.Close();
            CompareOutput(outFileName, cmpFileName);
        }

        /// <summary>Form fields flattening in the tagged document.</summary>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
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
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
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
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
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
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
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
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
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
            PdfButtonFormField pushButton = PdfFormField.CreatePushButton(pdfDoc, new Rectangle(36, 650, 40, 20), "push"
                , "Capcha");
            TagTreePointer tagPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer();
            tagPointer.MoveToKid(PdfName.Div);
            acroForm.AddField(pushButton);
            pdfDoc.Close();
            CompareOutput(outFileName, cmpFileName);
        }

        private void AddFormFieldsToDocument(PdfDocument pdfDoc, PdfAcroForm acroForm) {
            Rectangle rect = new Rectangle(36, 700, 20, 20);
            Rectangle rect1 = new Rectangle(36, 680, 20, 20);
            PdfButtonFormField group = PdfFormField.CreateRadioGroup(pdfDoc, "TestGroup", "1");
            PdfFormField.CreateRadioButton(pdfDoc, rect, group, "1");
            PdfFormField.CreateRadioButton(pdfDoc, rect1, group, "2");
            acroForm.AddField(group);
            PdfButtonFormField pushButton = PdfFormField.CreatePushButton(pdfDoc, new Rectangle(36, 650, 40, 20), "push"
                , "Capcha");
            PdfButtonFormField checkBox = PdfFormField.CreateCheckBox(pdfDoc, new Rectangle(36, 560, 20, 20), "TestCheck"
                , "1");
            acroForm.AddField(pushButton);
            acroForm.AddField(checkBox);
        }

        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
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
