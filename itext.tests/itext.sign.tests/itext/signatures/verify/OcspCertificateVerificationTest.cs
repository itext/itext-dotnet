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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures.Verify {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class OcspCertificateVerificationTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        // Such messageTemplate is equal to any log message. This is required for porting reasons.
        private const String ANY_LOG_MESSAGE = "{0}";

        private static readonly String ocspCertsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/verify/OcspCertificateVerificationTest/";

        private static readonly String rootOcspCert = ocspCertsSrc + "ocspRootRsa.pem";

        private static readonly String signOcspCert = ocspCertsSrc + "ocspSignRsa.pem";

        private static readonly String notOcspAndOcspCert = ocspCertsSrc + "notOcspAndOcspCertificates.pem";

        private static readonly char[] password = "testpassphrase".ToCharArray();

        private const String ocspServiceUrl = "http://localhost:9000/demo/ocsp/ocsp-service";

        private static IX509Certificate checkCert;

        private static IX509Certificate rootCert;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signOcspCert)[0];
            rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootOcspCert)[0];
        }

        [NUnit.Framework.Test]
        public virtual void KeyStoreWithRootOcspCertificateTest() {
            IBasicOcspResponse response = GetOcspResponse();
            NUnit.Framework.Assert.IsTrue(CertificateVerification.VerifyOcspCertificates(response, PemFileHelper.InitStore
                (rootOcspCert)));
        }

        [NUnit.Framework.Test]
        public virtual void KeyStoreWithSignOcspCertificateTest() {
            IBasicOcspResponse response = GetOcspResponse();
            NUnit.Framework.Assert.IsFalse(CertificateVerification.VerifyOcspCertificates(response, PemFileHelper.InitStore
                (signOcspCert)));
        }

        [NUnit.Framework.Test]
        public virtual void KeyStoreWithNotOcspAndOcspCertificatesTest() {
            IBasicOcspResponse response = GetOcspResponse();
            NUnit.Framework.Assert.IsTrue(CertificateVerification.VerifyOcspCertificates(response, PemFileHelper.InitStore
                (notOcspAndOcspCert)));
        }

        [NUnit.Framework.Test]
        [LogMessage(ANY_LOG_MESSAGE)]
        public virtual void KeyStoreWithNotOcspCertificateTest() {
            NUnit.Framework.Assert.IsFalse(CertificateVerification.VerifyOcspCertificates(null, PemFileHelper.InitStore
                (signOcspCert)));
        }

        private static IBasicOcspResponse GetOcspResponse() {
            TestOcspClient testClient = new TestOcspClient();
            IPrivateKey key = PemFileHelper.ReadFirstKey(rootOcspCert, password);
            testClient.AddBuilderForCertIssuer(rootCert, key);
            byte[] ocspResponseBytes = testClient.GetEncoded(checkCert, rootCert, ocspServiceUrl);
            IAsn1Object var2 = FACTORY.CreateASN1Primitive(ocspResponseBytes);
            return FACTORY.CreateBasicOCSPResponse(var2);
        }
    }
}
