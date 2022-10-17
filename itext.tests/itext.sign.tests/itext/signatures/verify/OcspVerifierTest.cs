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
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.X509;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Cert;
using iText.Signatures.Testutils.Client;
using iText.Test;
using iText.Test.Signutils;

namespace iText.Signatures.Verify {
    [NUnit.Framework.Category("UnitTest")]
    public class OcspVerifierTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] password = "testpass".ToCharArray();

        private static readonly String caCertFileName = certsSrc + "rootRsa.p12";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.Test]
        public virtual void ValidOcspTest01() {
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(caCertFileName, password)[0];
            ICipherParameters caPrivateKey = Pkcs12FileHelper.ReadFirstKey(caCertFileName, password, password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            NUnit.Framework.Assert.IsTrue(VerifyTest(builder));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidRevokedOcspTest01() {
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(caCertFileName, password)[0];
            ICipherParameters caPrivateKey = Pkcs12FileHelper.ReadFirstKey(caCertFileName, password, password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            builder.SetCertificateStatus(new RevokedStatus(DateTimeUtil.GetCurrentUtcTime().AddDays(-20), Org.BouncyCastle.Asn1.X509.CrlReason.KeyCompromise
                ));
            NUnit.Framework.Assert.IsFalse(VerifyTest(builder));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidUnknownOcspTest01() {
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(caCertFileName, password)[0];
            ICipherParameters caPrivateKey = Pkcs12FileHelper.ReadFirstKey(caCertFileName, password, password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            builder.SetCertificateStatus(new UnknownStatus());
            NUnit.Framework.Assert.IsFalse(VerifyTest(builder));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidOutdatedOcspTest01() {
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(caCertFileName, password)[0];
            ICipherParameters caPrivateKey = Pkcs12FileHelper.ReadFirstKey(caCertFileName, password, password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            DateTime thisUpdate = DateTimeUtil.GetCurrentTime().AddDays(-30);
            DateTime nextUpdate = DateTimeUtil.GetCurrentTime().AddDays(-15);
            builder.SetThisUpdate(thisUpdate);
            builder.SetNextUpdate(nextUpdate);
            NUnit.Framework.Assert.IsFalse(VerifyTest(builder));
        }

        [NUnit.Framework.Test]
        public virtual void ExpiredIssuerCertTest01() {
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(certsSrc + "intermediateExpiredCert.p12"
                , password)[0];
            ICipherParameters caPrivateKey = Pkcs12FileHelper.ReadFirstKey(certsSrc + "intermediateExpiredCert.p12", password
                , password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            NUnit.Framework.Assert.IsTrue(VerifyTest(builder, certsSrc + "signCertRsaWithExpiredChain.p12", caCert.NotBefore
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AuthorizedOCSPResponderTest() {
            DateTime ocspResponderCertStartDate = DateTimeUtil.GetCurrentUtcTime();
            DateTime ocspResponderCertEndDate = ocspResponderCertStartDate.AddDays(365 * 100);
            DateTime checkDate = DateTimeUtil.GetCurrentUtcTime();
            bool verifyRes = VerifyAuthorizedOCSPResponderTest(ocspResponderCertStartDate, ocspResponderCertEndDate, checkDate
                );
            NUnit.Framework.Assert.IsTrue(verifyRes);
        }

        [NUnit.Framework.Test]
        public virtual void ExpiredAuthorizedOCSPResponderTest_atValidPeriod() {
            DateTime ocspResponderCertStartDate = DateTimeUtil.Parse("15/10/2005", "dd/MM/yyyy");
            DateTime ocspResponderCertEndDate = DateTimeUtil.Parse("15/10/2010", "dd/MM/yyyy");
            DateTime checkDate = DateTimeUtil.Parse("15/10/2008", "dd/MM/yyyy");
            bool verifyRes = VerifyAuthorizedOCSPResponderTest(ocspResponderCertStartDate, ocspResponderCertEndDate, checkDate
                );
            NUnit.Framework.Assert.IsTrue(verifyRes);
        }

        [NUnit.Framework.Test]
        public virtual void ExpiredAuthorizedOCSPResponderTest_now() {
            DateTime ocspResponderCertStartDate = DateTimeUtil.Parse("15/10/2005", "dd/MM/yyyy");
            DateTime ocspResponderCertEndDate = DateTimeUtil.Parse("15/10/2010", "dd/MM/yyyy");
            DateTime checkDate = DateTimeUtil.GetCurrentUtcTime();
            NUnit.Framework.Assert.Catch(typeof(CertificateExpiredException), () => VerifyAuthorizedOCSPResponderTest(
                ocspResponderCertStartDate, ocspResponderCertEndDate, checkDate));
        }

        // Not getting here because of exception
        //Assert.assertFalse(verifyRes);
        private bool VerifyTest(TestOcspResponseBuilder rootRsaOcspBuilder) {
            return VerifyTest(rootRsaOcspBuilder, certsSrc + "signCertRsa01.p12", DateTimeUtil.GetCurrentUtcTime());
        }

        private bool VerifyTest(TestOcspResponseBuilder rootRsaOcspBuilder, String checkCertFileName, DateTime checkDate
            ) {
            X509Certificate checkCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(checkCertFileName, password)[
                0];
            X509Certificate rootCert = rootRsaOcspBuilder.GetIssuerCert();
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(rootCert, rootRsaOcspBuilder);
            byte[] basicOcspRespBytes = ocspClient.GetEncoded(checkCert, rootCert, null);
            Asn1Object var2 = Asn1Object.FromByteArray(basicOcspRespBytes);
            BasicOcspResp basicOCSPResp = new BasicOcspResp(BasicOcspResponse.GetInstance(var2));
            OCSPVerifier ocspVerifier = new OCSPVerifier(null, null);
            return ocspVerifier.Verify(basicOCSPResp, checkCert, rootCert, checkDate);
        }

        public virtual bool VerifyAuthorizedOCSPResponderTest(DateTime ocspResponderCertStartDate, DateTime ocspResponderCertEndDate
            , DateTime checkDate) {
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(certsSrc + "intermediateRsa.p12"
                , password)[0];
            ICipherParameters caPrivateKey = Pkcs12FileHelper.ReadFirstKey(certsSrc + "intermediateRsa.p12", password, 
                password);
            String checkCertFileName = certsSrc + "signCertRsaWithChain.p12";
            X509Certificate checkCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(checkCertFileName, password)[
                0];
            RsaKeyPairGenerator keyGen = SignTestPortUtil.BuildRSA2048KeyPairGenerator();
            AsymmetricCipherKeyPair key = keyGen.GenerateKeyPair();
            ICipherParameters ocspRespPrivateKey = key.Private;
            AsymmetricKeyParameter ocspRespPublicKey = key.Public;
            TestCertificateBuilder certBuilder = new TestCertificateBuilder(ocspRespPublicKey, caCert, caPrivateKey, "CN=iTextTestOCSPResponder, OU=test, O=iText"
                );
            certBuilder.SetStartDate(ocspResponderCertStartDate);
            certBuilder.SetEndDate(ocspResponderCertEndDate);
            X509Certificate ocspResponderCert = certBuilder.BuildAuthorizedOCSPResponderCert();
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(ocspResponderCert, ocspRespPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            byte[] basicOcspRespBytes = ocspClient.GetEncoded(checkCert, caCert, null);
            Asn1Object var2 = Asn1Object.FromByteArray(basicOcspRespBytes);
            BasicOcspResp basicOCSPResp = new BasicOcspResp(BasicOcspResponse.GetInstance(var2));
            OCSPVerifier ocspVerifier = new OCSPVerifier(null, null);
            return ocspVerifier.Verify(basicOCSPResp, checkCert, caCert, checkDate);
        }
    }
}
