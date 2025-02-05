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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfPadesMissingCertificatesTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfPadesMissingCertificatesTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PdfPadesMissingCertificatesTest/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CertificateFromAiaIsIncorrectTest() {
            String srcFileName = SOURCE_FOLDER + "helloWorldDoc.pdf";
            String rootCertFileName = CERTS_SRC + "root.pem";
            String intermediateCertFileName = CERTS_SRC + "intermediate.pem";
            String signCertFileName = CERTS_SRC + "sign.pem";
            IX509Certificate signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            SignerProperties signerProperties = CreateSignerProperties();
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IX509Certificate intermediateCert = (IX509Certificate)PemFileHelper.ReadFirstChain(intermediateCertFileName
                )[0];
            MemoryStream outputStream = new MemoryStream();
            PdfPadesSigner padesSigner = new PdfPadesSigner(new PdfReader(FileUtil.GetInputStreamForFile(srcFileName))
                , outputStream);
            IssuingCertificateRetriever issuingCertificateRetriever = new _IssuingCertificateRetriever_98(rootCertFileName
                );
            issuingCertificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(intermediateCert));
            padesSigner.SetIssuingCertificateRetriever(issuingCertificateRetriever);
            IX509Certificate[] signChain = new IX509Certificate[] { signCert, rootCert };
            padesSigner.SignWithBaselineBProfile(signerProperties, signChain, signPrivateKey);
            outputStream.Dispose();
            TestSignUtils.BasicCheckSignedDoc(new MemoryStream(outputStream.ToArray()), "Signature1");
            IList<IX509Certificate> certs = JavaUtil.ArraysAsList(rootCert, intermediateCert, signCert);
            TestSignUtils.SignedDocumentContainsCerts(new MemoryStream(outputStream.ToArray()), certs, "Signature1");
        }

        private sealed class _IssuingCertificateRetriever_98 : IssuingCertificateRetriever {
            public _IssuingCertificateRetriever_98(String rootCertFileName) {
                this.rootCertFileName = rootCertFileName;
            }

            protected internal override Stream GetIssuerCertByURI(String uri) {
                return FileUtil.GetInputStreamForFile(rootCertFileName);
            }

            private readonly String rootCertFileName;
        }

        [NUnit.Framework.Test]
        public virtual void RetrieveMissingCertificatesTest() {
            String srcFileName = SOURCE_FOLDER + "helloWorldDoc.pdf";
            String rootCertFileName = CERTS_SRC + "root.pem";
            String intermediateCertFileName = CERTS_SRC + "intermediate.pem";
            String signCertFileName = CERTS_SRC + "sign.pem";
            String rootCrlFileName = CERTS_SRC + "crlRoot.pem";
            String intermediateCrlFileName = CERTS_SRC + "crlIntermediate.pem";
            String crlCertFileName = CERTS_SRC + "crlCert.pem";
            String rootOcspFileName = CERTS_SRC + "ocspRoot.pem";
            String intermediateOscpFileName = CERTS_SRC + "ocspIntermediate.pem";
            String ocspCertFileName = CERTS_SRC + "ocspCert.pem";
            String rootTsaFileName = CERTS_SRC + "tsaRoot.pem";
            String intermediateTsaFileName = CERTS_SRC + "tsaIntermediate.pem";
            String tsaCertFileName = CERTS_SRC + "tsaCert.pem";
            String crlSignedByCA = CERTS_SRC + "crlSignedByCA.crl";
            String crlSignedByCrlCert = CERTS_SRC + "crlSignedByCrlCert.crl";
            IX509Certificate signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            IX509Certificate crlCert = (IX509Certificate)PemFileHelper.ReadFirstChain(crlCertFileName)[0];
            IX509Certificate ocspCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspCertFileName)[0];
            IPrivateKey ocspPrivateKey = PemFileHelper.ReadFirstKey(ocspCertFileName, PASSWORD);
            IX509Certificate tsaCert = (IX509Certificate)PemFileHelper.ReadFirstChain(tsaCertFileName)[0];
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
            SignerProperties signerProperties = CreateSignerProperties();
            TestTsaClient testTsa = new TestTsaClient(JavaCollectionsUtil.SingletonList(tsaCert), tsaPrivateKey);
            CrlClientOnline testCrlClient = new _CrlClientOnline_149(crlSignedByCrlCert, crlSignedByCA);
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IX509Certificate crlRootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCrlFileName)[0];
            IX509Certificate ocspRootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootOcspFileName)[0];
            IX509Certificate tsaRootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootTsaFileName)[0];
            IX509Certificate crlIntermediateCert = (IX509Certificate)PemFileHelper.ReadFirstChain(intermediateCrlFileName
                )[0];
            IX509Certificate ocspIntermediateCert = (IX509Certificate)PemFileHelper.ReadFirstChain(intermediateOscpFileName
                )[0];
            IX509Certificate tsaIntermediateCert = (IX509Certificate)PemFileHelper.ReadFirstChain(intermediateTsaFileName
                )[0];
            IX509Certificate intermediateCert = (IX509Certificate)PemFileHelper.ReadFirstChain(intermediateCertFileName
                )[0];
            AdvancedTestOcspClient ocspClient = new AdvancedTestOcspClient();
            ocspClient.AddBuilderForCertIssuer(signCert, ocspCert, ocspPrivateKey);
            ocspClient.AddBuilderForCertIssuer(ocspIntermediateCert, ocspCert, ocspPrivateKey);
            ocspClient.AddBuilderForCertIssuer(crlIntermediateCert, ocspCert, ocspPrivateKey);
            ocspClient.AddBuilderForCertIssuer(tsaIntermediateCert, ocspCert, ocspPrivateKey);
            ocspClient.AddBuilderForCertIssuer(rootCert, ocspCert, ocspPrivateKey);
            ocspClient.AddBuilderForCertIssuer(crlRootCert, ocspCert, ocspPrivateKey);
            ocspClient.AddBuilderForCertIssuer(ocspRootCert, ocspCert, ocspPrivateKey);
            ocspClient.AddBuilderForCertIssuer(tsaRootCert, ocspCert, ocspPrivateKey);
            MemoryStream outputStream = new MemoryStream();
            PdfPadesSigner padesSigner = new PdfPadesSigner(new PdfReader(FileUtil.GetInputStreamForFile(srcFileName))
                , outputStream);
            padesSigner.SetCrlClient(testCrlClient);
            padesSigner.SetOcspClient(ocspClient);
            IIssuingCertificateRetriever issuingCertificateRetriever = new _IssuingCertificateRetriever_184(crlCertFileName
                , intermediateCrlFileName, rootCrlFileName, intermediateTsaFileName, rootTsaFileName, intermediateOscpFileName
                , rootOcspFileName, intermediateCertFileName, rootCertFileName);
            padesSigner.SetIssuingCertificateRetriever(issuingCertificateRetriever);
            IX509Certificate[] signChain = new IX509Certificate[] { signCert };
            padesSigner.SignWithBaselineLTProfile(signerProperties, signChain, signPrivateKey, testTsa);
            outputStream.Dispose();
            TestSignUtils.BasicCheckSignedDoc(new MemoryStream(outputStream.ToArray()), "Signature1");
            IDictionary<String, int?> expectedNumberOfCrls = new Dictionary<String, int?>();
            IDictionary<String, int?> expectedNumberOfOcsps = new Dictionary<String, int?>();
            // It is expected to have two CRL responses, one for intermediate cert and another for leaf CRL/OCSP/TSA certs.
            expectedNumberOfCrls.Put(crlCert.GetSubjectDN().ToString(), 1);
            expectedNumberOfCrls.Put(rootCert.GetSubjectDN().ToString(), 1);
            // It is expected to have OCSP responses for all the root, CRL/OCSP/TSA intermediate certs, and signing cert.
            expectedNumberOfOcsps.Put(ocspCert.GetSubjectDN().ToString(), 8);
            IList<String> certs = JavaUtil.ArraysAsList(GetCertName(rootCert), GetCertName(crlRootCert), GetCertName(crlCert
                ), GetCertName(ocspCert), GetCertName(tsaRootCert), GetCertName(crlIntermediateCert), GetCertName(ocspIntermediateCert
                ), GetCertName(tsaIntermediateCert), GetCertName(ocspRootCert), GetCertName(signCert), GetCertName(tsaCert
                ), GetCertName(intermediateCert));
            TestSignUtils.AssertDssDict(new MemoryStream(outputStream.ToArray()), expectedNumberOfCrls, expectedNumberOfOcsps
                , certs);
        }

        private sealed class _CrlClientOnline_149 : CrlClientOnline {
            public _CrlClientOnline_149(String crlSignedByCrlCert, String crlSignedByCA) {
                this.crlSignedByCrlCert = crlSignedByCrlCert;
                this.crlSignedByCA = crlSignedByCA;
            }

            protected internal override Stream GetCrlResponse(IX509Certificate cert, Uri urlt) {
                if (urlt.ToString().Contains("sign-crl")) {
                    return FileUtil.GetInputStreamForFile(crlSignedByCrlCert);
                }
                return FileUtil.GetInputStreamForFile(crlSignedByCA);
            }

            private readonly String crlSignedByCrlCert;

            private readonly String crlSignedByCA;
        }

        private sealed class _IssuingCertificateRetriever_184 : IssuingCertificateRetriever {
            public _IssuingCertificateRetriever_184(String crlCertFileName, String intermediateCrlFileName, String rootCrlFileName
                , String intermediateTsaFileName, String rootTsaFileName, String intermediateOscpFileName, String rootOcspFileName
                , String intermediateCertFileName, String rootCertFileName) {
                this.crlCertFileName = crlCertFileName;
                this.intermediateCrlFileName = intermediateCrlFileName;
                this.rootCrlFileName = rootCrlFileName;
                this.intermediateTsaFileName = intermediateTsaFileName;
                this.rootTsaFileName = rootTsaFileName;
                this.intermediateOscpFileName = intermediateOscpFileName;
                this.rootOcspFileName = rootOcspFileName;
                this.intermediateCertFileName = intermediateCertFileName;
                this.rootCertFileName = rootCertFileName;
            }

            protected internal override Stream GetIssuerCertByURI(String uri) {
                if (uri.Contains("crl_cert")) {
                    return FileUtil.GetInputStreamForFile(crlCertFileName);
                }
                if (uri.Contains("crl_intermediate")) {
                    return FileUtil.GetInputStreamForFile(intermediateCrlFileName);
                }
                if (uri.Contains("crl_root")) {
                    return FileUtil.GetInputStreamForFile(rootCrlFileName);
                }
                if (uri.Contains("tsa_intermediate")) {
                    return FileUtil.GetInputStreamForFile(intermediateTsaFileName);
                }
                if (uri.Contains("tsa_root")) {
                    return FileUtil.GetInputStreamForFile(rootTsaFileName);
                }
                if (uri.Contains("ocsp_intermediate")) {
                    return FileUtil.GetInputStreamForFile(intermediateOscpFileName);
                }
                if (uri.Contains("ocsp_root")) {
                    return FileUtil.GetInputStreamForFile(rootOcspFileName);
                }
                if (uri.Contains("intermediate")) {
                    return FileUtil.GetInputStreamForFile(intermediateCertFileName);
                }
                return FileUtil.GetInputStreamForFile(rootCertFileName);
            }

            private readonly String crlCertFileName;

            private readonly String intermediateCrlFileName;

            private readonly String rootCrlFileName;

            private readonly String intermediateTsaFileName;

            private readonly String rootTsaFileName;

            private readonly String intermediateOscpFileName;

            private readonly String rootOcspFileName;

            private readonly String intermediateCertFileName;

            private readonly String rootCertFileName;
        }

        [NUnit.Framework.Test]
        public virtual void RetrieveMissingCertificatesUsingTrustedStoreTest() {
            String srcFileName = SOURCE_FOLDER + "helloWorldDoc.pdf";
            String rootCertFileName = SOURCE_FOLDER + "root.pem";
            String signCertFileName = SOURCE_FOLDER + "sign.pem";
            String rootCrlFileName = SOURCE_FOLDER + "crlRoot.pem";
            String crlCertFileName = SOURCE_FOLDER + "crlCert.pem";
            String tsaCertFileName = SOURCE_FOLDER + "tsCert.pem";
            String crlSignedByCA = SOURCE_FOLDER + "crlWithRootIssuer.crl";
            String crlSignedByCrlCert = SOURCE_FOLDER + "crlWithCrlIssuer.crl";
            IX509Certificate signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            IX509Certificate crlCert = (IX509Certificate)PemFileHelper.ReadFirstChain(crlCertFileName)[0];
            IX509Certificate tsaCert = (IX509Certificate)PemFileHelper.ReadFirstChain(tsaCertFileName)[0];
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
            SignerProperties signerProperties = CreateSignerProperties();
            TestTsaClient testTsa = new TestTsaClient(JavaCollectionsUtil.SingletonList(tsaCert), tsaPrivateKey);
            CrlClientOnline testCrlClient = new _CrlClientOnline_260(crlSignedByCrlCert, crlSignedByCA);
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IX509Certificate crlRootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCrlFileName)[0];
            MemoryStream outputStream = new MemoryStream();
            PdfPadesSigner padesSigner = new PdfPadesSigner(new PdfReader(FileUtil.GetInputStreamForFile(srcFileName))
                , outputStream);
            padesSigner.SetCrlClient(testCrlClient);
            IList<IX509Certificate> trustedCertificates = new List<IX509Certificate>();
            trustedCertificates.Add(rootCert);
            trustedCertificates.Add(crlRootCert);
            trustedCertificates.Add(crlCert);
            padesSigner.SetTrustedCertificates(trustedCertificates);
            IX509Certificate[] signChain = new IX509Certificate[] { signCert };
            padesSigner.SignWithBaselineLTProfile(signerProperties, signChain, signPrivateKey, testTsa);
            outputStream.Dispose();
            TestSignUtils.BasicCheckSignedDoc(new MemoryStream(outputStream.ToArray()), "Signature1");
            IDictionary<String, int?> expectedNumberOfCrls = new Dictionary<String, int?>();
            IDictionary<String, int?> expectedNumberOfOcsps = new Dictionary<String, int?>();
            // It is expected to have two CRL responses, one for signing cert and another for CRL response.
            expectedNumberOfCrls.Put(crlCert.GetSubjectDN().ToString(), 1);
            expectedNumberOfCrls.Put(rootCert.GetSubjectDN().ToString(), 1);
            IList<String> certs = JavaUtil.ArraysAsList(GetCertName(rootCert), GetCertName(crlRootCert), GetCertName(crlCert
                ), GetCertName(signCert), GetCertName(tsaCert));
            TestSignUtils.AssertDssDict(new MemoryStream(outputStream.ToArray()), expectedNumberOfCrls, expectedNumberOfOcsps
                , certs);
        }

        private sealed class _CrlClientOnline_260 : CrlClientOnline {
            public _CrlClientOnline_260(String crlSignedByCrlCert, String crlSignedByCA) {
                this.crlSignedByCrlCert = crlSignedByCrlCert;
                this.crlSignedByCA = crlSignedByCA;
            }

            protected internal override Stream GetCrlResponse(IX509Certificate cert, Uri urlt) {
                if (urlt.ToString().Contains("cert-crl")) {
                    return FileUtil.GetInputStreamForFile(crlSignedByCrlCert);
                }
                return FileUtil.GetInputStreamForFile(crlSignedByCA);
            }

            private readonly String crlSignedByCrlCert;

            private readonly String crlSignedByCA;
        }

        private String GetCertName(IX509Certificate certificate) {
            return certificate.GetSubjectDN().ToString();
        }

        private SignerProperties CreateSignerProperties() {
            SignerProperties signerProperties = new SignerProperties();
            signerProperties.SetFieldName("Signature1");
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID).SetContent
                ("Approval test signature.\nCreated by iText.");
            signerProperties.SetPageRect(new Rectangle(50, 650, 200, 100)).SetSignatureAppearance(appearance);
            return signerProperties;
        }
    }
}
