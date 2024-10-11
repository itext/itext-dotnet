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
using System.Linq;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Kernel.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Signatures.Exceptions;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;

namespace iText.Signatures {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class PdfPKCS7Test : PdfPKCS7BasicTest {
        private const double EPS = 0.001;

        [NUnit.Framework.Test]
        public virtual void UnknownHashAlgorithmTest() {
            // PdfPKCS7 is created here the same way it's done in PdfSigner#signDetached,
            // only the hash algorithm is altered
            String hashAlgorithm = "";
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfPKCS7(null, chain, hashAlgorithm
                , new BouncyCastleDigest(), false));
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
            NUnit.Framework.Assert.IsNull(pkcs7.GetSignatureMechanismOid());
            // test default fields
            NUnit.Framework.Assert.AreEqual(1, pkcs7.GetVersion());
            NUnit.Framework.Assert.AreEqual(1, pkcs7.GetSigningInfoVersion());
        }

        [NUnit.Framework.Test]
        public virtual void SimpleCreationWithPrivateKeyTest() {
            String hashAlgorithm = DigestAlgorithms.SHA256;
            PdfPKCS7 pkcs7 = new PdfPKCS7(pk, chain, hashAlgorithm, new BouncyCastleDigest(), false);
            String expectedOid = DigestAlgorithms.GetAllowedDigest(hashAlgorithm);
            NUnit.Framework.Assert.AreEqual(expectedOid, pkcs7.GetDigestAlgorithmOid());
            NUnit.Framework.Assert.AreEqual(chain[0], pkcs7.GetSigningCertificate());
            NUnit.Framework.Assert.AreEqual(chain, pkcs7.GetCertificates());
            NUnit.Framework.Assert.AreEqual(OID.RSA_WITH_SHA256, pkcs7.GetSignatureMechanismOid());
        }

