/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
using iText.Signatures.Testutils.Client;
using iText.Test;
using iText.Test.Attributes;
using iText.Test.Signutils;

namespace iText.Signatures.Verify {
    [NUnit.Framework.Category("UnitTest")]
    public class OcspCertificateVerificationTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        // Such messageTemplate is equal to any log message. This is required for porting reasons.
        private const String ANY_LOG_MESSAGE = "{0}";

        private static readonly String ocspCertsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/verify/OcspCertificateVerificationTest/";

        private static readonly String rootOcspCert = ocspCertsSrc + "ocspRootRsa.p12";

        private static readonly String signOcspCert = ocspCertsSrc + "ocspSignRsa.p12";

        private static readonly String notOcspAndOcspCert = ocspCertsSrc + "notOcspAndOcspCertificates.p12";

        private static readonly char[] password = "testpass".ToCharArray();

        private const String ocspServiceUrl = "http://localhost:9000/demo/ocsp/ocsp-service";

        private static IX509Certificate checkCert;

        private static IX509Certificate rootCert;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            checkCert = (IX509Certificate)Pkcs12FileHelper.ReadFirstChain(signOcspCert, password)[0];
            rootCert = (IX509Certificate)Pkcs12FileHelper.ReadFirstChain(rootOcspCert, password)[0];
        }

        [NUnit.Framework.Test]
        public virtual void KeyStoreWithRootOcspCertificateTest() {
            IBasicOCSPResponse response = GetOcspResponse();
            NUnit.Framework.Assert.IsTrue(CertificateVerification.VerifyOcspCertificates(response, Pkcs12FileHelper.InitStore
                (rootOcspCert, password)));
        }

        [NUnit.Framework.Test]
        public virtual void KeyStoreWithSignOcspCertificateTest() {
            IBasicOCSPResponse response = GetOcspResponse();
            NUnit.Framework.Assert.IsFalse(CertificateVerification.VerifyOcspCertificates(response, Pkcs12FileHelper.InitStore
                (signOcspCert, password)));
        }

        [NUnit.Framework.Test]
        public virtual void KeyStoreWithNotOcspAndOcspCertificatesTest() {
            IBasicOCSPResponse response = GetOcspResponse();
            NUnit.Framework.Assert.IsTrue(CertificateVerification.VerifyOcspCertificates(response, Pkcs12FileHelper.InitStore
                (notOcspAndOcspCert, password)));
        }

        [NUnit.Framework.Test]
        [LogMessage(ANY_LOG_MESSAGE)]
        public virtual void KeyStoreWithNotOcspCertificateTest() {
            NUnit.Framework.Assert.IsFalse(CertificateVerification.VerifyOcspCertificates(null, Pkcs12FileHelper.InitStore
                (signOcspCert, password)));
        }

        private static IBasicOCSPResponse GetOcspResponse() {
            TestOcspClient testClient = new TestOcspClient();
            IPrivateKey key = Pkcs12FileHelper.ReadFirstKey(rootOcspCert, password, password);
            testClient.AddBuilderForCertIssuer(rootCert, key);
            byte[] ocspResponseBytes = testClient.GetEncoded(checkCert, rootCert, ocspServiceUrl);
            IASN1Primitive var2 = FACTORY.CreateASN1Primitive(ocspResponseBytes);
            return FACTORY.CreateBasicOCSPResponse(var2);
        }
    }
}
