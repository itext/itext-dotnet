using System;
using System.Collections.Generic;
using iText.Forms.Fields;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    public class Utf8FormsTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/Utf8FormsTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/Utf8FormsTest/";

        public static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/Utf8FormsTest/NotoSansCJKsc-Regular.otf";

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ReadUtf8FieldName() {
            String filename = sourceFolder + "utf-8-field-name.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            IDictionary<String, PdfFormField> fields = form.GetFormFields();
            pdfDoc.Close();
            foreach (String fldName in fields.Keys) {
                //  لا
                NUnit.Framework.Assert.AreEqual("\u0644\u0627", fldName);
            }
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ReadUtf8TextAnnot() {
            String filename = sourceFolder + "utf-8-text-annot.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            IDictionary<String, PdfFormField> fields = form.GetFormFields();
            pdfDoc.Close();
            foreach (String fldName in fields.Keys) {
                //  福昕 福昕UTF8
                NUnit.Framework.Assert.AreEqual("\u798F\u6615 \u798F\u6615UTF8", fields.Get(fldName).GetValueAsString());
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void WriteUtf8FieldNameAndValue() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + "writeUtf8FieldNameAndValue.pdf"));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfTextFormField field = PdfTextFormField.CreateText(pdfDoc, new Rectangle(99, 753, 425, 15), "", "");
            field.SetFont(PdfFontFactory.CreateFont(FONT, PdfEncodings.IDENTITY_H));
            //  لا
            field.Put(PdfName.T, new PdfString("\u0644\u0627", PdfEncodings.UTF8));
            //  福昕 福昕UTF8
            field.Put(PdfName.V, new PdfString("\u798F\u6615 \u798F\u6615UTF8", PdfEncodings.UTF8));
            field.RegenerateField();
            form.AddField(field);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "writeUtf8FieldNameAndValue.pdf"
                , sourceFolder + "cmp_writeUtf8FieldNameAndValue.pdf", destinationFolder, "diffFieldNameAndValue_"));
        }
    }
}
