using System.IO;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Test;

namespace iText.Signatures {
    public class PdfSignerUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CreateNewSignatureFormFieldInvisibleAnnotationTest() {
            PdfSigner signer = new PdfSigner(new PdfReader(new MemoryStream(CreateDocumentWithoutWidgetAnnotation()), 
                new ReaderProperties().SetPassword("owner".GetBytes())), new ByteArrayOutputStream(), new StampingProperties
                ());
            signer.cryptoDictionary = new PdfSignature();
            signer.appearance.SetPageRect(new Rectangle(100, 100, 0, 0));
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(signer.document, true);
            signer.CreateNewSignatureFormField(acroForm, signer.fieldName);
            PdfFormField formField = acroForm.GetField(signer.fieldName);
            PdfDictionary formFieldDictionary = formField.GetPdfObject();
            NUnit.Framework.Assert.IsNotNull(formFieldDictionary);
            NUnit.Framework.Assert.IsFalse(formFieldDictionary.ContainsKey(PdfName.AP));
        }

        [NUnit.Framework.Test]
        public virtual void CreateNewSignatureFormFieldNotInvisibleAnnotationTest() {
            PdfSigner signer = new PdfSigner(new PdfReader(new MemoryStream(CreateDocumentWithoutWidgetAnnotation()), 
                new ReaderProperties().SetPassword("owner".GetBytes())), new ByteArrayOutputStream(), new StampingProperties
                ());
            signer.cryptoDictionary = new PdfSignature();
            signer.appearance.SetPageRect(new Rectangle(100, 100, 10, 10));
            PdfSigFieldLock fieldLock = new PdfSigFieldLock();
            signer.fieldLock = fieldLock;
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(signer.document, true);
            NUnit.Framework.Assert.AreEqual(fieldLock, signer.CreateNewSignatureFormField(acroForm, signer.fieldName));
            PdfFormField formField = acroForm.GetField(signer.fieldName);
            PdfDictionary formFieldDictionary = formField.GetPdfObject();
            NUnit.Framework.Assert.IsNotNull(formFieldDictionary);
            NUnit.Framework.Assert.IsTrue(formFieldDictionary.ContainsKey(PdfName.AP));
        }

        [NUnit.Framework.Test]
        public virtual void PopulateExistingSignatureFormFieldInvisibleAnnotationTest() {
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            PdfDocument document = new PdfDocument(new PdfWriter(outputStream, new WriterProperties().SetStandardEncryption
                ("user".GetBytes(), "owner".GetBytes(), 0, EncryptionConstants.STANDARD_ENCRYPTION_128)));
            document.AddNewPage();
            PdfWidgetAnnotation widgetAnnotation = new PdfWidgetAnnotation(new Rectangle(100, 100, 0, 0));
            document.GetPage(1).AddAnnotation(widgetAnnotation);
            document.Close();
            PdfSigner signer = new PdfSigner(new PdfReader(new MemoryStream(outputStream.ToArray()), new ReaderProperties
                ().SetPassword("owner".GetBytes())), new ByteArrayOutputStream(), new StampingProperties());
            signer.cryptoDictionary = new PdfSignature();
            signer.appearance.SetPageRect(new Rectangle(100, 100, 0, 0));
            widgetAnnotation = (PdfWidgetAnnotation)signer.document.GetPage(1).GetAnnotations()[0];
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(signer.document, true);
            PdfFormField formField = new PdfSignerUnitTest.ExtendedPdfSignatureFormField(widgetAnnotation, signer.document
                );
            formField.SetFieldName(signer.fieldName);
            acroForm.AddField(formField);
            signer.PopulateExistingSignatureFormField(acroForm);
            formField = acroForm.GetField(signer.fieldName);
            PdfDictionary formFieldDictionary = formField.GetPdfObject();
            NUnit.Framework.Assert.IsNotNull(formFieldDictionary);
            NUnit.Framework.Assert.IsFalse(formFieldDictionary.ContainsKey(PdfName.AP));
        }

        [NUnit.Framework.Test]
        public virtual void PopulateExistingSignatureFormFieldNotInvisibleAnnotationTest() {
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            PdfDocument document = new PdfDocument(new PdfWriter(outputStream, new WriterProperties().SetStandardEncryption
                ("user".GetBytes(), "owner".GetBytes(), 0, EncryptionConstants.STANDARD_ENCRYPTION_128)));
            document.AddNewPage();
            PdfWidgetAnnotation widgetAnnotation = new PdfWidgetAnnotation(new Rectangle(100, 100, 0, 0));
            document.GetPage(1).AddAnnotation(widgetAnnotation);
            document.Close();
            PdfSigner signer = new PdfSigner(new PdfReader(new MemoryStream(outputStream.ToArray()), new ReaderProperties
                ().SetPassword("owner".GetBytes())), new ByteArrayOutputStream(), new StampingProperties());
            signer.cryptoDictionary = new PdfSignature();
            PdfSigFieldLock fieldLock = new PdfSigFieldLock();
            signer.fieldLock = fieldLock;
            signer.appearance.SetPageRect(new Rectangle(100, 100, 10, 10));
            widgetAnnotation = (PdfWidgetAnnotation)signer.document.GetPage(1).GetAnnotations()[0];
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(signer.document, true);
            PdfFormField formField = new PdfSignerUnitTest.ExtendedPdfSignatureFormField(widgetAnnotation, signer.document
                );
            formField.SetFieldName(signer.fieldName);
            acroForm.AddField(formField);
            NUnit.Framework.Assert.AreEqual(signer.PopulateExistingSignatureFormField(acroForm), fieldLock);
            formField = acroForm.GetField(signer.fieldName);
            PdfDictionary formFieldDictionary = formField.GetPdfObject();
            NUnit.Framework.Assert.IsNotNull(formFieldDictionary);
            NUnit.Framework.Assert.IsTrue(formFieldDictionary.ContainsKey(PdfName.AP));
        }

        private static byte[] CreateDocumentWithoutWidgetAnnotation() {
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            PdfDocument document = new PdfDocument(new PdfWriter(outputStream, new WriterProperties().SetStandardEncryption
                ("user".GetBytes(), "owner".GetBytes(), 0, EncryptionConstants.STANDARD_ENCRYPTION_128)));
            document.AddNewPage();
            document.Close();
            return outputStream.ToArray();
        }

        internal class ExtendedPdfSignatureFormField : PdfSignatureFormField {
            public ExtendedPdfSignatureFormField(PdfWidgetAnnotation widgetAnnotation, PdfDocument document)
                : base(widgetAnnotation, document) {
            }
        }
    }
}
