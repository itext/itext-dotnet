/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.IO;
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

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void LtvEnabledTest01() {
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
            PdfSigner signer = new PdfSigner(new PdfReader(ltvFileName), new FileStream(ltvTsFileName, FileMode.Create
                ), new StampingProperties().UseAppendMode());
            signer.Timestamp(testTsa, "timestampSig1");
            BasicCheckLtvDoc("ltvEnabledTsTest01.pdf", "timestampSig1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(ltvTsFileName, SOURCE_FOLDER + "cmp_ltvEnabledTsTest01.pdf"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void LtvEnabledSingleSignatureNoCrlDataTest() {
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
            ICollection<ICrlClient> crlNotAvailableList = JavaUtil.ArraysAsList((ICrlClient)null, new _ICrlClient_149(
                ));
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), new FileStream(ltvFileName, FileMode.Create), 
                new StampingProperties());
            signer.SetFieldName("Signature1");
            signer.SignDetached(pks, signChain, crlNotAvailableList, testOcspClient, testTsa, 0, PdfSigner.CryptoStandard
                .CADES);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(ltvFileName, SOURCE_FOLDER + "cmp_ltvEnabledSingleSignatureNoCrlDataTest.pdf"
                ));
        }

        private sealed class _ICrlClient_149 : ICrlClient {
            public _ICrlClient_149() {
            }

            public ICollection<byte[]> GetEncoded(IX509Certificate checkCert, String url) {
                return null;
            }
        }

        [NUnit.Framework.Test]
        public virtual void LtvEnabledSingleSignatureNoOcspDataTest() {
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
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), new FileStream(ltvFileName, FileMode.Create), 
                new StampingProperties());
            signer.SetFieldName("Signature1");
            signer.SignDetached(pks, signChain, JavaCollectionsUtil.SingletonList<ICrlClient>(testCrlClient), null, testTsa
                , 0, PdfSigner.CryptoStandard.CADES);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(ltvFileName, SOURCE_FOLDER + "cmp_ltvEnabledSingleSignatureNoOcspDataTest.pdf"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SecondLtvOriginalHasNoVri01() {
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
            PdfSigner signer = new PdfSigner(new PdfReader(ltvFileName), new FileStream(ltvTsFileName, FileMode.Create
                ), new StampingProperties().UseAppendMode());
            signer.Timestamp(testTsa, "timestampSig2");
            BasicCheckLtvDoc("secondLtvOriginalHasNoVriTs01.pdf", "timestampSig2");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(ltvTsFileName, SOURCE_FOLDER + "cmp_secondLtvOriginalHasNoVriTs01.pdf"
                ));
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
            PadesSigTest.BasicCheckSignedDoc(DESTINATION_FOLDER + outFileName, tsSigName);
        }
    }
}
