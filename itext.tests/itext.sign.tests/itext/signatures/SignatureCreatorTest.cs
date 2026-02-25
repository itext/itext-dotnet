/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Collections.Generic;
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Forms.Form.Element;
using iText.IO.Util;
using iText.Kernel.Crypto;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures {
    [NUnit.Framework.Category("IntegrationTest")]
    public class SignatureCreatorTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/SignatureCreatorTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/signatures/SignatureCreatorTest/";

        private static readonly String SOURCE_FILE = SOURCE_FOLDER + "helloWorldDoc.pdf";

        private const String SIGNATURE_FIELD = "Signature";

        private static readonly char[] KEY_PASSPHRASE = "testpassphrase".ToCharArray();

        private static readonly String CERT_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Setup() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        private static IEnumerable<Object[]> CreateSignatureCreatorParameters() {
            return JavaUtil.ArraysAsList(new Object[] { null }, new Object[] { "Custom Signature creator" });
        }

        [NUnit.Framework.TestCaseSource("CreateSignatureCreatorParameters")]
        public virtual void SignVerifySignatureCreatorTest(String signatureCreator) {
            String outFile = System.IO.Path.Combine(DESTINATION_FOLDER, "signVerify" + (signatureCreator == null ? "NoSignatureCreator"
                 : "CustomSignatureCreator") + ".pdf").ToString();
            String certPath = CERT_FOLDER + "signCertRsaWithChain.pem";
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outFile));
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(certPath);
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(certPath, KEY_PASSPHRASE);
            IExternalSignature pks = new PrivateKeySignature(signPrivateKey, DigestAlgorithms.SHA256);
            using (Stream @out = FileUtil.GetFileOutputStream(outFile)) {
                PdfSigner signer = new PdfSigner(new PdfReader(SOURCE_FILE), @out, new StampingProperties());
                SignerProperties signerProperties = GetSignerProperties("Approval test signature.\nCreated by iText.", signatureCreator
                    );
                signer.SetSignerProperties(signerProperties);
                signer.SignDetached(new BouncyCastleDigest(), pks, signChain, null, null, null, 0, PdfSigner.CryptoStandard
                    .CMS);
            }
            AssertSignatureCreator(GetSignatureCreator(new PdfDocument(new PdfReader(outFile)), SIGNATURE_FIELD), signatureCreator
                );
        }

        [NUnit.Framework.TestCaseSource("CreateSignatureCreatorParameters")]
        public virtual void SignExternalContainerSignatureCreatorTest(String signatureCreator) {
            String certPath = CERT_FOLDER + "signCertRsaWithChain.pem";
            String outFile = System.IO.Path.Combine(DESTINATION_FOLDER, "signExternalContainer" + (signatureCreator ==
                 null ? "NoSignatureCreator" : "CustomSignatureCreator") + ".pdf").ToString();
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outFile));
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(certPath);
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(certPath, KEY_PASSPHRASE);
            using (Stream @out = FileUtil.GetFileOutputStream(outFile)) {
                PdfSigner signer = new PdfSigner(new PdfReader(SOURCE_FILE), @out, new StampingProperties());
                SignerProperties signerProperties = GetSignerProperties("Sign external container.\nCreated by iText.", signatureCreator
                    );
                signer.SetSignerProperties(signerProperties);
                IExternalSignatureContainer extSigContainer = new PKCS7ExternalSignatureContainer(signPrivateKey, signChain
                    , DigestAlgorithms.SHA256);
                signer.SignExternalContainer(extSigContainer, 5000);
            }
            AssertSignatureCreator(GetSignatureCreator(new PdfDocument(new PdfReader(outFile)), SIGNATURE_FIELD), signatureCreator
                );
        }

        [NUnit.Framework.TestCaseSource("CreateSignatureCreatorParameters")]
        public virtual void SignDeferredSignatureCreatorTest(String signatureCreator) {
            String certPath = CERT_FOLDER + "signCertRsaWithChain.pem";
            String preparedFile = System.IO.Path.Combine(DESTINATION_FOLDER, "preparedDoc" + (signatureCreator == null
                 ? "NoSignatureCreator" : "CustomSignatureCreator") + ".pdf").ToString();
            String outFile = System.IO.Path.Combine(DESTINATION_FOLDER, "signDeferred" + (signatureCreator == null ? "NoSignatureCreator"
                 : "CustomSignatureCreator") + ".pdf").ToString();
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outFile));
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(certPath);
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(certPath, KEY_PASSPHRASE);
            PdfSigner signer = new PdfSigner(new PdfReader(SOURCE_FILE), FileUtil.GetFileOutputStream(preparedFile), new 
                StampingProperties());
            SignerProperties signerProperties = GetSignerProperties("Signature field which signing is deferred.", signatureCreator
                );
            signer.SetSignerProperties(signerProperties);
            IExternalSignatureContainer external = new ExternalBlankSignatureContainer(PdfName.Adobe_PPKLite, PdfName.
                Adbe_pkcs7_detached);
            signer.SignExternalContainer(external, 5000);
            IExternalSignatureContainer extSigContainer = new PKCS7ExternalSignatureContainer(signPrivateKey, signChain
                , DigestAlgorithms.SHA256);
            using (PdfReader reader = new PdfReader(preparedFile)) {
                using (Stream outStream = FileUtil.GetFileOutputStream(outFile)) {
                    PdfSigner.SignDeferred(reader, SIGNATURE_FIELD, outStream, extSigContainer);
                }
            }
            AssertSignatureCreator(GetSignatureCreator(new PdfDocument(new PdfReader(outFile)), SIGNATURE_FIELD), signatureCreator
                );
        }

        [NUnit.Framework.TestCaseSource("CreateSignatureCreatorParameters")]
        public virtual void TwoPhaseSigningSignatureCreatorTest(String signatureCreator) {
            String certPath = CERT_FOLDER + "signCertRsaWithChain.pem";
            String outFile = System.IO.Path.Combine(DESTINATION_FOLDER, "twoPhaseSigning" + (signatureCreator == null ? 
                "NoSignatureCreator" : "CustomSignatureCreator") + ".pdf").ToString();
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outFile));
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(certPath);
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(certPath, KEY_PASSPHRASE);
            using (PdfReader reader = new PdfReader(FileUtil.GetInputStreamForFile(SOURCE_FILE))) {
                using (MemoryStream outputStream = new MemoryStream()) {
                    PdfTwoPhaseSigner signer = new PdfTwoPhaseSigner(reader, outputStream);
                    SignerProperties signerProperties = GetSignerProperties("Two-phase signing.\nCreated by iText.", signatureCreator
                        );
                    byte[] digest = signer.PrepareDocumentForSignature(signerProperties, DigestAlgorithms.SHA256, PdfName.Adobe_PPKLite
                        , PdfName.Adbe_pkcs7_detached, 5000, false);
                    PdfPKCS7 sgn = new PdfPKCS7((IPrivateKey)null, signChain, DigestAlgorithms.SHA256, new BouncyCastleDigest(
                        ), false);
                    byte[] sh = sgn.GetAuthenticatedAttributeBytes(digest, PdfSigner.CryptoStandard.CMS, null, null);
                    PrivateKeySignature pkSign = new PrivateKeySignature(signPrivateKey, DigestAlgorithms.SHA256);
                    byte[] signData = pkSign.Sign(sh);
                    sgn.SetExternalSignatureValue(signData, null, pkSign.GetSignatureAlgorithmName(), pkSign.GetSignatureMechanismParameters
                        ());
                    byte[] data = sgn.GetEncodedPKCS7(digest, PdfSigner.CryptoStandard.CMS, null, null, null);
                    using (Stream outputStreamPhase2 = FileUtil.GetFileOutputStream(outFile)) {
                        using (PdfReader newReader = new PdfReader(new MemoryStream(outputStream.ToArray()))) {
                            PdfTwoPhaseSigner.AddSignatureToPreparedDocument(newReader, SIGNATURE_FIELD, outputStreamPhase2, data);
                        }
                    }
                }
            }
            AssertSignatureCreator(GetSignatureCreator(new PdfDocument(new PdfReader(outFile)), SIGNATURE_FIELD), signatureCreator
                );
        }

        [NUnit.Framework.TestCaseSource("CreateSignatureCreatorParameters")]
        public virtual void TimestampSignatureCreatorTest(String signatureCreator) {
            String certPath = CERT_FOLDER + "tsCertRsa.pem";
            String outFile = System.IO.Path.Combine(DESTINATION_FOLDER, "timestamp" + (signatureCreator == null ? "NoSignatureCreator"
                 : "CustomSignatureCreator") + ".pdf").ToString();
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outFile));
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(certPath);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(certPath, KEY_PASSPHRASE);
            using (Stream @out = FileUtil.GetFileOutputStream(outFile)) {
                PdfSigner signer = new PdfSigner(new PdfReader(SOURCE_FILE), @out, new StampingProperties());
                SignerProperties signerProperties = GetSignerProperties("Timestamp signature.\nCreated by iText.", signatureCreator
                    );
                signer.SetSignerProperties(signerProperties);
                ITSAClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
                signer.Timestamp(testTsa, SIGNATURE_FIELD);
            }
            AssertSignatureCreator(GetSignatureCreator(new PdfDocument(new PdfReader(outFile)), SIGNATURE_FIELD), signatureCreator
                );
        }

        [NUnit.Framework.TestCaseSource("CreateSignatureCreatorParameters")]
        public virtual void PadesLTASignatureLevelSignatureCreatorTest(String signatureCreator) {
            String signCertFileName = CERT_FOLDER + "signCertRsa01.pem";
            String tsaCertFileName = CERT_FOLDER + "tsCertRsa.pem";
            String caCertFileName = CERT_FOLDER + "rootRsa.pem";
            String outFile = System.IO.Path.Combine(DESTINATION_FOLDER, "padesLTASignatureLevel" + (signatureCreator ==
                 null ? "NoSignatureCreator" : "CustomSignatureCreator") + ".pdf").ToString();
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outFile));
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, KEY_PASSPHRASE);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, KEY_PASSPHRASE);
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, KEY_PASSPHRASE);
            SignerProperties signerProperties = GetSignerProperties("Sign with baseline-LTA profile.\nCreated by iText."
                , signatureCreator);
            PdfPadesSigner padesSigner = new PdfPadesSigner(new PdfReader(FileUtil.GetInputStreamForFile(SOURCE_FILE))
                , FileUtil.GetFileOutputStream(outFile));
            padesSigner.SetEstimatedSize(0);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            ICrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey, DigestAlgorithms.
                SHA256);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey, "SHA256withRSA"
                ).AddBuilderForCertIssuer((IX509Certificate)tsaChain[0], tsaPrivateKey, DigestAlgorithms.SHA256);
            padesSigner.SetOcspClient(ocspClient).SetCrlClient(crlClient).SetTimestampSignatureName("timestampSig1");
            padesSigner.SignWithBaselineLTAProfile(signerProperties, signChain, signPrivateKey, testTsa);
            PdfDocument outDoc = new PdfDocument(new PdfReader(outFile));
            NUnit.Framework.Assert.AreEqual("", GetSignatureCreator(outDoc, "timestampSig1"));
            AssertSignatureCreator(GetSignatureCreator(outDoc, SIGNATURE_FIELD), signatureCreator);
        }

        private static String GetSignatureCreator(PdfDocument document, String signatureName) {
            SignatureUtil signatureUtil = new SignatureUtil(document);
            PdfSignature signature = signatureUtil.GetSignature(signatureName);
            return signature.GetPdfObject().GetAsDictionary(PdfName.Prop_Build).GetAsDictionary(PdfName.App).GetAsName
                (PdfName.Name).GetValue();
        }

        private static void AssertSignatureCreator(String actualSignatureCreator, String expectedSignatureCreator) {
            if (expectedSignatureCreator == null) {
                MemoryStream outputStream = new MemoryStream();
                PdfDocument doc = new PdfDocument(new PdfWriter(outputStream));
                doc.AddNewPage();
                doc.Close();
                using (PdfDocument regularPdf = new PdfDocument(new PdfReader(new MemoryStream(outputStream.ToArray())))) {
                    NUnit.Framework.Assert.AreEqual(regularPdf.GetDocumentInfo().GetProducer(), actualSignatureCreator);
                }
            }
            else {
                NUnit.Framework.Assert.AreEqual(expectedSignatureCreator, actualSignatureCreator);
            }
        }

        private static SignerProperties GetSignerProperties(String description, String signatureCreator) {
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID).SetContent
                (description);
            SignerProperties signerProperties = new SignerProperties().SetFieldName(SIGNATURE_FIELD).SetPageRect(new Rectangle
                (50, 650, 200, 100)).SetReason("Test").SetLocation("TestCity").SetSignatureAppearance(appearance);
            if (signatureCreator != null) {
                signerProperties.SetSignatureCreator(signatureCreator);
            }
            return signerProperties;
        }
    }
}
