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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.X509;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Signatures.Exceptions;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Test;
using iText.Test.Signutils;

namespace iText.Signatures {
    [NUnit.Framework.Category("UnitTest")]
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

        [NUnit.Framework.Test]
        public virtual void FindCrlIsNotNullTest() {
            PdfDocument outDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "singleSignatureNotEmptyCRL.pdf"));
            SignatureUtil sigUtil = new SignatureUtil(outDocument);
            PdfPKCS7 pkcs7 = sigUtil.ReadSignatureData("Signature1");
            IList<X509Crl> crls = pkcs7.GetCRLs().Select((crl) => (X509Crl)crl).ToList();
            NUnit.Framework.Assert.AreEqual(2, crls.Count);
            NUnit.Framework.Assert.AreEqual(crls[0].GetEncoded(), File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER
                , "firstCrl.bin")));
            NUnit.Framework.Assert.AreEqual(crls[1].GetEncoded(), File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER
                , "secondCrl.bin")));
        }

        [NUnit.Framework.Test]
        public virtual void FindCrlNullSequenceNoExceptionTest() {
            PdfPKCS7 pkcs7 = CreateSimplePdfPKCS7();
            pkcs7.FindCRL(null);
            NUnit.Framework.Assert.IsTrue(pkcs7.GetCRLs().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void IsRevocationValidWithInvalidOcspTest() {
            PdfDocument outDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "signatureWithInvalidOcspTest.pdf"
                ));
            SignatureUtil sigUtil = new SignatureUtil(outDocument);
            PdfPKCS7 pkcs7 = sigUtil.ReadSignatureData("Signature1");
            NUnit.Framework.Assert.IsFalse(pkcs7.IsRevocationValid());
        }

        [NUnit.Framework.Test]
        public virtual void IsRevocationValidWithValidOcspTest() {
            PdfDocument outDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "signatureWithValidOcspTest.pdf"));
            SignatureUtil sigUtil = new SignatureUtil(outDocument);
            PdfPKCS7 pkcs7 = sigUtil.ReadSignatureData("Signature1");
            NUnit.Framework.Assert.IsTrue(pkcs7.IsRevocationValid());
        }

        [NUnit.Framework.Test]
        public virtual void IsRevocationValidOcspResponseIsNullTest() {
            PdfPKCS7 pkcs7 = CreateSimplePdfPKCS7();
            pkcs7.basicResp = null;
            NUnit.Framework.Assert.IsFalse(pkcs7.IsRevocationValid());
        }

        [NUnit.Framework.Test]
        public virtual void IsRevocationValidLackOfSignCertsTest() {
            PdfPKCS7 pkcs7 = CreateSimplePdfPKCS7();
            pkcs7.basicResp = new BasicOcspResp(BasicOcspResponse.GetInstance(new Asn1InputStream(File.ReadAllBytes(System.IO.Path.Combine
                (SOURCE_FOLDER, "simpleOCSPResponse.bin"))).ReadObject()));
            pkcs7.signCerts = JavaCollectionsUtil.Singleton(chain[0]);
            NUnit.Framework.Assert.IsFalse(pkcs7.IsRevocationValid());
        }

        [NUnit.Framework.Test]
        public virtual void IsRevocationValidExceptionDuringValidationTest() {
            PdfPKCS7 pkcs7 = CreateSimplePdfPKCS7();
            pkcs7.basicResp = new BasicOcspResp(BasicOcspResponse.GetInstance(new Asn1InputStream(File.ReadAllBytes(System.IO.Path.Combine
                (SOURCE_FOLDER, "simpleOCSPResponse.bin"))).ReadObject()));
            pkcs7.signCerts = JavaUtil.ArraysAsList(new X509Certificate[] { null, null });
            NUnit.Framework.Assert.IsFalse(pkcs7.IsRevocationValid());
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedPkcs1Test() {
            String hashAlgorithm = DigestAlgorithms.SHA256;
            PdfPKCS7 pkcs7 = new PdfPKCS7(pk, chain, hashAlgorithm, true);
            byte[] bytes = pkcs7.GetEncodedPKCS1();
            byte[] cmpBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "cmpBytesPkcs1.txt"));
            Asn1OctetString outOctetString = Asn1OctetString.GetInstance(bytes);
            Asn1OctetString cmpOctetString = Asn1OctetString.GetInstance(cmpBytes);
            NUnit.Framework.Assert.AreEqual(outOctetString, cmpOctetString);
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedPkcs1NullPrivateKeyTest() {
            String hashAlgorithm = DigestAlgorithms.SHA256;
            PdfPKCS7 pkcs7 = new PdfPKCS7(null, chain, hashAlgorithm, true);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pkcs7.GetEncodedPKCS1());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.UNKNOWN_PDF_EXCEPTION, exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedPkcs7UnknownExceptionTest() {
            String hashAlgorithm = DigestAlgorithms.SHA256;
            PdfPKCS7 pkcs7 = new PdfPKCS7(pk, chain, hashAlgorithm, true);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(chain), pk);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pkcs7.GetEncodedPKCS7(null, 
                PdfSigner.CryptoStandard.CMS, testTsa, null, null));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.UNKNOWN_PDF_EXCEPTION, exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedPkcs7Test() {
            String hashAlgorithm = DigestAlgorithms.SHA256;
            PdfPKCS7 pkcs7 = new PdfPKCS7(pk, chain, hashAlgorithm, true);
            byte[] bytes = pkcs7.GetEncodedPKCS7();
            byte[] cmpBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "cmpBytesPkcs7.txt"));
            Asn1Object outStream = Asn1Object.FromByteArray(bytes);
            Asn1Object cmpStream = Asn1Object.FromByteArray(cmpBytes);
            NUnit.Framework.Assert.AreEqual("SHA256withRSA", pkcs7.GetDigestAlgorithm());
            NUnit.Framework.Assert.AreEqual(outStream, cmpStream);
        }

        // PdfPKCS7 is created here the same way it's done in PdfSigner#signDetached
        private static PdfPKCS7 CreateSimplePdfPKCS7() {
            return new PdfPKCS7(null, chain, DigestAlgorithms.SHA256, false);
        }
    }
}