        [NUnit.Framework.Test]
        public virtual void NotAvailableSignatureTest() {
            String hashAlgorithm = "GOST3411";
            // Throws different exceptions on .net and java, bc/bcfips
            NUnit.Framework.Assert.Catch(typeof(Exception), () => new PdfPKCS7(pk, chain, hashAlgorithm, new BouncyCastleDigest
                (), false));
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
            ITstInfo timeStampTokenInfo = pkcs7.GetTimeStampTokenInfo();
            NUnit.Framework.Assert.IsNotNull(timeStampTokenInfo);
            // The number corresponds to 3 September, 2021 13:32:33.
            double expectedMillis = (double)1630675953000L;
            NUnit.Framework.Assert.AreEqual(TimeTestUtil.GetFullDaysMillis(expectedMillis), TimeTestUtil.GetFullDaysMillis
                (DateTimeUtil.GetUtcMillisFromEpoch(DateTimeUtil.GetCalendar(timeStampTokenInfo.GetGenTime()))), EPS);
            NUnit.Framework.Assert.AreEqual(TimeTestUtil.GetFullDaysMillis(expectedMillis), TimeTestUtil.GetFullDaysMillis
                (DateTimeUtil.GetUtcMillisFromEpoch(DateTimeUtil.GetCalendar(pkcs7.GetOcsp().GetProducedAtDate()))), EPS
                );
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
            IList<IX509Crl> crls = pkcs7.GetCRLs().Select((crl) => (IX509Crl)crl).ToList();
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
            pkcs7.basicResp = BOUNCY_CASTLE_FACTORY.CreateBasicOCSPResponse(BOUNCY_CASTLE_FACTORY.CreateASN1InputStream
                (File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER, "simpleOCSPResponse.bin"))).ReadObject());
            pkcs7.signCerts = JavaCollectionsUtil.Singleton(chain[0]);
            NUnit.Framework.Assert.IsFalse(pkcs7.IsRevocationValid());
        }

        [NUnit.Framework.Test]
        public virtual void IsRevocationValidExceptionDuringValidationTest() {
            PdfPKCS7 pkcs7 = CreateSimplePdfPKCS7();
            pkcs7.basicResp = BOUNCY_CASTLE_FACTORY.CreateBasicOCSPResponse(BOUNCY_CASTLE_FACTORY.CreateASN1InputStream
                (File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER, "simpleOCSPResponse.bin"))).ReadObject());
            pkcs7.signCerts = JavaUtil.ArraysAsList(new IX509Certificate[] { null, null });
            NUnit.Framework.Assert.IsFalse(pkcs7.IsRevocationValid());
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedPkcs1Test() {
            String hashAlgorithm = DigestAlgorithms.SHA256;
            PdfPKCS7 pkcs7 = new PdfPKCS7(pk, chain, hashAlgorithm, new BouncyCastleDigest(), true);
            byte[] bytes = pkcs7.GetEncodedPKCS1();
            byte[] cmpBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "cmpBytesPkcs1.txt"));
            IAsn1OctetString outOctetString = BOUNCY_CASTLE_FACTORY.CreateASN1OctetString(bytes);
            IAsn1OctetString cmpOctetString = BOUNCY_CASTLE_FACTORY.CreateASN1OctetString(cmpBytes);
            NUnit.Framework.Assert.AreEqual(outOctetString, cmpOctetString);
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedPkcs1NullPrivateKeyTest() {
            String hashAlgorithm = DigestAlgorithms.SHA256;
            PdfPKCS7 pkcs7 = new PdfPKCS7(null, chain, hashAlgorithm, new BouncyCastleDigest(), true);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pkcs7.GetEncodedPKCS1());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.UNKNOWN_PDF_EXCEPTION, exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedPkcs7UnknownExceptionTest() {
            String hashAlgorithm = DigestAlgorithms.SHA256;
            PdfPKCS7 pkcs7 = new PdfPKCS7(pk, chain, hashAlgorithm, new BouncyCastleDigest(), true);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(chain), pk);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pkcs7.GetEncodedPKCS7(null, 
                PdfSigner.CryptoStandard.CMS, testTsa, null, null));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.UNKNOWN_PDF_EXCEPTION, exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedPkcs7Test() {
            String hashAlgorithm = DigestAlgorithms.SHA256;
            PdfPKCS7 pkcs7 = new PdfPKCS7(pk, chain, hashAlgorithm, new BouncyCastleDigest(), true);
            byte[] bytes = pkcs7.GetEncodedPKCS7();
            byte[] cmpBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "cmpBytesPkcs7.txt"));
            IAsn1Object outStream = BOUNCY_CASTLE_FACTORY.CreateASN1Primitive(bytes);
            IAsn1Object cmpStream = BOUNCY_CASTLE_FACTORY.CreateASN1Primitive(cmpBytes);
            NUnit.Framework.Assert.AreEqual("SHA256withRSA", pkcs7.GetSignatureMechanismName());
            NUnit.Framework.Assert.AreEqual(outStream, cmpStream);
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedPkcs7WithRevocationInfoTest() {
            String hashAlgorithm = DigestAlgorithms.SHA256;
            PdfPKCS7 pkcs7 = new PdfPKCS7(pk, chain, hashAlgorithm, new BouncyCastleDigest(), true);
            pkcs7.GetSignedDataCRLs().Add(SignTestPortUtil.ParseCrlFromStream(FileUtil.GetInputStreamForFile(SOURCE_FOLDER
                 + "firstCrl.bin")));
            pkcs7.GetSignedDataOcsps().Add(BOUNCY_CASTLE_FACTORY.CreateBasicOCSPResponse(BOUNCY_CASTLE_FACTORY.CreateASN1InputStream
                (File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER, "simpleOCSPResponse.bin"))).ReadObject()));
            byte[] bytes = pkcs7.GetEncodedPKCS7();
            byte[] cmpBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "cmpBytesPkcs7WithRevInfo.txt")
                );
            NUnit.Framework.Assert.AreEqual("SHA256withRSA", pkcs7.GetSignatureMechanismName());
            NUnit.Framework.Assert.AreEqual(SerializedAsString(bytes), SerializedAsString(cmpBytes));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyNistECDSASha2SignatureTest() {
            VerifyIsoExtensionExample("SHA256withECDSA", "sample-nistp256-sha256.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void VerifyBrainpoolSha2SignatureTest() {
            VerifyIsoExtensionExample("SHA384withECDSA", "sample-brainpoolP384r1-sha384.pdf");
        }

        // PdfPKCS7 is created here the same way it's done in PdfSigner#signDetached
        private static PdfPKCS7 CreateSimplePdfPKCS7() {
            return new PdfPKCS7(null, chain, DigestAlgorithms.SHA256, new BouncyCastleDigest(), false);
        }

        private String SerializedAsString(byte[] serialized) {
            IAsn1InputStream @is = BOUNCY_CASTLE_FACTORY.CreateASN1InputStream(serialized);
            IAsn1Object obj1 = @is.ReadObject();
            return BOUNCY_CASTLE_FACTORY.CreateASN1Dump().DumpAsString(obj1, true).Replace("\r\n", "\n");
        }
    }
}
