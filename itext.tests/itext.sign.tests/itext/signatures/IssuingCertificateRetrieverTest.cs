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
using System.Linq;
using NUnit.Framework;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Client;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Mocks;
using iText.Test;

namespace iText.Signatures {
//\cond DO_NOT_DOCUMENT
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    internal class IssuingCertificateRetrieverTest : ExtendedITextTest {
        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.Test]
#if !NETSTANDARD2_0
        [NUnit.Framework.Timeout(10000)]
#endif // !NETSTANDARD2_0
        public virtual void InfiniteloopTest() {
            IssuingCertificateRetriever issuingCertificateRetriever = new IssuingCertificateRetriever();
            IX509Certificate[] cert = PemFileHelper.ReadFirstChain(CERTS_SRC + "crossSigned/sign.cert.pem");
            // the order of the known certificates is important,
            // changing it can make the test succeed without fix
            // the cross signed certificates that create the loop
            // must come first
            IX509Certificate[] knownCerts = new IX509Certificate[6];
            knownCerts[0] = PemFileHelper.ReadFirstChain(CERTS_SRC + "crossSigned/sign.cert.pem")[0];
            knownCerts[5] = PemFileHelper.ReadFirstChain(CERTS_SRC + "crossSigned/ca1.cert.pem")[0];
            knownCerts[4] = PemFileHelper.ReadFirstChain(CERTS_SRC + "crossSigned/ca2a.cert.pem")[0];
            knownCerts[3] = PemFileHelper.ReadFirstChain(CERTS_SRC + "crossSigned/ca2b.cert.pem")[0];
            knownCerts[2] = PemFileHelper.ReadFirstChain(CERTS_SRC + "crossSigned/ca3a.cert.pem")[0];
            knownCerts[1] = PemFileHelper.ReadFirstChain(CERTS_SRC + "crossSigned/ca3b.cert.pem")[0];
            issuingCertificateRetriever.AddKnownCertificates(JavaUtil.ArraysAsList(knownCerts));
            // An endless loop does not throw an exception, but it is caught the @Timeout annotation
            IX509Certificate[] result = issuingCertificateRetriever.RetrieveMissingCertificates(cert);
            NUnit.Framework.Assert.IsNotNull(result);
            NUnit.Framework.Assert.IsTrue(result.Length > 1);
        }

        [NUnit.Framework.Test]
#if !NETSTANDARD2_0
        [NUnit.Framework.Timeout(10000)]
#endif // !NETSTANDARD2_0
        public virtual void CertificateWithMultipleRootsTest() {
            IssuingCertificateRetriever issuingCertificateRetriever = new IssuingCertificateRetriever();
            IX509Certificate[] intialChain = PemFileHelper.ReadFirstChain(CERTS_SRC + "crossSigned/sign.cert.pem");
            IList<IX509Certificate> certificates = new List<IX509Certificate>();
            certificates.Add(PemFileHelper.ReadFirstChain(CERTS_SRC + "crossSigned/ca1.cert.pem")[0]);
            certificates.Add(PemFileHelper.ReadFirstChain(CERTS_SRC + "crossSigned/ca2a.cert.pem")[0]);
            certificates.Add(PemFileHelper.ReadFirstChain(CERTS_SRC + "crossSigned/ca2b.cert.pem")[0]);
            certificates.Add(PemFileHelper.ReadFirstChain(CERTS_SRC + "crossSigned/ca3a.cert.pem")[0]);
            certificates.Add(PemFileHelper.ReadFirstChain(CERTS_SRC + "crossSigned/ca3b.cert.pem")[0]);
            certificates.Add(PemFileHelper.ReadFirstChain(CERTS_SRC + "crossSigned/ca4.cert.pem")[0]);
            certificates.Add(PemFileHelper.ReadFirstChain(CERTS_SRC + "crossSigned/ca5.cert.pem")[0]);
            certificates.Add(PemFileHelper.ReadFirstChain(CERTS_SRC + "crossSigned/r1.cert.pem")[0]);
            certificates.Add(PemFileHelper.ReadFirstChain(CERTS_SRC + "crossSigned/r2.cert.pem")[0]);
            issuingCertificateRetriever.AddKnownCertificates(certificates);
            IX509Certificate[] result = issuingCertificateRetriever.RetrieveMissingCertificates(intialChain);
            NUnit.Framework.Assert.IsNotNull(result);
            NUnit.Framework.Assert.AreEqual(9, result.Length);
            NUnit.Framework.Assert.IsTrue(JavaUtil.ArraysToEnumerable(result).Any((c) => ((IX509Certificate)c).GetSubjectDN
                ().ToString().Contains("CN=iTextTestSign")));
            NUnit.Framework.Assert.IsTrue(JavaUtil.ArraysToEnumerable(result).Any((c) => ((IX509Certificate)c).GetSubjectDN
                ().ToString().Contains("CN=iTextTestIntermediate1")));
            NUnit.Framework.Assert.IsTrue(JavaUtil.ArraysToEnumerable(result).Any((c) => ((IX509Certificate)c).GetSubjectDN
                ().ToString().Contains("CN=iTextTestIntermediate2") && ((IX509Certificate)c).GetIssuerDN().ToString().
                Contains("CN=iTextTestIntermediate5")));
            NUnit.Framework.Assert.IsTrue(JavaUtil.ArraysToEnumerable(result).Any((c) => ((IX509Certificate)c).GetSubjectDN
                ().ToString().Contains("CN=iTextTestIntermediate2") && ((IX509Certificate)c).GetIssuerDN().ToString().
                Contains("CN=iTextTestIntermediate3")));
            NUnit.Framework.Assert.IsTrue(JavaUtil.ArraysToEnumerable(result).Any((c) => ((IX509Certificate)c).GetSubjectDN
                ().ToString().Contains("CN=iTextTestIntermediate3") && ((IX509Certificate)c).GetIssuerDN().ToString().
                Contains("CN=iTextTestIntermediate4")));
            NUnit.Framework.Assert.IsTrue(JavaUtil.ArraysToEnumerable(result).Any((c) => ((IX509Certificate)c).GetSubjectDN
                ().ToString().Contains("CN=iTextTestIntermediate4")));
            NUnit.Framework.Assert.IsTrue(JavaUtil.ArraysToEnumerable(result).Any((c) => ((IX509Certificate)c).GetSubjectDN
                ().ToString().Contains("CN=iTextTestIntermediate5")));
            NUnit.Framework.Assert.IsTrue(JavaUtil.ArraysToEnumerable(result).Any((c) => ((IX509Certificate)c).GetSubjectDN
                ().ToString().Contains("CN=iTextTestRoot1")));
            NUnit.Framework.Assert.IsTrue(JavaUtil.ArraysToEnumerable(result).Any((c) => ((IX509Certificate)c).GetSubjectDN
                ().ToString().Contains("CN=iTextTestRoot2")));
        }

