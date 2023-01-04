/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Exceptions;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Client;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures.Verify {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class CertificateVerificationClassTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        // Such messageTemplate is equal to any log message. This is required for porting reasons.
        private const String ANY_LOG_MESSAGE = "{0}";

        private const int COUNTER_TO_MAKE_CRL_AVAILABLE_AT_THE_CURRENT_TIME = -1;

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void After() {
        }

        [NUnit.Framework.Test]
        public virtual void ValidCertificateChain01() {
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS_SRC + "signCertRsaWithChain.pem");
            String caCertFileName = CERTS_SRC + "rootRsa.pem";
            List<IX509Certificate> caKeyStore = PemFileHelper.InitStore(caCertFileName);
            IList<VerificationException> verificationExceptions = CertificateVerification.VerifyCertificates(certChain
                , caKeyStore);
            NUnit.Framework.Assert.IsTrue(verificationExceptions.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void TimestampCertificateAndKeyStoreCorrespondTest() {
            String tsaCertFileName = CERTS_SRC + "tsCertRsa.pem";
            List<IX509Certificate> caKeyStore = PemFileHelper.InitStore(tsaCertFileName);
            NUnit.Framework.Assert.IsTrue(VerifyTimestampCertificates(tsaCertFileName, caKeyStore));
        }

        [NUnit.Framework.Test]
        [LogMessage("certificate hash does not match certID hash.")]
        public virtual void TimestampCertificateAndKeyStoreDoNotCorrespondTest() {
            String tsaCertFileName = CERTS_SRC + "tsCertRsa.pem";
            String notTsaCertFileName = CERTS_SRC + "rootRsa.pem";
            List<IX509Certificate> caKeyStore = PemFileHelper.InitStore(notTsaCertFileName);
            NUnit.Framework.Assert.IsFalse(VerifyTimestampCertificates(tsaCertFileName, caKeyStore));
        }

        [NUnit.Framework.Test]
        [LogMessage(ANY_LOG_MESSAGE)]
        public virtual void KeyStoreWithoutCertificatesTest() {
            String tsaCertFileName = CERTS_SRC + "tsCertRsa.pem";
            NUnit.Framework.Assert.IsFalse(VerifyTimestampCertificates(tsaCertFileName, null));
        }

        [NUnit.Framework.Test]
        public virtual void ExpiredCertificateTest() {
            IX509Certificate expiredCert = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "expiredCert.pem"
                )[0];
            String verificationResult = CertificateVerification.VerifyCertificate(expiredCert, null);
            String expectedResultString = SignaturesTestUtils.GetExpiredMessage(expiredCert);
            NUnit.Framework.Assert.AreEqual(expectedResultString, verificationResult);
        }

        [NUnit.Framework.Test]
        public virtual void UnsupportedCriticalExtensionTest() {
            IX509Certificate unsupportedExtensionCert = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "unsupportedCriticalExtensionCert.pem"
                )[0];
            String verificationResult = CertificateVerification.VerifyCertificate(unsupportedExtensionCert, null);
            NUnit.Framework.Assert.AreEqual(CertificateVerification.HAS_UNSUPPORTED_EXTENSIONS, verificationResult);
        }

        [NUnit.Framework.Test]
        public virtual void ClrWithGivenCertificateTest() {
            String caCertFileName = CERTS_SRC + "rootRsa.pem";
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, PASSWORD);
            String checkCertFileName = CERTS_SRC + "signCertRsa01.pem";
            IX509Certificate checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, caPrivateKey, DateTimeUtil.GetCurrentUtcTime().AddDays
                (COUNTER_TO_MAKE_CRL_AVAILABLE_AT_THE_CURRENT_TIME));
            crlBuilder.AddCrlEntry(caCert, DateTimeUtil.GetCurrentUtcTime().AddDays(COUNTER_TO_MAKE_CRL_AVAILABLE_AT_THE_CURRENT_TIME
                ), FACTORY.CreateCRLReason().GetKeyCompromise());
            TestCrlBuilder crlForCheckBuilder = new TestCrlBuilder(caCert, caPrivateKey, DateTimeUtil.GetCurrentUtcTime
                ().AddDays(COUNTER_TO_MAKE_CRL_AVAILABLE_AT_THE_CURRENT_TIME));
            crlForCheckBuilder.AddCrlEntry(checkCert, DateTimeUtil.GetCurrentUtcTime().AddDays(COUNTER_TO_MAKE_CRL_AVAILABLE_AT_THE_CURRENT_TIME
                ), FACTORY.CreateCRLReason().GetKeyCompromise());
            TestCrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(crlBuilder);
            TestCrlClient crlForCheckClient = new TestCrlClient().AddBuilderForCertIssuer(crlForCheckBuilder);
            ICollection<byte[]> crlBytesForRootCertCollection = crlClient.GetEncoded(caCert, null);
            ICollection<byte[]> crlBytesForCheckCertCollection = crlForCheckClient.GetEncoded(checkCert, null);
            IList<IX509Crl> crls = new List<IX509Crl>();
            foreach (byte[] crlBytes in crlBytesForRootCertCollection) {
                crls.Add(SignTestPortUtil.ParseCrlFromStream(new MemoryStream(crlBytes)));
            }
            foreach (byte[] crlBytes in crlBytesForCheckCertCollection) {
                crls.Add(SignTestPortUtil.ParseCrlFromStream(new MemoryStream(crlBytes)));
            }
            String verificationResult = CertificateVerification.VerifyCertificate(checkCert, crls);
            NUnit.Framework.Assert.AreEqual(CertificateVerification.CERTIFICATE_REVOKED, verificationResult);
        }

        [NUnit.Framework.Test]
        public virtual void ValidCertWithEmptyCrlCollectionTest() {
            String caCertFileName = CERTS_SRC + "rootRsa.pem";
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            String verificationResult = CertificateVerification.VerifyCertificate(rootCert, JavaCollectionsUtil.EmptyList
                <IX509Crl>());
            NUnit.Framework.Assert.IsNull(verificationResult);
        }

        [NUnit.Framework.Test]
        public virtual void ValidCertWithCrlDoesNotContainCertTest() {
            int COUNTER_TO_MAKE_CRL_AVAILABLE_AT_THE_CURRENT_TIME = -1;
            String rootCertFileName = CERTS_SRC + "rootRsa.pem";
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            String certForAddingToCrlName = CERTS_SRC + "signCertRsa01.pem";
            IX509Certificate certForCrl = (IX509Certificate)PemFileHelper.ReadFirstChain(certForAddingToCrlName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(certForAddingToCrlName, PASSWORD);
            TestCrlBuilder crlForCheckBuilder = new TestCrlBuilder(certForCrl, caPrivateKey, DateTimeUtil.GetCurrentUtcTime
                ().AddDays(COUNTER_TO_MAKE_CRL_AVAILABLE_AT_THE_CURRENT_TIME));
            TestCrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(crlForCheckBuilder);
            ICollection<byte[]> crlBytesForRootCertCollection = crlClient.GetEncoded(certForCrl, null);
            IList<IX509Crl> crls = new List<IX509Crl>();
            foreach (byte[] crlBytes in crlBytesForRootCertCollection) {
                crls.Add(SignTestPortUtil.ParseCrlFromStream(new MemoryStream(crlBytes)));
            }
            NUnit.Framework.Assert.IsNull(CertificateVerification.VerifyCertificate(rootCert, crls));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyCertChainTest() {
            IX509Certificate[] emptyCertChain = new IX509Certificate[] {  };
            String expectedResult = MessageFormatUtil.Format("Certificate Unknown failed: {0}", SignExceptionMessageConstant
                .INVALID_STATE_WHILE_CHECKING_CERT_CHAIN);
            IList<VerificationException> resultedExceptionList = CertificateVerification.VerifyCertificates(emptyCertChain
                , null, (ICollection<IX509Crl>)null);
            NUnit.Framework.Assert.AreEqual(1, resultedExceptionList.Count);
            NUnit.Framework.Assert.AreEqual(expectedResult, resultedExceptionList[0].Message);
        }

        [NUnit.Framework.Test]
        public virtual void ValidCertChainWithEmptyKeyStoreTest() {
            String validCertChainFileName = CERTS_SRC + "signCertRsaWithChain.pem";
            String emptyCertChain = CERTS_SRC + "emptyCertChain.pem";
            IX509Certificate[] validCertChain = PemFileHelper.ReadFirstChain(validCertChainFileName);
            List<IX509Certificate> emptyKeyStore = PemFileHelper.InitStore(emptyCertChain);
            IList<VerificationException> resultedExceptionList = CertificateVerification.VerifyCertificates(validCertChain
                , emptyKeyStore, (ICollection<IX509Crl>)null);
            String expectedResult = MessageFormatUtil.Format(SignExceptionMessageConstant.CERTIFICATE_TEMPLATE_FOR_EXCEPTION_MESSAGE
                , FACTORY.CreateX500Name((IX509Certificate)validCertChain[2]).ToString(), SignExceptionMessageConstant
                .CANNOT_BE_VERIFIED_CERTIFICATE_CHAIN);
            NUnit.Framework.Assert.AreEqual(1, resultedExceptionList.Count);
            NUnit.Framework.Assert.AreEqual(expectedResult, resultedExceptionList[0].Message);
        }

        [NUnit.Framework.Test]
        public virtual void ValidCertChainWithRootCertAsKeyStoreTest() {
            String validCertChainFileName = CERTS_SRC + "signCertRsaWithChain.pem";
            String emptyCertChain = CERTS_SRC + "rootRsa.pem";
            IX509Certificate[] validCertChain = PemFileHelper.ReadFirstChain(validCertChainFileName);
            List<IX509Certificate> emptyKeyStore = PemFileHelper.InitStore(emptyCertChain);
            IList<VerificationException> resultedExceptionList = CertificateVerification.VerifyCertificates(validCertChain
                , emptyKeyStore, (ICollection<IX509Crl>)null);
            NUnit.Framework.Assert.AreEqual(0, resultedExceptionList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void CertChainWithExpiredCertTest() {
            String validCertChainFileName = CERTS_SRC + "signCertRsaWithExpiredChain.pem";
            IX509Certificate[] validCertChain = PemFileHelper.ReadFirstChain(validCertChainFileName);
            IX509Certificate expectedExpiredCert = (IX509Certificate)validCertChain[1];
            String expiredCertName = FACTORY.CreateX500Name(expectedExpiredCert).ToString();
            IX509Certificate rootCert = (IX509Certificate)validCertChain[2];
            String rootCertName = FACTORY.CreateX500Name(rootCert).ToString();
            IList<VerificationException> resultedExceptionList = CertificateVerification.VerifyCertificates(validCertChain
                , null, (ICollection<IX509Crl>)null);
            NUnit.Framework.Assert.AreEqual(2, resultedExceptionList.Count);
            String expectedFirstResultMessage = MessageFormatUtil.Format(SignExceptionMessageConstant.CERTIFICATE_TEMPLATE_FOR_EXCEPTION_MESSAGE
                , expiredCertName, SignaturesTestUtils.GetExpiredMessage(expectedExpiredCert));
            String expectedSecondResultMessage = MessageFormatUtil.Format(SignExceptionMessageConstant.CERTIFICATE_TEMPLATE_FOR_EXCEPTION_MESSAGE
                , rootCertName, SignExceptionMessageConstant.CANNOT_BE_VERIFIED_CERTIFICATE_CHAIN);
            NUnit.Framework.Assert.AreEqual(expectedFirstResultMessage, resultedExceptionList[0].Message);
            NUnit.Framework.Assert.AreEqual(expectedSecondResultMessage, resultedExceptionList[1].Message);
        }

        private static bool VerifyTimestampCertificates(String tsaClientCertificate, List<IX509Certificate> caKeyStore
            ) {
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaClientCertificate);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaClientCertificate, PASSWORD);
            TestTsaClient testTsaClient = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            byte[] tsaCertificateBytes = testTsaClient.GetTimeStampToken(testTsaClient.GetMessageDigest().Digest());
            ITimeStampToken timeStampToken = FACTORY.CreateTimeStampToken(FACTORY.CreateContentInfo(FACTORY.CreateASN1Sequence
                (tsaCertificateBytes)));
            return CertificateVerification.VerifyTimestampCertificates(timeStampToken, caKeyStore);
        }
    }
}
