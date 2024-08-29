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
using System.Collections.Generic;
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class LtvSigTest : ExtendedITextTest {
        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/LtvSigTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/LtvSigTest/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();
        private static bool runningInFipsMode;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
            runningInFipsMode = "BCFIPS".Equals(BouncyCastleFactoryCreator.GetFactory().GetProviderName());
    }

        [NUnit.Framework.Test]
        public virtual void LtvEnabledTest01() {
            string compareFile = SOURCE_FOLDER + "cmp_ltvEnabledTsTest01.pdf";
            if (runningInFipsMode) {
                compareFile = SOURCE_FOLDER + "cmp_ltvEnabledTsTest01_FIPS.pdf";
            }
            String tsaCertP12FileName = CERTS_SRC + "tsCertRsa.pem";
            String caCertP12FileName = CERTS_SRC + "rootRsa.pem";
            String srcFileName = SOURCE_FOLDER + "signedDoc.pdf";
            String ltvFileName = DESTINATION_FOLDER + "ltvEnabledTest01.pdf";
            String ltvTsFileName = DESTINATION_FOLDER + "ltvEnabledTsTest01.pdf";
            TestCrlClient testCrlClient = PrepareCrlClientForIssuer(caCertP12FileName);
            TestOcspClient testOcspClient = PrepareOcspClientForIssuer(caCertP12FileName);
            TestTsaClient testTsa = PrepareTsaClient(tsaCertP12FileName);
            PdfDocument document = new PdfDocument(new PdfReader(srcFileName), new PdfWriter(ltvFileName), new StampingProperties
                ().UseAppendMode());
            LtvVerification ltvVerification = new LtvVerification(document);
            ltvVerification.AddVerification("Signature1", testOcspClient, testCrlClient, LtvVerification.CertificateOption
                .SIGNING_CERTIFICATE, LtvVerification.Level.OCSP_CRL, LtvVerification.CertificateInclusion.YES);
            ltvVerification.Merge();
            document.Close();
            PdfSigner signer = new PdfSigner(new PdfReader(ltvFileName), FileUtil.GetFileOutputStream(ltvTsFileName),
                new StampingProperties().UseAppendMode());
            signer.Timestamp(testTsa, "timestampSig1");
            BasicCheckLtvDoc("ltvEnabledTsTest01.pdf", "timestampSig1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(ltvTsFileName, compareFile));
        }

        [NUnit.Framework.Test]
        public virtual void LtvEnabledSingleSignatureNoCrlDataTest() {
            string compareFile = SOURCE_FOLDER + "cmp_ltvEnabledSingleSignatureNoCrlDataTest.pdf";
            if (runningInFipsMode)
            {
                compareFile = SOURCE_FOLDER + "cmp_ltvEnabledSingleSignatureNoCrlDataTest_FIPS.pdf";
            }
            String signCertP12FileName = CERTS_SRC + "signCertRsaWithChain.pem";
            String tsaCertP12FileName = CERTS_SRC + "tsCertRsa.pem";
            String intermediateCertP12FileName = CERTS_SRC + "intermediateRsa.pem";
            String caCertP12FileName = CERTS_SRC + "rootRsa.pem";
            String srcFileName = SOURCE_FOLDER + "helloWorldDoc.pdf";
            String ltvFileName = DESTINATION_FOLDER + "ltvEnabledSingleSignatureNoCrlDataTest.pdf";
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(signCertP12FileName);
            IExternalSignature pks = PrepareSignatureHandler(signCertP12FileName);
            TestTsaClient testTsa = PrepareTsaClient(tsaCertP12FileName);
            TestOcspClient testOcspClient = PrepareOcspClientForIssuer(intermediateCertP12FileName, caCertP12FileName);
            ICollection<ICrlClient> crlNotAvailableList = JavaUtil.ArraysAsList((ICrlClient)null, new _ICrlClient_129(
                ));
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), FileUtil.GetFileOutputStream(ltvFileName), 
                new StampingProperties());
            signer.SetSignerProperties(new SignerProperties().SetFieldName("Signature1"));
            signer.SignDetached(new BouncyCastleDigest(), pks, signChain, crlNotAvailableList, testOcspClient, testTsa, 0, PdfSigner.CryptoStandard
                .CADES);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(ltvFileName, compareFile));
        }

        private sealed class _ICrlClient_129 : ICrlClient {
            public _ICrlClient_129() {
            }

            public ICollection<byte[]> GetEncoded(IX509Certificate checkCert, String url) {
                return null;
            }
        }

        [NUnit.Framework.Test]
        public virtual void LtvEnabledSingleSignatureNoOcspDataTest() {
            string compareFile = SOURCE_FOLDER + "cmp_ltvEnabledSingleSignatureNoOcspDataTest.pdf";
            if (runningInFipsMode)
            {
                compareFile = SOURCE_FOLDER + "cmp_ltvEnabledSingleSignatureNoOcspDataTest_FIPS.pdf";
            }
            String signCertP12FileName = CERTS_SRC + "signCertRsaWithChain.pem";
            String tsaCertP12FileName = CERTS_SRC + "tsCertRsa.pem";
            String intermediateCertP12FileName = CERTS_SRC + "intermediateRsa.pem";
            String caCertP12FileName = CERTS_SRC + "rootRsa.pem";
            String srcFileName = SOURCE_FOLDER + "helloWorldDoc.pdf";
            String ltvFileName = DESTINATION_FOLDER + "ltvEnabledSingleSignatureNoOcspDataTest.pdf";
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(signCertP12FileName);
            IExternalSignature pks = PrepareSignatureHandler(signCertP12FileName);
            TestTsaClient testTsa = PrepareTsaClient(tsaCertP12FileName);
            TestCrlClient testCrlClient = PrepareCrlClientForIssuer(caCertP12FileName, intermediateCertP12FileName);
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), FileUtil.GetFileOutputStream(ltvFileName), 
                new StampingProperties());
            signer.SetSignerProperties(new SignerProperties().SetFieldName("Signature1"));
            signer.SignDetached(new BouncyCastleDigest(), pks, signChain, JavaCollectionsUtil.SingletonList<ICrlClient>(testCrlClient), null, testTsa
                , 0, PdfSigner.CryptoStandard.CADES);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(ltvFileName, compareFile));
        }

        [NUnit.Framework.Test]
        public virtual void SecondLtvOriginalHasNoVri01() {
            string compareFile = SOURCE_FOLDER + "cmp_secondLtvOriginalHasNoVriTs01.pdf";
            if (runningInFipsMode)
            {
                compareFile = SOURCE_FOLDER + "cmp_secondLtvOriginalHasNoVriTs01_FIPS.pdf";
            }
            String tsaCertFileName = CERTS_SRC + "tsCertRsa.pem";
            String caCertFileName = CERTS_SRC + "rootRsa.pem";
            String srcFileName = SOURCE_FOLDER + "ltvEnabledNoVriEntry.pdf";
            String ltvFileName = DESTINATION_FOLDER + "secondLtvOriginalHasNoVri01.pdf";
            String ltvTsFileName = DESTINATION_FOLDER + "secondLtvOriginalHasNoVriTs01.pdf";
            TestCrlClient testCrlClient = PrepareCrlClientForIssuer(caCertFileName);
            TestOcspClient testOcspClient = PrepareOcspClientForIssuer(caCertFileName);
            TestTsaClient testTsa = PrepareTsaClient(tsaCertFileName);
            PdfDocument document = new PdfDocument(new PdfReader(srcFileName), new PdfWriter(ltvFileName), new StampingProperties
                ().UseAppendMode());
            LtvVerification ltvVerification = new LtvVerification(document);
            ltvVerification.AddVerification("timestampSig1", testOcspClient, testCrlClient, LtvVerification.CertificateOption
                .SIGNING_CERTIFICATE, LtvVerification.Level.OCSP_CRL, LtvVerification.CertificateInclusion.YES);
            ltvVerification.Merge();
            document.Close();
            PdfSigner signer = new PdfSigner(new PdfReader(ltvFileName), FileUtil.GetFileOutputStream(ltvTsFileName),
                new StampingProperties().UseAppendMode());
            signer.Timestamp(testTsa, "timestampSig2");
            BasicCheckLtvDoc("secondLtvOriginalHasNoVriTs01.pdf", "timestampSig2");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(ltvTsFileName, compareFile));
        }

        private static IExternalSignature PrepareSignatureHandler(String signCertP12FileName) {
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(signCertP12FileName, PASSWORD);
            return new PrivateKeySignature(signPrivateKey, DigestAlgorithms.SHA256);
        }

        private static TestCrlClient PrepareCrlClientForIssuer(params String[] issuerCertP12FileNames) {
            TestCrlClient testCrlClient = new TestCrlClient();
            foreach (String issuerP12File in issuerCertP12FileNames) {
                IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(issuerP12File)[0];
                IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(issuerP12File, PASSWORD);
                testCrlClient.AddBuilderForCertIssuer(caCert, caPrivateKey);
            }
            return testCrlClient;
        }

        private static TestOcspClient PrepareOcspClientForIssuer(params String[] issuerCertP12FileNames) {
            TestOcspClient ocspClient = new TestOcspClient();
            foreach (String issuerP12File in issuerCertP12FileNames) {
                IX509Certificate issuerCertificate = (IX509Certificate)PemFileHelper.ReadFirstChain(issuerP12File)[0];
                IPrivateKey issuerPrivateKey = PemFileHelper.ReadFirstKey(issuerP12File, PASSWORD);
                ocspClient.AddBuilderForCertIssuer(issuerCertificate, issuerPrivateKey);
            }
            return ocspClient;
        }

        private static TestTsaClient PrepareTsaClient(String tsaCertP12FileName) {
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertP12FileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertP12FileName, PASSWORD);
            return new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
        }

        private void BasicCheckLtvDoc(String outFileName, String tsSigName) {
            PdfDocument outDocument = new PdfDocument(new PdfReader(DESTINATION_FOLDER + outFileName));
            PdfDictionary dssDict = outDocument.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.DSS);
            NUnit.Framework.Assert.IsNotNull(dssDict);
            NUnit.Framework.Assert.AreEqual(4, dssDict.Size());
            outDocument.Close();
            TestSignUtils.BasicCheckSignedDoc(DESTINATION_FOLDER + outFileName, tsSigName);
        }
    }
}
