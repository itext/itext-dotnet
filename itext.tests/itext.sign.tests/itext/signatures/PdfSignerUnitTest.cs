/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System;
using System.IO;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Pdfa;
using iText.Signatures.Exceptions;
using iText.Signatures.Testutils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class PdfSignerUnitTest : ExtendedITextTest {
        private static readonly byte[] OWNER = "owner".GetBytes();

        private static readonly byte[] USER = "user".GetBytes();

        private static readonly String PDFA_RESOURCES = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/pdfa/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/PdfSignerUnitTest/";

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private IX509Certificate[] chain;

        private IPrivateKey pk;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void Init() {
            pk = PemFileHelper.ReadFirstKey(CERTS_SRC + "signCertRsa01.pem", PASSWORD);
            chain = PemFileHelper.ReadFirstChain(CERTS_SRC + "signCertRsa01.pem");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void CreateNewSignatureFormFieldInvisibleAnnotationTest() {
            PdfSigner signer = new PdfSigner(new PdfReader(new MemoryStream(CreateEncryptedDocumentWithoutWidgetAnnotation
                ()), new ReaderProperties().SetPassword(OWNER)), new ByteArrayOutputStream(), new StampingProperties()
                );
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
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void CreateNewSignatureFormFieldNotInvisibleAnnotationTest() {
            PdfSigner signer = new PdfSigner(new PdfReader(new MemoryStream(CreateEncryptedDocumentWithoutWidgetAnnotation
                ()), new ReaderProperties().SetPassword(OWNER)), new ByteArrayOutputStream(), new StampingProperties()
                );
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
        public virtual void SignWithFieldLockNotNullTest() {
            PdfSigner signer = new PdfSigner(new PdfReader(new MemoryStream(CreateSimpleDocument(PdfVersion.PDF_2_0)))
                , new ByteArrayOutputStream(), new StampingProperties());
            signer.cryptoDictionary = new PdfSignature();
            signer.appearance.SetPageRect(new Rectangle(100, 100, 10, 10));
            PdfSigFieldLock fieldLock = new PdfSigFieldLock();
            signer.fieldLock = fieldLock;
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            NUnit.Framework.Assert.IsTrue(signer.closed);
        }

        [NUnit.Framework.Test]
        public virtual void SignDetachedWhenAlreadySignedIsNotPossibleTest() {
            PdfSigner signer = new PdfSigner(new PdfReader(new MemoryStream(CreateSimpleDocument())), new ByteArrayOutputStream
                (), new StampingProperties());
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => signer.SignDetached(pks, chain, null
                , null, null, 0, PdfSigner.CryptoStandard.CADES));
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.THIS_INSTANCE_OF_PDF_SIGNER_ALREADY_CLOSED, e
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void SignExternalWhenAlreadySignedIsNotPossibleTest() {
            PdfSigner signer = new PdfSigner(new PdfReader(new MemoryStream(CreateSimpleDocument())), new ByteArrayOutputStream
                (), new StampingProperties());
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => signer.SignExternalContainer(new ExternalBlankSignatureContainer
                (new PdfDictionary()), 0));
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.THIS_INSTANCE_OF_PDF_SIGNER_ALREADY_CLOSED, e
                .Message);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void PopulateExistingSignatureFormFieldInvisibleAnnotationTest() {
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            PdfDocument document = new PdfDocument(new PdfWriter(outputStream, new WriterProperties().SetStandardEncryption
                (USER, OWNER, 0, EncryptionConstants.STANDARD_ENCRYPTION_128)));
            document.AddNewPage();
            PdfWidgetAnnotation widgetAnnotation = new PdfWidgetAnnotation(new Rectangle(100, 100, 0, 0));
            document.GetPage(1).AddAnnotation(widgetAnnotation);
            document.Close();
            PdfSigner signer = new PdfSigner(new PdfReader(new MemoryStream(outputStream.ToArray()), new ReaderProperties
                ().SetPassword(OWNER)), new ByteArrayOutputStream(), new StampingProperties());
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
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void PopulateExistingSignatureFormFieldNotInvisibleAnnotationTest() {
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            PdfDocument document = new PdfDocument(new PdfWriter(outputStream, new WriterProperties().SetStandardEncryption
                (USER, OWNER, 0, EncryptionConstants.STANDARD_ENCRYPTION_128)));
            document.AddNewPage();
            PdfWidgetAnnotation widgetAnnotation = new PdfWidgetAnnotation(new Rectangle(100, 100, 0, 0));
            document.GetPage(1).AddAnnotation(widgetAnnotation);
            document.Close();
            PdfSigner signer = new PdfSigner(new PdfReader(new MemoryStream(outputStream.ToArray()), new ReaderProperties
                ().SetPassword(OWNER)), new ByteArrayOutputStream(), new StampingProperties());
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

        [NUnit.Framework.Test]
        public virtual void TempFileProvidedTest() {
            String tempFileName = "tempFile";
            PdfSigner signer = new PdfSigner(new PdfReader(new MemoryStream(CreateSimpleDocument())), new ByteArrayOutputStream
                (), DESTINATION_FOLDER + tempFileName, new StampingProperties());
            NUnit.Framework.Assert.IsNotNull(signer.tempFile);
            NUnit.Framework.Assert.AreEqual(tempFileName, signer.tempFile.Name);
            NUnit.Framework.Assert.IsNull(signer.temporaryOS);
        }

        [NUnit.Framework.Test]
        public virtual void InitPdfaDocumentTest() {
            PdfSigner signer = new PdfSigner(new PdfReader(new MemoryStream(CreateSimplePdfaDocument())), new ByteArrayOutputStream
                (), new StampingProperties());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_1A, ((PdfAAgnosticPdfDocument)signer.GetDocument
                ()).GetConformanceLevel());
        }

        [NUnit.Framework.Test]
        public virtual void SigningDateSetGetTest() {
            PdfSigner signer = new PdfSigner(new PdfReader(new MemoryStream(CreateSimplePdfaDocument())), new ByteArrayOutputStream
                (), new StampingProperties());
            DateTime testDate = DateTimeUtil.GetCurrentTime();
            signer.SetSignDate(testDate);
            NUnit.Framework.Assert.AreEqual(testDate, signer.GetSignDate());
        }

        [NUnit.Framework.Test]
        public virtual void CertificationLevelSetGetTest() {
            PdfSigner signer = new PdfSigner(new PdfReader(new MemoryStream(CreateSimplePdfaDocument())), new ByteArrayOutputStream
                (), new StampingProperties());
            NUnit.Framework.Assert.AreEqual(PdfSigner.NOT_CERTIFIED, signer.GetCertificationLevel());
            int testLevel = PdfSigner.CERTIFIED_NO_CHANGES_ALLOWED;
            signer.SetCertificationLevel(testLevel);
            NUnit.Framework.Assert.AreEqual(testLevel, signer.GetCertificationLevel());
        }

        [NUnit.Framework.Test]
        public virtual void SignatureDictionarySetGetTest() {
            PdfSigner signer = new PdfSigner(new PdfReader(new MemoryStream(CreateSimplePdfaDocument())), new ByteArrayOutputStream
                (), new StampingProperties());
            NUnit.Framework.Assert.IsNull(signer.GetSignatureDictionary());
            PdfSignature testSignature = new PdfSignature();
            signer.cryptoDictionary = testSignature;
            NUnit.Framework.Assert.AreEqual(testSignature, signer.GetSignatureDictionary());
        }

        [NUnit.Framework.Test]
        public virtual void SignatureEventSetGetTest() {
            PdfSigner signer = new PdfSigner(new PdfReader(new MemoryStream(CreateSimplePdfaDocument())), new ByteArrayOutputStream
                (), new StampingProperties());
            NUnit.Framework.Assert.IsNull(signer.GetSignatureEvent());
            PdfSigner.ISignatureEvent testEvent = new PdfSignerUnitTest.DummySignatureEvent(this);
            signer.SetSignatureEvent(testEvent);
            NUnit.Framework.Assert.AreEqual(testEvent, signer.GetSignatureEvent());
        }

        [NUnit.Framework.Test]
        public virtual void SignatureFieldNameMustNotContainDotTest() {
            PdfSigner signer = new PdfSigner(new PdfReader(new MemoryStream(CreateSimplePdfaDocument())), new ByteArrayOutputStream
                (), new StampingProperties());
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => signer.SetFieldName("name.with.dots"
                ));
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.FIELD_NAMES_CANNOT_CONTAIN_A_DOT, exception.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void DocumentWithoutReaderCannotBeSetToSignerTest() {
            PdfReader reader = new PdfReader(new MemoryStream(CreateSimplePdfaDocument()));
            PdfSigner signer = new PdfSigner(reader, new ByteArrayOutputStream(), new StampingProperties());
            PdfDocument documentWithoutReader = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => signer.SetDocument(documentWithoutReader
                ));
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.DOCUMENT_MUST_HAVE_READER, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DocumentSetGetTest() {
            PdfReader reader = new PdfReader(new MemoryStream(CreateSimplePdfaDocument()));
            PdfSigner signer = new PdfSigner(reader, new ByteArrayOutputStream(), new StampingProperties());
            PdfDocument document = signer.GetDocument();
            NUnit.Framework.Assert.AreEqual(reader, document.GetReader());
            PdfDocument documentWithoutReader = new PdfDocument(new PdfReader(new MemoryStream(CreateSimpleDocument())
                ), new PdfWriter(new ByteArrayOutputStream()));
            signer.SetDocument(documentWithoutReader);
            NUnit.Framework.Assert.AreEqual(documentWithoutReader, signer.GetDocument());
        }

        [NUnit.Framework.Test]
        public virtual void OutputStreamSetGetTest() {
            PdfReader reader = new PdfReader(new MemoryStream(CreateSimplePdfaDocument()));
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            PdfSigner signer = new PdfSigner(reader, outputStream, new StampingProperties());
            NUnit.Framework.Assert.AreEqual(outputStream, signer.originalOS);
            ByteArrayOutputStream anotherStream = new ByteArrayOutputStream();
            signer.SetOriginalOutputStream(anotherStream);
            NUnit.Framework.Assert.AreEqual(anotherStream, signer.originalOS);
        }

        [NUnit.Framework.Test]
        public virtual void FieldLockSetGetTest() {
            PdfReader reader = new PdfReader(new MemoryStream(CreateSimplePdfaDocument()));
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            PdfSigner signer = new PdfSigner(reader, outputStream, new StampingProperties());
            NUnit.Framework.Assert.IsNull(signer.GetFieldLockDict());
            PdfSigFieldLock fieldLock = new PdfSigFieldLock();
            signer.SetFieldLockDict(fieldLock);
            NUnit.Framework.Assert.AreEqual(fieldLock, signer.GetFieldLockDict());
        }

        [NUnit.Framework.Test]
        public virtual void SetFieldNameNullForDefaultSignerTest() {
            PdfReader reader = new PdfReader(new MemoryStream(CreateSimpleDocument()));
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            PdfSigner signer = new PdfSigner(reader, outputStream, new StampingProperties());
            signer.SetFieldName(null);
            NUnit.Framework.Assert.AreEqual("Signature1", signer.GetFieldName());
        }

        [NUnit.Framework.Test]
        public virtual void KeepFieldNameAfterSetToNullTest() {
            PdfReader reader = new PdfReader(new MemoryStream(CreateSimpleDocument()));
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            PdfSigner signer = new PdfSigner(reader, outputStream, new StampingProperties());
            String testName = "test_name";
            signer.SetFieldName(testName);
            signer.SetFieldName(null);
            NUnit.Framework.Assert.AreEqual(testName, signer.GetFieldName());
        }

        [NUnit.Framework.Test]
        public virtual void SetFieldNameToFieldWithSameNameAndNoSigTest() {
            PdfReader reader = new PdfReader(new MemoryStream(CreateDocumentWithEmptyField()));
            PdfSigner signer = new PdfSigner(reader, new ByteArrayOutputStream(), new StampingProperties());
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => signer.SetFieldName("test_field"
                ));
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.FIELD_TYPE_IS_NOT_A_SIGNATURE_FIELD_TYPE, e.Message
                );
            reader.Close();
        }

        [NUnit.Framework.Test]
        public virtual void SetFieldNameToSigFieldWithValueTest() {
            String fieldName = "test_field";
            String fieldValue = "test_value";
            PdfReader reader = new PdfReader(new MemoryStream(CreateDocumentWithSignatureWithTestValueField(fieldName, 
                fieldValue)));
            PdfSigner signer = new PdfSigner(reader, new ByteArrayOutputStream(), new StampingProperties());
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => signer.SetFieldName(fieldName)
                );
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.FIELD_ALREADY_SIGNED, e.Message);
            reader.Close();
        }

        [NUnit.Framework.Test]
        public virtual void SetFieldNameToSigFieldWithoutWidgetsTest() {
            String fieldName = "test_field";
            PdfReader reader = new PdfReader(new MemoryStream(CreateDocumentWithSignatureField(fieldName)));
            PdfSigner signer = new PdfSigner(reader, new ByteArrayOutputStream(), new StampingProperties());
            signer.SetFieldName(fieldName);
            NUnit.Framework.Assert.AreEqual(fieldName, signer.GetFieldName());
            reader.Close();
        }

        private static byte[] CreateDocumentWithEmptyField() {
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outputStream));
            PdfFormField formField = PdfFormField.CreateEmptyField(pdfDocument).SetFieldName("test_field");
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDocument, true);
            acroForm.AddField(formField);
            pdfDocument.Close();
            return outputStream.ToArray();
        }

        private static byte[] CreateDocumentWithSignatureWithTestValueField(String fieldName, String fieldValue) {
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outputStream));
            PdfFormField formField = PdfFormField.CreateSignature(pdfDocument).SetFieldName(fieldName).SetValue(fieldValue
                );
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDocument, true);
            acroForm.AddField(formField);
            pdfDocument.Close();
            return outputStream.ToArray();
        }

        private static byte[] CreateDocumentWithSignatureField(String fieldName) {
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outputStream));
            PdfFormField formField = PdfFormField.CreateSignature(pdfDocument).SetFieldName(fieldName);
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDocument, true);
            acroForm.AddField(formField);
            pdfDocument.Close();
            return outputStream.ToArray();
        }

        private static byte[] CreateEncryptedDocumentWithoutWidgetAnnotation() {
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            PdfDocument document = new PdfDocument(new PdfWriter(outputStream, new WriterProperties().SetStandardEncryption
                (USER, OWNER, 0, EncryptionConstants.STANDARD_ENCRYPTION_128)));
            document.AddNewPage();
            document.Close();
            return outputStream.ToArray();
        }

        private static byte[] CreateSimpleDocument() {
            return CreateSimpleDocument(PdfVersion.PDF_1_7);
        }

        private static byte[] CreateSimpleDocument(PdfVersion version) {
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            WriterProperties writerProperties = new WriterProperties();
            if (null != version) {
                writerProperties.SetPdfVersion(version);
            }
            PdfDocument document = new PdfDocument(new PdfWriter(outputStream, writerProperties));
            document.AddNewPage();
            document.Close();
            return outputStream.ToArray();
        }

        private static byte[] CreateSimplePdfaDocument() {
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            PdfWriter writer = new PdfWriter(outputStream);
            Stream @is = new FileStream(PDFA_RESOURCES + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read
                );
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfDocument document = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1A, outputIntent);
            document.SetTagged();
            document.GetCatalog().SetLang(new PdfString("en-US"));
            document.AddNewPage();
            document.Close();
            return outputStream.ToArray();
        }

        internal class ExtendedPdfSignatureFormField : PdfSignatureFormField {
            public ExtendedPdfSignatureFormField(PdfWidgetAnnotation widgetAnnotation, PdfDocument document)
                : base(widgetAnnotation, document) {
            }
        }

        internal class DummySignatureEvent : PdfSigner.ISignatureEvent {
            public virtual void GetSignatureDictionary(PdfSignature sig) {
            }

            internal DummySignatureEvent(PdfSignerUnitTest _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly PdfSignerUnitTest _enclosing;
            // Do nothing
        }
    }
}
