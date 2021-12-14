/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.X509;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Signatures.Exceptions;
using iText.Signatures.Testutils;
using iText.Test;
using iText.Test.Signutils;

namespace iText.Signatures {
    public class PdfPKCS7Test : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/PdfPKCS7Test/";

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] PASSWORD = "testpass".ToCharArray();

        private const double EPS = 0.001;

        private static X509Certificate[] chain;

        private static ICipherParameters pk;

        [NUnit.Framework.OneTimeSetUp]
        public static void Init() {
            pk = Pkcs12FileHelper.ReadFirstKey(CERTS_SRC + "signCertRsa01.p12", PASSWORD, PASSWORD);
            chain = Pkcs12FileHelper.ReadFirstChain(CERTS_SRC + "signCertRsa01.p12", PASSWORD);
        }

        [NUnit.Framework.Test]
        public virtual void UnknownHashAlgorithmTest() {
            // PdfPKCS7 is created here the same way it's done in PdfSigner#signDetached,
            // only the hash algorithm is altered
            String hashAlgorithm = "";
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfPKCS7(null, chain, hashAlgorithm
                , false));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(SignExceptionMessageConstant.UNKNOWN_HASH_ALGORITHM
                , hashAlgorithm), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleCreationTest() {
            String hashAlgorithm = DigestAlgorithms.SHA256;
            PdfPKCS7 pkcs7 = new PdfPKCS7(null, chain, hashAlgorithm, false);
            String expectedOid = DigestAlgorithms.GetAllowedDigest(hashAlgorithm);
            NUnit.Framework.Assert.AreEqual(expectedOid, pkcs7.GetDigestAlgorithmOid());
            NUnit.Framework.Assert.AreEqual(chain[0], pkcs7.GetSigningCertificate());
            NUnit.Framework.Assert.AreEqual(chain, pkcs7.GetCertificates());
            NUnit.Framework.Assert.IsNull(pkcs7.GetDigestEncryptionAlgorithmOid());
            // test default fields
            NUnit.Framework.Assert.AreEqual(1, pkcs7.GetVersion());
            NUnit.Framework.Assert.AreEqual(1, pkcs7.GetSigningInfoVersion());
        }

        [NUnit.Framework.Test]
        public virtual void SimpleCreationWithPrivateKeyTest() {
            String hashAlgorithm = DigestAlgorithms.SHA256;
            PdfPKCS7 pkcs7 = new PdfPKCS7(pk, chain, hashAlgorithm, false);
            String expectedOid = DigestAlgorithms.GetAllowedDigest(hashAlgorithm);
            NUnit.Framework.Assert.AreEqual(expectedOid, pkcs7.GetDigestAlgorithmOid());
            NUnit.Framework.Assert.AreEqual(chain[0], pkcs7.GetSigningCertificate());
            NUnit.Framework.Assert.AreEqual(chain, pkcs7.GetCertificates());
            NUnit.Framework.Assert.AreEqual(SecurityIDs.ID_RSA, pkcs7.GetDigestEncryptionAlgorithmOid());
        }

        [NUnit.Framework.Test]
        public virtual void ReasonSetGetTest() {
            PdfPKCS7 pkcs7 = CreateSimplePdfPKCS7();
            NUnit.Framework.Assert.IsNull(pkcs7.GetReason());
            String testReason = "testReason";
            pkcs7.SetReason(testReason);
            NUnit.Framework.Assert.AreEqual(testReason, pkcs7.GetReason());
        }

        [NUnit.Framework.Test]
        public virtual void LocationSetGetTest() {
            PdfPKCS7 pkcs7 = CreateSimplePdfPKCS7();
            NUnit.Framework.Assert.IsNull(pkcs7.GetLocation());
            String testLocation = "testLocation";
            pkcs7.SetLocation(testLocation);
            NUnit.Framework.Assert.AreEqual(testLocation, pkcs7.GetLocation());
        }

        [NUnit.Framework.Test]
        public virtual void SignNameSetGetTest() {
            PdfPKCS7 pkcs7 = CreateSimplePdfPKCS7();
            NUnit.Framework.Assert.IsNull(pkcs7.GetSignName());
            String testSignName = "testSignName";
            pkcs7.SetSignName(testSignName);
            NUnit.Framework.Assert.AreEqual(testSignName, pkcs7.GetSignName());
        }

        [NUnit.Framework.Test]
        public virtual void SignDateSetGetTest() {
            PdfPKCS7 pkcs7 = CreateSimplePdfPKCS7();
            NUnit.Framework.Assert.AreEqual(TimestampConstants.UNDEFINED_TIMESTAMP_DATE, pkcs7.GetSignDate());
            DateTime testSignDate = DateTimeUtil.GetCurrentTime();
            pkcs7.SetSignDate(testSignDate);
            NUnit.Framework.Assert.AreEqual(testSignDate, pkcs7.GetSignDate());
        }

        [NUnit.Framework.Test]
        public virtual void OcspGetTest() {
            PdfDocument outDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "ltvEnabledSingleSignatureTest01.pdf"
                ));
            SignatureUtil sigUtil = new SignatureUtil(outDocument);
            PdfPKCS7 pkcs7 = sigUtil.ReadSignatureData("Signature1");
            NUnit.Framework.Assert.IsNull(pkcs7.GetCRLs());
            // it's tested here that ocsp and time stamp token were found while
            // constructing PdfPKCS7 instance
            TimeStampToken timeStampToken = pkcs7.GetTimeStampToken();
            NUnit.Framework.Assert.IsNotNull(timeStampToken);
            // The number corresponds to 3 September, 2021 13:32:33.
            double expectedMillis = (double)1630675953000L;
            NUnit.Framework.Assert.AreEqual(TimeTestUtil.GetFullDaysMillis(expectedMillis), TimeTestUtil.GetFullDaysMillis
                (DateTimeUtil.GetUtcMillisFromEpoch(DateTimeUtil.GetCalendar(timeStampToken.TimeStampInfo.GenTime))), 
                EPS);
            NUnit.Framework.Assert.AreEqual(TimeTestUtil.GetFullDaysMillis(expectedMillis), TimeTestUtil.GetFullDaysMillis
                (DateTimeUtil.GetUtcMillisFromEpoch(DateTimeUtil.GetCalendar(pkcs7.GetOcsp().ProducedAt))), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void VerifyTimestampImprintSimpleSignatureTest() {
            PdfDocument outDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "simpleSignature.pdf"));
            PdfPKCS7 pkcs7 = new SignatureUtil(outDocument).ReadSignatureData("Signature1");
            NUnit.Framework.Assert.IsFalse(pkcs7.VerifyTimestampImprint());
        }

        [NUnit.Framework.Test]
        public virtual void VerifyTimestampImprintTimeStampSignatureTest() {
            PdfDocument outDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "timeStampSignature.pdf"));
            PdfPKCS7 pkcs7 = new SignatureUtil(outDocument).ReadSignatureData("timestampSig1");
            NUnit.Framework.Assert.IsFalse(pkcs7.VerifyTimestampImprint());
        }

        [NUnit.Framework.Test]
        public virtual void VerifyTimestampImprintEmbeddedTimeStampSignatureTest() {
            PdfDocument outDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "embeddedTimeStampSignature.pdf"));
            PdfPKCS7 pkcs7 = new SignatureUtil(outDocument).ReadSignatureData("Signature1");
            NUnit.Framework.Assert.IsTrue(pkcs7.VerifyTimestampImprint());
        }

        [NUnit.Framework.Test]
        public virtual void VerifyTimestampImprintCorruptedTimeStampSignatureTest() {
            PdfDocument outDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "embeddedTimeStampCorruptedSignature.pdf"
                ));
            PdfPKCS7 pkcs7 = new SignatureUtil(outDocument).ReadSignatureData("Signature1");
            NUnit.Framework.Assert.IsTrue(pkcs7.VerifyTimestampImprint());
        }

        // PdfPKCS7 is created here the same way it's done in PdfSigner#signDetached
        private static PdfPKCS7 CreateSimplePdfPKCS7() {
            return new PdfPKCS7(null, chain, DigestAlgorithms.SHA256, false);
        }
    }
}