        [NUnit.Framework.Test]
        public virtual void TestResourceRetrieverUsage() {
            IX509Certificate[] cert = PemFileHelper.ReadFirstChain(CERTS_SRC + "intermediate.pem");
            IList<Uri> urlsCalled = new List<Uri>();
            MockResourceRetriever mockRetriever = new MockResourceRetriever();
            mockRetriever.OnGetInputStreamByUrl((u) => {
                urlsCalled.Add(u);
                try {
                    return FileUtil.GetInputStreamForFile(CERTS_SRC + "root.pem");
                }
                catch (System.IO.IOException e) {
                    throw new Exception("Error reading certificate.", e);
                }
            }
            );
            ValidatorChainBuilder builder = new ValidatorChainBuilder().WithResourceRetriever(() => mockRetriever);
            builder.GetCertificateRetriever().RetrieveIssuerCertificate(cert[0]);
            NUnit.Framework.Assert.AreEqual(1, urlsCalled.Count);
            NUnit.Framework.Assert.AreEqual("http://test.example.com/example-ca/certs/ca/ca.crt", urlsCalled[0].ToString
                ());
        }

        [NUnit.Framework.Test]
        public virtual void OcspWithKeyHashTest() {
            IX509Certificate cert = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "rootRsa.pem")[0];
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(CERTS_SRC + "rootRsa.pem", PASSWORD);
            TestOcspClient testOcspClient = new TestOcspClient();
            TestOcspResponseBuilder responseBuilder = new TestOcspResponseBuilder(cert, signRsaPrivateKey);
            responseBuilder.SetResponseBuilder(FACTORY.CreateBasicOCSPRespBuilder(FACTORY.CreateRespID(cert)));
            testOcspClient.AddBuilderForCertIssuer(cert, responseBuilder);
            IBasicOcspResponse ocspResponse = testOcspClient.GetBasicOcspResp(cert, cert);
            IssuingCertificateRetriever issuingCertificateRetriever = new IssuingCertificateRetriever();
            ICollection<IX509Certificate> retrievers = issuingCertificateRetriever.RetrieveOCSPResponderByNameCertificate
                (ocspResponse);
            NUnit.Framework.Assert.AreEqual(1, retrievers.Count);
            NUnit.Framework.Assert.IsTrue(retrievers.Contains(cert));
        }
    }
//\endcond
}
