/*
This file is part of the iText (R) project.
    Copyright (c) 1998-2024 Apryse Group NV
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
using System.Security.Cryptography;
using System.Xml.Serialization;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Digest;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Signatures.Cms;
using iText.Signatures.Exceptions;
using iText.Signatures.Testutils;
using iText.Test;
using SignerInfo = iText.Signatures.Cms.SignerInfo;

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
        
        private static readonly String RSA_PRIVATE_KEY_FILE = CERTS_SRC + "signCertRsa01.xml"; 

        private const String DIGEST_ALGORITHM = DigestAlgorithms.SHA384;

        private const string DIGEST_ALGORITHM_OID = "2.16.840.1.101.3.4.2.2";

        private const String FIELD_NAME = "Signature1";

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
        public virtual void TestPreparationWithClosedPdfTwoPhaseSigner() {
            // prepare the file
            using (PdfReader reader = new PdfReader(FileUtil.GetInputStreamForFile(SIMPLE_DOC_PATH))) {
                using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                    PdfTwoPhaseSigner signer = new PdfTwoPhaseSigner(reader, outputStream);
                    signer.PrepareDocumentForSignature(new SignerProperties(), DigestAlgorithms.SHA384, PdfName.Adobe_PPKLite, PdfName.Adbe_pkcs7_detached
                        , 5000, false);
                    Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () =>
                        {
                            SignerProperties signerProperties = new SignerProperties();
                            byte[] digest = signer.PrepareDocumentForSignature(signerProperties, DigestAlgorithms.SHA384, PdfName.Adobe_PPKLite, PdfName
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
            using (PdfReader reader = new PdfReader(new FileInfo(SOURCE_FOLDER + "2PhasePreparedSignature.pdf"
                   ))) {
                using (Stream signedDoc = new ByteArrayOutputStream()) {
                    // add signature
                    Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                            PdfTwoPhaseSigner.AddSignatureToPreparedDocument(reader, "wrong" + FIELD_NAME, signedDoc, signData);
                        }
                    );
                    NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(SignExceptionMessageConstant.THERE_IS_NO_FIELD_IN_THE_DOCUMENT_WITH_SUCH_NAME
                        , "wrong" + FIELD_NAME), e.Message);
                }
            }
        }
        
        [NUnit.Framework.Test]
        public virtual void TestCompletionWithWrongFieldNameAndDeprecatedApiTest() {
            byte[] signData = new byte[4096];
            // open prepared document
            using (PdfDocument preparedDoc = new PdfDocument(new PdfReader(new FileInfo(SOURCE_FOLDER + "2PhasePreparedSignature.pdf"
                   )))) {
                using (Stream signedDoc = new ByteArrayOutputStream()) {
                    // add signature
                    Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                            PdfTwoPhaseSigner.AddSignatureToPreparedDocument(preparedDoc, "wrong" + FIELD_NAME, signedDoc, signData);
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
            using (PdfReader reader = new PdfReader(new FileInfo(SOURCE_FOLDER + "2PhasePreparedSignature.pdf"
                ))) {
                using (Stream signedDoc = new ByteArrayOutputStream()) {
                    // add signature
                    Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                        PdfTwoPhaseSigner.AddSignatureToPreparedDocument(reader, FIELD_NAME, signedDoc, signData);
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
                    PdfTwoPhaseSigner signer = new PdfTwoPhaseSigner(reader, outputStream);
                    // Add second signature field
                    SignerProperties signerProperties = new SignerProperties();
                    byte[] digest = signer.PrepareDocumentForSignature(signerProperties,DIGEST_ALGORITHM, PdfName.Adobe_PPKLite, PdfName.Adbe_pkcs7_detached
                        , 5000, false);
                    byte[] signData = new byte[1024];
                    using (Stream outputStreamPhase2 = FileUtil.GetFileOutputStream(DESTINATION_FOLDER + "2PhaseCompleteCycle.pdf"
                        )) {
                        using (PdfReader newReader = new PdfReader(new MemoryStream(outputStream.ToArray()))) {
                            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                                PdfTwoPhaseSigner.AddSignatureToPreparedDocument(newReader, FIELD_NAME, outputStreamPhase2, signData);
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
                    PdfTwoPhaseSigner signer = new PdfTwoPhaseSigner(reader, outputStream);
                    SignerProperties signerProperties = new SignerProperties();
                    byte[] digest = signer.PrepareDocumentForSignature(signerProperties,DigestAlgorithms.SHA384, PdfName.Adobe_PPKLite, PdfName
                        .Adbe_pkcs7_detached, 5000, false);
                    String fieldName = signerProperties.GetFieldName();
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
                    PdfTwoPhaseSigner signer = new PdfTwoPhaseSigner(reader, outputStream);
                    SignerProperties signerProperties = new SignerProperties();
                    byte[] digest = signer.PrepareDocumentForSignature(signerProperties,DIGEST_ALGORITHM, PdfName.Adobe_PPKLite, PdfName.Adbe_pkcs7_detached
                        , 5000, false);
                    String fieldName = signerProperties.GetFieldName();
                    // Phase 2 sign the document digest
                    byte[] signData = SignDigest(digest, DIGEST_ALGORITHM);
                    using (Stream outputStreamPhase2 = FileUtil.GetFileOutputStream(DESTINATION_FOLDER + "2PhaseCompleteCycle.pdf"
                        )) {
                        using (PdfReader newReader = new PdfReader(new MemoryStream(outputStream.ToArray()))) {
                            PdfTwoPhaseSigner.AddSignatureToPreparedDocument(newReader, fieldName, outputStreamPhase2, signData);
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
            using (PdfReader reader = new PdfReader(new FileInfo(SOURCE_FOLDER + "2PhasePreparedSignature.pdf"
                ))) {
                using (Stream signedDoc = FileUtil.GetFileOutputStream(DESTINATION_FOLDER + "2PhaseCompletion.pdf")) {
                    // add signature
                    PdfTwoPhaseSigner.AddSignatureToPreparedDocument(reader, FIELD_NAME, signedDoc, signData);
                }
            }
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(DESTINATION_FOLDER + "2PhaseCompletion.pdf"
                , SOURCE_FOLDER + "cmp_2PhaseCompleteCycle.pdf"));
        }

        [NUnit.Framework.Test]
        public virtual void TestWithCMS() {
            String signatureName = "Signature1";
            using (ByteArrayOutputStream phaseOneOS = new ByteArrayOutputStream()) {
                // Phase 1 prepare the document, add the partial CMS  and get the documents digest of signed attributes
                byte[] dataToEncrypt = PrepareDocumentAndCMS(new FileInfo(SIMPLE_DOC_PATH), phaseOneOS, signatureName);
                // Phase 2 sign the document digest
                //simulating server side
                byte[] signaturedata = ServerSideSigning(dataToEncrypt);

                String signedDocumentName = DESTINATION_FOLDER + "2PhaseCompleteCycleCMS.pdf";
                // phase 2.1 extract CMS from the prepared document
                using (Stream outputStreamPhase2 = FileUtil.GetFileOutputStream(signedDocumentName)) {
                    using (PdfDocument doc = new PdfDocument(new PdfReader(new MemoryStream(phaseOneOS.ToArray())))) {
                        SignatureUtil su = new SignatureUtil(doc);
                        PdfSignature sig = su.GetSignature(signatureName);
                        PdfString encodedCMS = sig.GetContents();
                        byte[] encodedCMSdata = encodedCMS.GetValueBytes();
                        CMSContainer cmsToUpdate = new CMSContainer(encodedCMSdata);
                        //phase 2.2 add the signatureValue to the CMS
                        cmsToUpdate.GetSignerInfo().SetSignature(signaturedata);
                        //if needed a time stamp could be added here
                        //Phase 2.3 add the updated CMS to the document
                        PdfTwoPhaseSigner.AddSignatureToPreparedDocument(new PdfReader(new MemoryStream(phaseOneOS.ToArray())), signatureName, outputStreamPhase2, cmsToUpdate);
                    }
                }
                // validate signature
                using (PdfReader reader = new PdfReader(signedDocumentName)) {
                    using (PdfDocument finalDoc = new PdfDocument(reader)) {
                        SignatureUtil su = new SignatureUtil(finalDoc);
                        PdfPKCS7 cms = su.ReadSignatureData(signatureName);
                        NUnit.Framework.Assert.IsTrue(cms.VerifySignatureIntegrityAndAuthenticity(), "Signature should be valid");
                    }
                }
                // compare result
                NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(signedDocumentName, SOURCE_FOLDER + 
                    "cmp_2PhaseCompleteCycleCMS.pdf"));
            }
        }

        private byte[] SignDigest(byte[] data, String hashAlgorithm) {
            PdfPKCS7 sgn = new PdfPKCS7((IPrivateKey)null, chain, hashAlgorithm, new BouncyCastleDigest(), false);
            byte[] sh = sgn.GetAuthenticatedAttributeBytes(data, PdfSigner.CryptoStandard.CMS, null, null);
            PrivateKeySignature pkSign = new PrivateKeySignature(pk, hashAlgorithm);
            byte[] signData = pkSign.Sign(sh);
            sgn.SetExternalSignatureValue(signData, null, pkSign.GetSignatureAlgorithmName(), pkSign.GetSignatureMechanismParameters
                ());
            return sgn.GetEncodedPKCS7(data, PdfSigner.CryptoStandard.CMS, null, null, null);
        }

        private byte[] PrepareDocumentAndCMS(FileInfo document, ByteArrayOutputStream preparedOS, String signatureName
            ) {
            using (PdfReader reader = new PdfReader(FileUtil.GetInputStreamForFile(document))) {
                using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                    PdfTwoPhaseSigner signer = new PdfTwoPhaseSigner(reader, outputStream);
                    SignerProperties signerProperties = new SignerProperties().SetFieldName(signatureName);
                    byte[] digest = signer.PrepareDocumentForSignature(signerProperties,DIGEST_ALGORITHM, PdfName.Adobe_PPKLite, PdfName.Adbe_pkcs7_detached
                        , 5000, false);
                    System.Console.Out.WriteLine("Document digest from prepare call: " + digest.Length + "bytes");
                    System.Console.Out.WriteLine(Convert.ToBase64String(digest));
                    String fieldName = signerProperties.GetFieldName();
                    // Phase 1.1 prepare the CMS
                    CMSContainer cms = new CMSContainer();
                    SignerInfo signerInfo = new SignerInfo();
                    //signerInfo.setSigningCertificateAndAddToSignedAttributes(chain[0], OID.ID_SHA384);
                    signerInfo.SetSigningCertificate(chain[0]);
                    // in the two phase scenario,; we don't have the private key! So we start from the signing certificate
                    
                    signerInfo.SetSignatureAlgorithm(new Cms.AlgorithmIdentifier(chain[0].GetSigAlgOID()));
                    signerInfo.SetDigestAlgorithm(new Cms.AlgorithmIdentifier(DIGEST_ALGORITHM_OID));
                    signerInfo.SetMessageDigest(digest);
                    cms.SetSignerInfo(signerInfo);
                    cms.AddCertificates(chain);
                    byte[] signedAttributesToSign = cms.GetSerializedSignedAttributes();

                    IMessageDigest sha = iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest(DIGEST_ALGORITHM
                        );
                    byte[] dataToSign = sha.Digest(signedAttributesToSign);

                    // now we store signedAttributesToSign together with the prepared document and send
                    // dataToSign to the signing instance
                    using (PdfReader newReader = new PdfReader(new MemoryStream(outputStream.ToArray()))) {
                        PdfTwoPhaseSigner.AddSignatureToPreparedDocument(newReader, fieldName, preparedOS, cms);
                    }
                    return dataToSign;
                }
            }
        }

        private byte[] ServerSideSigning(byte[] dataToEncrypt) {
            String signingAlgoritmName = pk.GetAlgorithm();
            if ("EC".Equals(signingAlgoritmName)) {
                signingAlgoritmName = "ECDSA";
            }

            using (var fs = new StreamReader(RSA_PRIVATE_KEY_FILE)) {
                RSA rsa = RSA.Create();
                XmlSerializer serializer =
                    new XmlSerializer(typeof(RSAParameters));
                var rsaParams = (RSAParameters)serializer.Deserialize(fs);
                rsa.ImportParameters(rsaParams);
                byte[] signaturedata = rsa.SignHash(dataToEncrypt, HashAlgorithmName.SHA384, RSASignaturePadding.Pkcs1);
                return signaturedata;
            }
        }
    }
}
