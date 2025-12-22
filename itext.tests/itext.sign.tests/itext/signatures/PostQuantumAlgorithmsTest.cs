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
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Mocks;
using iText.Signatures.Validation.Report;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PostQuantumAlgorithmsTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private static readonly bool FIPS_MODE = "BCFIPS".Equals(BOUNCY_CASTLE_FACTORY.GetProviderName());

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/PostQuantumAlgorithmsTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/signatures/PostQuantumAlgorithmsTest/";

        private static readonly String SOURCE_FILE = SOURCE_FOLDER + "helloWorldDoc.pdf";

        private const String SIGNATURE_FIELD = "Signature";

        private static readonly char[] KEY_PASSPHRASE = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Setup() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IEnumerable<Object[]> Algorithms() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { "ML-DSA-44", DigestAlgorithms.SHA3_256, OID.ML_DSA_44
                , 2420 }, new Object[] { "ML-DSA-65", DigestAlgorithms.SHA3_384, OID.ML_DSA_65, 3309 }, new Object[] { 
                "ML-DSA-87", DigestAlgorithms.SHA3_512, OID.ML_DSA_87, 4627 }, new Object[] { "slh-dsa-sha2-128s", DigestAlgorithms
                .SHA256, OID.SLH_DSA_SHA2_128S, 7856 }, new Object[] { "slh-dsa-sha2-128f", DigestAlgorithms.SHA256, OID
                .SLH_DSA_SHA2_128F, 17088 }, new Object[] { "slh-dsa-shake-128s", DigestAlgorithms.SHAKE256, OID.SLH_DSA_SHAKE_128S
                , 7856 }, new Object[] { "slh-dsa-shake-128f", DigestAlgorithms.SHAKE256, OID.SLH_DSA_SHAKE_128F, 17088
                 }, new Object[] { "slh-dsa-sha2-192s", DigestAlgorithms.SHA512, OID.SLH_DSA_SHA2_192S, 16224 }, new Object
                [] { "slh-dsa-sha2-192f", DigestAlgorithms.SHA512, OID.SLH_DSA_SHA2_192F, 35664 }, new Object[] { "slh-dsa-shake-192s"
                , DigestAlgorithms.SHAKE256, OID.SLH_DSA_SHAKE_192S, 16224 }, new Object[] { "slh-dsa-shake-192f", DigestAlgorithms
                .SHAKE256, OID.SLH_DSA_SHAKE_192F, 35664 }, new Object[] { "slh-dsa-sha2-256s", DigestAlgorithms.SHA512
                , OID.SLH_DSA_SHA2_256S, 29792 }, new Object[] { "slh-dsa-sha2-256f", DigestAlgorithms.SHA512, OID.SLH_DSA_SHA2_256F
                , 49856 }, new Object[] { "slh-dsa-shake-256s", DigestAlgorithms.SHAKE256, OID.SLH_DSA_SHAKE_256S, 29792
                 }, new Object[] { "slh-dsa-shake-256f", DigestAlgorithms.SHAKE256, OID.SLH_DSA_SHAKE_256F, 49856 } });
        }

        [NUnit.Framework.TestCaseSource("Algorithms")]
        [LogMessage(KernelLogMessageConstant.ALGORITHM_NOT_FROM_SPEC, Ignore = true)]
        public virtual void SignVerifyPQCTest(String signatureAlgo, String digestAlgo, String expectedSigAlgoIdentifier
            , int signatureBytesSize) {
            String outFile = System.IO.Path.Combine(DESTINATION_FOLDER, signatureAlgo + ".pdf").ToString();
            String certPath = SOURCE_FOLDER + "cert_" + signatureAlgo + ".pem";
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outFile));
            if (FIPS_MODE) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(Exception), () => DoSign(signatureAlgo, digestAlgo, certPath
                    , outFile, signatureBytesSize));
                NUnit.Framework.Assert.IsTrue(e.Message.Contains(MessageFormatUtil.Format(KernelExceptionMessageConstant.NO_SUCH_ALGORITHM_FOR_PROVIDER_BCFIPS
                    , expectedSigAlgoIdentifier)) || KernelExceptionMessageConstant.NO_SUCH_ALGORITHM.Equals(e.Message));
            }
            else {
                DoSign(signatureAlgo, digestAlgo, certPath, outFile, signatureBytesSize);
                DoVerify(outFile, expectedSigAlgoIdentifier);
            }
        }

        [NUnit.Framework.TestCaseSource("Algorithms")]
        [LogMessage(KernelLogMessageConstant.ALGORITHM_NOT_FROM_SPEC, Ignore = true)]
        public virtual void SignExternalContainerPQCTest(String signatureAlgo, String digestAlgo, String expectedSigAlgoIdentifier
            , int signatureBytesSize) {
            String outFile = System.IO.Path.Combine(DESTINATION_FOLDER, "ext_cont_" + signatureAlgo + ".pdf").ToString
                ();
            String certPath = SOURCE_FOLDER + "cert_" + signatureAlgo + ".pem";
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outFile));
            if (FIPS_MODE) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(Exception), () => DoSignExternalContainer(digestAlgo, certPath
                    , outFile, signatureBytesSize));
                NUnit.Framework.Assert.IsTrue(e.Message.Contains(MessageFormatUtil.Format(KernelExceptionMessageConstant.NO_SUCH_ALGORITHM_FOR_PROVIDER_BCFIPS
                    , expectedSigAlgoIdentifier)) || KernelExceptionMessageConstant.NO_SUCH_ALGORITHM.Equals(e.Message));
            }
            else {
                DoSignExternalContainer(digestAlgo, certPath, outFile, signatureBytesSize);
                DoVerify(outFile, expectedSigAlgoIdentifier);
            }
        }

        [NUnit.Framework.TestCaseSource("Algorithms")]
        [LogMessage(KernelLogMessageConstant.ALGORITHM_NOT_FROM_SPEC, Ignore = true)]
        public virtual void SignDeferredPQCTest(String signatureAlgo, String digestAlgo, String expectedSigAlgoIdentifier
            , int signatureBytesSize) {
            String preparedFile = System.IO.Path.Combine(DESTINATION_FOLDER, "prep_" + signatureAlgo + ".pdf").ToString
                ();
            String outFile = System.IO.Path.Combine(DESTINATION_FOLDER, "deferred_" + signatureAlgo + ".pdf").ToString
                ();
            String certPath = SOURCE_FOLDER + "cert_" + signatureAlgo + ".pem";
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outFile));
            PrepareDocForSignDeferred(preparedFile, signatureBytesSize);
            if (FIPS_MODE) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(Exception), () => DoSignDeferred(preparedFile, outFile, 
                    digestAlgo, certPath));
                NUnit.Framework.Assert.IsTrue(e.Message.Contains(MessageFormatUtil.Format(KernelExceptionMessageConstant.NO_SUCH_ALGORITHM_FOR_PROVIDER_BCFIPS
                    , expectedSigAlgoIdentifier)) || KernelExceptionMessageConstant.NO_SUCH_ALGORITHM.Equals(e.Message));
            }
            else {
                DoSignDeferred(preparedFile, outFile, digestAlgo, certPath);
                DoVerify(outFile, expectedSigAlgoIdentifier);
            }
        }

        [NUnit.Framework.TestCaseSource("Algorithms")]
        [LogMessage(KernelLogMessageConstant.ALGORITHM_NOT_FROM_SPEC, Ignore = true)]
        public virtual void TwoPhaseSigningPQCTest(String signatureAlgo, String digestAlgo, String expectedSigAlgoIdentifier
            , int signatureBytesSize) {
            String outFile = System.IO.Path.Combine(DESTINATION_FOLDER, "two_phase_" + signatureAlgo + ".pdf").ToString
                ();
            String certPath = SOURCE_FOLDER + "cert_" + signatureAlgo + ".pem";
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outFile));
            if (FIPS_MODE) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(Exception), () => DoTwoPhaseSigning(digestAlgo, certPath
                    , outFile, signatureBytesSize));
                NUnit.Framework.Assert.IsTrue(e.Message.Contains(MessageFormatUtil.Format(KernelExceptionMessageConstant.NO_SUCH_ALGORITHM_FOR_PROVIDER_BCFIPS
                    , expectedSigAlgoIdentifier)) || KernelExceptionMessageConstant.NO_SUCH_ALGORITHM.Equals(e.Message));
            }
            else {
                DoTwoPhaseSigning(digestAlgo, certPath, outFile, signatureBytesSize);
                DoVerify(outFile, expectedSigAlgoIdentifier);
            }
        }

        [NUnit.Framework.TestCaseSource("Algorithms")]
        [LogMessage(KernelLogMessageConstant.ALGORITHM_NOT_FROM_SPEC, Ignore = true)]
        public virtual void TimestampPQCTest(String signatureAlgo, String digestAlgo, String expectedSigAlgoIdentifier
            , int signatureBytesSize) {
            String outFile = System.IO.Path.Combine(DESTINATION_FOLDER, "timestamp_" + signatureAlgo + ".pdf").ToString
                ();
            String certPath = SOURCE_FOLDER + "timestamp/ts_cert_" + signatureAlgo + ".pem";
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outFile));
            if (FIPS_MODE) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(Exception), () => DoTimestamp(signatureAlgo, digestAlgo, 
                    certPath, outFile, signatureBytesSize));
                NUnit.Framework.Assert.IsTrue(e.Message.Contains(MessageFormatUtil.Format(KernelExceptionMessageConstant.NO_SUCH_ALGORITHM_FOR_PROVIDER_BCFIPS
                    , expectedSigAlgoIdentifier)) || KernelExceptionMessageConstant.NO_SUCH_ALGORITHM.Equals(e.Message));
            }
            else {
                DoTimestamp(signatureAlgo, digestAlgo, certPath, outFile, signatureBytesSize);
                DoVerify(outFile, expectedSigAlgoIdentifier);
            }
        }

        [NUnit.Framework.TestCaseSource("Algorithms")]
        [LogMessage(KernelLogMessageConstant.ALGORITHM_NOT_FROM_SPEC, Ignore = true)]
        public virtual void PadesLTASignatureLevelPQCTest(String signatureAlgo, String digestAlgo, String expectedSigAlgoIdentifier
            , int signatureBytesSize) {
            String outFile = System.IO.Path.Combine(DESTINATION_FOLDER, "pades_LTA_" + signatureAlgo + ".pdf").ToString
                ();
            String signCertFileName = SOURCE_FOLDER + "chain/sign_" + signatureAlgo + ".pem";
            String tsaCertFileName = SOURCE_FOLDER + "timestamp/ts_cert_" + signatureAlgo + ".pem";
            String caCertFileName = SOURCE_FOLDER + "cert_" + signatureAlgo + ".pem";
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outFile));
            if (FIPS_MODE) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(Exception), () => SignWithBaselineLTAProfile(signatureAlgo
                    , digestAlgo, signatureBytesSize, signCertFileName, tsaCertFileName, caCertFileName, outFile));
                NUnit.Framework.Assert.IsTrue(e.Message.Contains(MessageFormatUtil.Format(KernelExceptionMessageConstant.NO_SUCH_ALGORITHM_FOR_PROVIDER_BCFIPS
                    , expectedSigAlgoIdentifier)) || KernelExceptionMessageConstant.NO_SUCH_ALGORITHM.Equals(e.Message));
            }
            else {
                SignWithBaselineLTAProfile(signatureAlgo, digestAlgo, signatureBytesSize, signCertFileName, tsaCertFileName
                    , caCertFileName, outFile);
                ValidateExistingSignature(outFile, caCertFileName, tsaCertFileName);
            }
        }

        private static void DoSign(String signatureAlgo, String digestAlgo, String certPath, String outFile, int signatureBytesSize
            ) {
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(certPath);
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(certPath, KEY_PASSPHRASE);
            IExternalSignature pks = new PrivateKeySignature(signPrivateKey, digestAlgo, signatureAlgo, null);
            using (Stream @out = FileUtil.GetFileOutputStream(outFile)) {
                PdfSigner signer = new PdfSigner(new PdfReader(SOURCE_FILE), @out, new StampingProperties());
                SignerProperties signerProperties = GetSignerProperties("Approval test signature.\nCreated by iText.");
                signer.SetSignerProperties(signerProperties);
                signer.SignDetached(new BouncyCastleDigest(), pks, signChain, null, null, null, signatureBytesSize * 3, PdfSigner.CryptoStandard
                    .CMS);
            }
        }

        private static void DoSignExternalContainer(String digestAlgo, String certPath, String outFile, int signatureBytesSize
            ) {
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(certPath);
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(certPath, KEY_PASSPHRASE);
            using (Stream @out = FileUtil.GetFileOutputStream(outFile)) {
                PdfSigner signer = new PdfSigner(new PdfReader(SOURCE_FILE), @out, new StampingProperties());
                SignerProperties signerProperties = GetSignerProperties("Sign external container.\nCreated by iText.");
                signer.SetSignerProperties(signerProperties);
                PKCS7ExternalSignatureContainer pkcs7ExternalSignatureContainer = new PKCS7ExternalSignatureContainer(signPrivateKey
                    , signChain, digestAlgo);
                signer.SignExternalContainer(pkcs7ExternalSignatureContainer, 3 * signatureBytesSize);
            }
        }

        private static void PrepareDocForSignDeferred(String output, int estimatedSize) {
            PdfSigner signer = new PdfSigner(new PdfReader(SOURCE_FILE), FileUtil.GetFileOutputStream(output), new StampingProperties
                ());
            SignerProperties signerProperties = GetSignerProperties("Signature field which signing is deferred.");
            signer.SetSignerProperties(signerProperties);
            PdfName filter = PdfName.Adobe_PPKLite;
            PdfName subFilter = PdfName.Adbe_pkcs7_detached;
            IExternalSignatureContainer external = new ExternalBlankSignatureContainer(filter, subFilter);
            signer.SignExternalContainer(external, 3 * estimatedSize);
        }

        private static void DoSignDeferred(String srcFile, String outFile, String digestAlgo, String certPath) {
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(certPath);
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(certPath, KEY_PASSPHRASE);
            IExternalSignatureContainer extSigContainer = new PKCS7ExternalSignatureContainer(signPrivateKey, signChain
                , digestAlgo);
            using (PdfReader reader = new PdfReader(srcFile)) {
                using (Stream outStream = FileUtil.GetFileOutputStream(outFile)) {
                    PdfSigner.SignDeferred(reader, SIGNATURE_FIELD, outStream, extSigContainer);
                }
            }
        }

        private static void DoTwoPhaseSigning(String digestAlgo, String certPath, String outFile, int signatureBytesSize
            ) {
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(certPath);
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(certPath, KEY_PASSPHRASE);
            using (PdfReader reader = new PdfReader(FileUtil.GetInputStreamForFile(SOURCE_FILE))) {
                using (MemoryStream outputStream = new MemoryStream()) {
                    PdfTwoPhaseSigner signer = new PdfTwoPhaseSigner(reader, outputStream);
                    SignerProperties signerProperties = GetSignerProperties("Two-phase signing.\nCreated by iText.");
                    byte[] digest = signer.PrepareDocumentForSignature(signerProperties, digestAlgo, PdfName.Adobe_PPKLite, PdfName
                        .Adbe_pkcs7_detached, 3 * signatureBytesSize, false);
                    byte[] signData = SignDigest(digest, signChain, signPrivateKey, digestAlgo);
                    using (Stream outputStreamPhase2 = FileUtil.GetFileOutputStream(outFile)) {
                        using (PdfReader newReader = new PdfReader(new MemoryStream(outputStream.ToArray()))) {
                            PdfTwoPhaseSigner.AddSignatureToPreparedDocument(newReader, SIGNATURE_FIELD, outputStreamPhase2, signData);
                        }
                    }
                }
            }
        }

        private static byte[] SignDigest(byte[] data, IX509Certificate[] chain, IPrivateKey pk, String digestAlgo) {
            PdfPKCS7 sgn = new PdfPKCS7((IPrivateKey)null, chain, digestAlgo, new BouncyCastleDigest(), false);
            byte[] sh = sgn.GetAuthenticatedAttributeBytes(data, PdfSigner.CryptoStandard.CMS, null, null);
            PrivateKeySignature pkSign = new PrivateKeySignature(pk, digestAlgo);
            byte[] signData = pkSign.Sign(sh);
            sgn.SetExternalSignatureValue(signData, null, pkSign.GetSignatureAlgorithmName(), pkSign.GetSignatureMechanismParameters
                ());
            return sgn.GetEncodedPKCS7(data, PdfSigner.CryptoStandard.CMS, null, null, null);
        }

        private static void DoTimestamp(String signatureAlgo, String digestAlgo, String certPath, String outFile, 
            int signatureBytesSize) {
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(certPath);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(certPath, KEY_PASSPHRASE);
            using (Stream @out = FileUtil.GetFileOutputStream(outFile)) {
                PdfSigner signer = new PdfSigner(new PdfReader(SOURCE_FILE), @out, new StampingProperties());
                SignerProperties signerProperties = GetSignerProperties("Timestamp signature.\nCreated by iText.");
                signer.SetSignerProperties(signerProperties);
                ITSAClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey, 3 * signatureBytesSize
                    , signatureAlgo, digestAlgo);
                signer.Timestamp(testTsa, SIGNATURE_FIELD);
            }
        }

        private static void SignWithBaselineLTAProfile(String signatureAlgo, String digestAlgo, int signatureBytesSize
            , String signCertFileName, String tsaCertFileName, String caCertFileName, String outFile) {
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, KEY_PASSPHRASE);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, KEY_PASSPHRASE);
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, KEY_PASSPHRASE);
            SignerProperties signerProperties = GetSignerProperties("Sign with baseline-LTA profile.\nCreated by iText."
                );
            PdfPadesSigner padesSigner = new PdfPadesSigner(new PdfReader(FileUtil.GetInputStreamForFile(SOURCE_FILE))
                , FileUtil.GetFileOutputStream(outFile));
            padesSigner.SetEstimatedSize(10 * signatureBytesSize);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey, 3 * signatureBytesSize
                , signatureAlgo, digestAlgo);
            ICrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey, signatureAlgo);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey, signatureAlgo
                ).AddBuilderForCertIssuer((IX509Certificate)tsaChain[0], tsaPrivateKey, signatureAlgo);
            padesSigner.SetOcspClient(ocspClient).SetCrlClient(crlClient).SetTimestampSignatureName("timestampSig1");
            padesSigner.SignWithBaselineLTAProfile(signerProperties, signChain, signPrivateKey, testTsa);
        }

        private static SignerProperties GetSignerProperties(String description) {
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID).SetContent
                (description);
            return new SignerProperties().SetFieldName(SIGNATURE_FIELD).SetPageRect(new Rectangle(50, 650, 200, 100)).
                SetReason("Test").SetLocation("TestCity").SetSignatureAppearance(appearance);
        }

        private static void DoVerify(String fileName, String expectedSigAlgoIdentifier) {
            using (PdfReader r = new PdfReader(fileName)) {
                using (PdfDocument pdfDoc = new PdfDocument(r)) {
                    SignatureUtil u = new SignatureUtil(pdfDoc);
                    PdfPKCS7 data = u.ReadSignatureData(SIGNATURE_FIELD);
                    NUnit.Framework.Assert.IsTrue(data.VerifySignatureIntegrityAndAuthenticity());
                    if (expectedSigAlgoIdentifier != null) {
                        NUnit.Framework.Assert.AreEqual(expectedSigAlgoIdentifier, data.GetSignatureMechanismOid());
                    }
                }
            }
        }

        private static void ValidateExistingSignature(String src, String rootCert, String tsaCert) {
            SignatureValidationProperties properties = GetSignatureValidationProperties();
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCert)[0];
            IX509Certificate caTsaCert = (IX509Certificate)PemFileHelper.ReadFirstChain(tsaCert)[0];
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            certificateRetriever.SetTrustedCertificates(JavaUtil.ArraysAsList(caCert, caTsaCert));
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder().WithIssuingCertificateRetrieverFactory
                (() => certificateRetriever).WithRevocationDataValidatorFactory(() => new MockRevocationDataValidator(
                )).WithSignatureValidationProperties(properties);
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(src))) {
                SignatureValidator validator = validatorChainBuilder.BuildSignatureValidator(document);
                report = validator.ValidateSignatures();
            }
            System.Diagnostics.Debug.Assert((report.GetValidationResult().Equals(ValidationReport.ValidationResult.VALID
                )));
        }

        private static SignatureValidationProperties GetSignatureValidationProperties() {
            SignatureValidationProperties properties = new SignatureValidationProperties();
            properties.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH);
            properties.SetFreshness(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts.Of(TimeBasedContext
                .HISTORICAL), TimeSpan.FromDays(0));
            properties.SetContinueAfterFailure(ValidatorContexts.Of(ValidatorContext.OCSP_VALIDATOR, ValidatorContext.
                CRL_VALIDATOR), CertificateSources.Of(CertificateSource.CRL_ISSUER, CertificateSource.OCSP_ISSUER, CertificateSource
                .CERT_ISSUER), true);
            return properties;
        }
    }
}
