/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
Authors: iText Software.

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
