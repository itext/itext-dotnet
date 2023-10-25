/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Exceptions;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class TwoPhaseSigningTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/TwoPhaseSigningTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/TwoPhaseSigningTest/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private static readonly String SIMPLE_DOC_PATH = SOURCE_FOLDER + "SimpleDoc.pdf";

        private const String DIGEST_ALGORITHM = DigestAlgorithms.SHA384;

        public const String FIELD_NAME = "Signature1";

        private IPrivateKey pk;

        private IX509Certificate[] chain;

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
        public virtual void TestPreparationWithClosedPdfSigner() {
            // prepare the file
            using (PdfReader reader = new PdfReader(FileUtil.GetInputStreamForFile(SIMPLE_DOC_PATH))) {
                using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                    PdfSigner signer = new PdfSigner(reader, outputStream, new StampingProperties());
                    signer.PrepareDocumentForSignature(DigestAlgorithms.SHA384, PdfName.Adobe_PPKLite, PdfName.Adbe_pkcs7_detached
                        , 5000, false);
                    Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                        byte[] digest = signer.PrepareDocumentForSignature(DigestAlgorithms.SHA384, PdfName.Adobe_PPKLite, PdfName
                            .Adbe_pkcs7_detached, 5000, false);
                    }
                    );
                    NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.THIS_INSTANCE_OF_PDF_SIGNER_ALREADY_CLOSED, e
                        .Message);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCompletionWithWrongFieldName() {
            byte[] signData = new byte[4096];
            // open prepared document
            using (PdfDocument preparedDoc = new PdfDocument(new PdfReader(new FileInfo(SOURCE_FOLDER + "2PhasePreparedSignature.pdf"
                )))) {
                using (Stream signedDoc = new ByteArrayOutputStream()) {
                    // add signature
                    Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                        PdfSigner.AddSignatureToPreparedDocument(preparedDoc, "wrong" + FIELD_NAME, signedDoc, signData);
                    }
                    );
                    NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(SignExceptionMessageConstant.THERE_IS_NO_FIELD_IN_THE_DOCUMENT_WITH_SUCH_NAME
                        , "wrong" + FIELD_NAME), e.Message);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCompletionWithNotEnoughSpace() {
            byte[] signData = new byte[20000];
            // open prepared document
            using (PdfDocument preparedDoc = new PdfDocument(new PdfReader(new FileInfo(SOURCE_FOLDER + "2PhasePreparedSignature.pdf"
                )))) {
                using (Stream signedDoc = new ByteArrayOutputStream()) {
                    // add signature
                    Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                        PdfSigner.AddSignatureToPreparedDocument(preparedDoc, FIELD_NAME, signedDoc, signData);
                    }
                    );
                    NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.AVAILABLE_SPACE_IS_NOT_ENOUGH_FOR_SIGNATURE, 
                        e.Message);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCompletionWithSignatureFieldNotLastOne() {
            using (PdfReader reader = new PdfReader(FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "2PhasePreparedSignature.pdf"
                ))) {
                using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                    PdfSigner signer = new PdfSigner(reader, outputStream, new StampingProperties());
                    // Add second signature field
                    byte[] digest = signer.PrepareDocumentForSignature(DIGEST_ALGORITHM, PdfName.Adobe_PPKLite, PdfName.Adbe_pkcs7_detached
                        , 5000, false);
                    byte[] signData = new byte[1024];
                    using (Stream outputStreamPhase2 = FileUtil.GetFileOutputStream(DESTINATION_FOLDER + "2PhaseCompleteCycle.pdf"
                        )) {
                        using (PdfDocument doc = new PdfDocument(new PdfReader(new MemoryStream(outputStream.ToArray())))) {
                            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                                PdfSigner.AddSignatureToPreparedDocument(doc, FIELD_NAME, outputStreamPhase2, signData);
                            }
                            );
                            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(SignExceptionMessageConstant.SIGNATURE_WITH_THIS_NAME_IS_NOT_THE_LAST_IT_DOES_NOT_COVER_WHOLE_DOCUMENT
                                , FIELD_NAME), e.Message);
                        }
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestPreparation() {
            // prepare the file
            using (PdfReader reader = new PdfReader(FileUtil.GetInputStreamForFile(SIMPLE_DOC_PATH))) {
                using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                    PdfSigner signer = new PdfSigner(reader, outputStream, new StampingProperties());
                    String fieldName = signer.GetFieldName();
                    byte[] digest = signer.PrepareDocumentForSignature(DigestAlgorithms.SHA384, PdfName.Adobe_PPKLite, PdfName
                        .Adbe_pkcs7_detached, 5000, false);
                    using (PdfDocument cmp_document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "cmp_prepared.pdf"))) {
                        using (PdfDocument outDocument = new PdfDocument(new PdfReader(new MemoryStream(outputStream.ToArray())))) {
                            SignatureUtil signatureUtil = new SignatureUtil(cmp_document);
                            PdfSignature cmpSignature = signatureUtil.GetSignature(fieldName);
                            signatureUtil = new SignatureUtil(outDocument);
                            PdfSignature outSignature = signatureUtil.GetSignature(fieldName);
                            try {
                                NUnit.Framework.Assert.IsTrue(signatureUtil.SignatureCoversWholeDocument(FIELD_NAME));
                                NUnit.Framework.Assert.AreEqual(cmpSignature.GetContents().GetValueBytes(), outSignature.GetContents().GetValueBytes
                                    ());
                            }
                            catch (Exception) {
                                Stream fs = FileUtil.GetFileOutputStream(DESTINATION_FOLDER + "testPreparation.pdf");
                                fs.Write(outputStream.ToArray());
                                fs.Dispose();
                            }
                        }
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCompleteCycle() {
            // Phase 1 prepare the document and get the documents digest and the fieldname of the created signature
            using (PdfReader reader = new PdfReader(FileUtil.GetInputStreamForFile(SIMPLE_DOC_PATH))) {
                using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                    PdfSigner signer = new PdfSigner(reader, outputStream, new StampingProperties());
                    byte[] digest = signer.PrepareDocumentForSignature(DIGEST_ALGORITHM, PdfName.Adobe_PPKLite, PdfName.Adbe_pkcs7_detached
                        , 5000, false);
                    String fieldName = signer.GetFieldName();
                    // Phase 2 sign the document digest
                    byte[] signData = SignDigest(digest, DIGEST_ALGORITHM);
                    using (Stream outputStreamPhase2 = FileUtil.GetFileOutputStream(DESTINATION_FOLDER + "2PhaseCompleteCycle.pdf"
                        )) {
                        using (PdfDocument doc = new PdfDocument(new PdfReader(new MemoryStream(outputStream.ToArray())))) {
                            PdfSigner.AddSignatureToPreparedDocument(doc, fieldName, outputStreamPhase2, signData);
                        }
                    }
                    NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(DESTINATION_FOLDER + "2PhaseCompleteCycle.pdf"
                        , SOURCE_FOLDER + "cmp_2PhaseCompleteCycle.pdf"));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCompletion() {
            // read data
            byte[] signData = new byte[4096];
            using (Stream signdataS = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "signeddata.bin")) {
                signdataS.Read(signData);
            }
            // open prepared document
            using (PdfDocument preparedDoc = new PdfDocument(new PdfReader(new FileInfo(SOURCE_FOLDER + "2PhasePreparedSignature.pdf"
                )))) {
                using (Stream signedDoc = FileUtil.GetFileOutputStream(DESTINATION_FOLDER + "2PhaseCompletion.pdf")) {
                    // add signature
                    PdfSigner.AddSignatureToPreparedDocument(preparedDoc, FIELD_NAME, signedDoc, signData);
                }
            }
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(DESTINATION_FOLDER + "2PhaseCompletion.pdf"
                , SOURCE_FOLDER + "cmp_2PhaseCompleteCycle.pdf"));
        }

        private byte[] SignDigest(byte[] data, String hashAlgorithm) {
            PdfPKCS7 sgn = new PdfPKCS7((IPrivateKey)null, chain, hashAlgorithm, false);
            byte[] sh = sgn.GetAuthenticatedAttributeBytes(data, PdfSigner.CryptoStandard.CMS, null, null);
            PrivateKeySignature pkSign = new PrivateKeySignature(pk, hashAlgorithm);
            byte[] signData = pkSign.Sign(sh);
            sgn.SetExternalSignatureValue(signData, null, pkSign.GetSignatureAlgorithmName(), pkSign.GetSignatureMechanismParameters
                ());
            return sgn.GetEncodedPKCS7(data, PdfSigner.CryptoStandard.CMS, null, null, null);
        }
    }
}
