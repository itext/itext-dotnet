/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.IO.Source;
using iText.Kernel.Crypto;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Signatures {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class PdfTwoPhaseSignerUnitTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly byte[] OWNER = "owner".GetBytes(System.Text.Encoding.UTF8);

        private static readonly byte[] USER = "user".GetBytes(System.Text.Encoding.UTF8);

        private static readonly String PDFA_RESOURCES = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/pdfa/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/Pdf2PhaseSignerUnitTest/";

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void PrepareDocumentTestWithSHA256() {
            PdfReader reader = new PdfReader(new MemoryStream(CreateSimpleDocument()));
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            PdfTwoPhaseSigner signer = new PdfTwoPhaseSigner(reader, outputStream);
            int estimatedSize = 8079;
            SignerProperties signerProperties = new SignerProperties();
            byte[] digest = signer.PrepareDocumentForSignature(signerProperties, DigestAlgorithms.SHA256, PdfName.Adobe_PPKLite
                , PdfName.Adbe_pkcs7_detached, estimatedSize, false);
            String fieldName = signerProperties.GetFieldName();
            PdfReader resultReader = new PdfReader(new MemoryStream(outputStream.ToArray()));
            PdfDocument resultDoc = new PdfDocument(resultReader);
            SignatureUtil signatureUtil = new SignatureUtil(resultDoc);
            PdfSignature signature = signatureUtil.GetSignature(fieldName);
            NUnit.Framework.Assert.AreEqual(estimatedSize, signature.GetContents().GetValueBytes().Length);
        }

        [NUnit.Framework.Test]
        public virtual void PrepareDocumentTestWithExternalDigest() {
            PdfReader reader = new PdfReader(new MemoryStream(CreateSimpleDocument()));
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            PdfTwoPhaseSigner signer = new PdfTwoPhaseSigner(reader, outputStream);
            int estimatedSize = 8079;
            SignerProperties signerProperties = new SignerProperties();
            signer.SetExternalDigest(new BouncyCastleDigest());
            byte[] digest = signer.PrepareDocumentForSignature(signerProperties, DigestAlgorithms.SHA256, PdfName.Adobe_PPKLite
                , PdfName.Adbe_pkcs7_detached, estimatedSize, false);
            String fieldName = signerProperties.GetFieldName();
            PdfReader resultReader = new PdfReader(new MemoryStream(outputStream.ToArray()));
            PdfDocument resultDoc = new PdfDocument(resultReader);
            SignatureUtil signatureUtil = new SignatureUtil(resultDoc);
            PdfSignature signature = signatureUtil.GetSignature(fieldName);
            NUnit.Framework.Assert.AreEqual(estimatedSize, signature.GetContents().GetValueBytes().Length);
        }

        [NUnit.Framework.Test]
        public virtual void AddSignatureToPreparedDocumentTest() {
            PdfReader reader = new PdfReader(new MemoryStream(CreateSimpleDocument()));
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            PdfTwoPhaseSigner signer = new PdfTwoPhaseSigner(reader, outputStream);
            int estimatedSize = 8079;
            SignerProperties signerProperties = new SignerProperties();
            signer.PrepareDocumentForSignature(signerProperties, DigestAlgorithms.SHA256, PdfName.Adobe_PPKLite, PdfName
                .Adbe_pkcs7_detached, estimatedSize, false);
            String fieldName = signerProperties.GetFieldName();
            using (PdfReader resultReader = new PdfReader(new MemoryStream(outputStream.ToArray()))) {
                ByteArrayOutputStream completedOutputStream = new ByteArrayOutputStream();
                byte[] testData = ByteUtils.GetIsoBytes("Some data to test the signature addition with");
                PdfTwoPhaseSigner.AddSignatureToPreparedDocument(resultReader, fieldName, completedOutputStream, testData);
                using (PdfDocument resultDoc = new PdfDocument(new PdfReader(new MemoryStream(completedOutputStream.ToArray
                    ())))) {
                    SignatureUtil signatureUtil = new SignatureUtil(resultDoc);
                    PdfSignature signature = signatureUtil.GetSignature(fieldName);
                    byte[] content = signature.GetContents().GetValueBytes();
                    for (int i = 0; i < testData.Length; i++) {
                        NUnit.Framework.Assert.AreEqual(testData[i], content[i]);
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddSignatureToPreparedDocumentDeprecatedApiTest() {
            PdfReader reader = new PdfReader(new MemoryStream(CreateSimpleDocument()));
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            PdfTwoPhaseSigner signer = new PdfTwoPhaseSigner(reader, outputStream);
            int estimatedSize = 8079;
            SignerProperties signerProperties = new SignerProperties();
            signer.PrepareDocumentForSignature(signerProperties, DigestAlgorithms.SHA256, PdfName.Adobe_PPKLite, PdfName
                .Adbe_pkcs7_detached, estimatedSize, false);
            String fieldName = signerProperties.GetFieldName();
            using (PdfDocument document = new PdfDocument(new PdfReader(new MemoryStream(outputStream.ToArray())))) {
                ByteArrayOutputStream completedOutputStream = new ByteArrayOutputStream();
                byte[] testData = ByteUtils.GetIsoBytes("Some data to test the signature addition with");
                PdfTwoPhaseSigner.AddSignatureToPreparedDocument(document, fieldName, completedOutputStream, testData);
                using (PdfDocument resultDoc = new PdfDocument(new PdfReader(new MemoryStream(completedOutputStream.ToArray
                    ())))) {
                    SignatureUtil signatureUtil = new SignatureUtil(resultDoc);
                    PdfSignature signature = signatureUtil.GetSignature(fieldName);
                    byte[] content = signature.GetContents().GetValueBytes();
                    for (int i = 0; i < testData.Length; i++) {
                        NUnit.Framework.Assert.AreEqual(testData[i], content[i]);
                    }
                }
            }
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
    }
}
