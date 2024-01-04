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
using System.Linq;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Util;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Signatures;
using iText.Signatures.Exceptions;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Test;

namespace iText.Signatures.Cms {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class SignerInfoTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private static readonly IAsn1Dump DUMP = FACTORY.CreateASN1Dump();

        private static readonly byte[] MESSAGE_DIGEST = CMSTestHelper.MESSAGE_DIGEST_STRING.GetBytes(System.Text.Encoding
            .UTF8);

        private static readonly byte[] EXPECTEDRESULT_1 = Convert.FromBase64String(CMSTestHelper.EXPECTEDRESULT_1);

        private static readonly byte[] EXPECTEDRESULT_2 = Convert.FromBase64String(CMSTestHelper.EXPECTEDRESULT_2);

        private static readonly byte[] EXPECTEDRESULT_3 = Convert.FromBase64String(CMSTestHelper.EXPECTEDRESULT_3);

        private static readonly byte[] EXPECTEDRESULT_4 = Convert.FromBase64String(CMSTestHelper.EXPECTEDRESULT_4);

        private static readonly byte[] EXPECTEDRESULT_5 = Convert.FromBase64String(CMSTestHelper.EXPECTEDRESULT_5);

        private static readonly IList<IX509Certificate> chain = new List<IX509Certificate>();

        static SignerInfoTest() {
            IX509Certificate[] certChain = new IX509Certificate[0];
            try {
                certChain = PemFileHelper.ReadFirstChain(CERTS_SRC + "signCertRsaWithChain.pem");
            }
            catch (Exception) {
            }
            // Ignore.
            foreach (IX509Certificate cert in certChain) {
                chain.Add((IX509Certificate)cert);
            }
        }

        private IX509Certificate signCert;

        private IList<byte[]> testCrlResponse;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.SetUp]
        public virtual void Init() {
            signCert = chain[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(CERTS_SRC + "signCertRsaWithChain.pem", PASSWORD);
            TestCrlBuilder testCrlBuilder = new TestCrlBuilder(signCert, caPrivateKey);
            testCrlBuilder.AddCrlEntry(signCert, FACTORY.CreateCRLReason().GetKeyCompromise());
            testCrlResponse = JavaCollectionsUtil.SingletonList(testCrlBuilder.MakeCrl());
        }

        [NUnit.Framework.Test]
        public virtual void TestSignedAttributesReadonlyModeActivatedByGettingSerializedData() {
            SignerInfo si = new SignerInfo();
            si.SetSignatureAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_RSA));
            si.SetSigningCertificate(signCert);
            List<byte[]> fakeOcspREsponses = new List<byte[]>();
            fakeOcspREsponses.Add(Convert.FromBase64String(CMSTestHelper.BASE64_OCSP_RESPONSE));
            si.SetMessageDigest(new byte[1024]);
            si.SetOcspResponses(fakeOcspREsponses);
            si.SetCrlResponses(testCrlResponse);
            si.SetDigestAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_SHA512));
            si.SerializeSignedAttributes();
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => si.SetSerializedSignedAttributes(new 
                byte[1235]));
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => si.SetCrlResponses(testCrlResponse));
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => si.SetOcspResponses(fakeOcspREsponses
                ));
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => si.SetMessageDigest(new byte[1024]));
            CmsAttribute attribute = new CmsAttribute("", FACTORY.CreateASN1Integer(1));
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => si.AddSignedAttribute(attribute));
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => si.AddSignerCertificateToSignedAttributes
                (signCert, SecurityIDs.ID_SHA512));
        }

        [NUnit.Framework.Test]
        public virtual void TestGetSerializedBasicSignedAttributes() {
            SignerInfo si = new SignerInfo();
            si.SetSignatureAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_RSASSA_PSS));
            si.SetSigningCertificate(signCert);
            si.SetMessageDigest(MESSAGE_DIGEST);
            si.SetDigestAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_SHA512));
            byte[] serRes = si.SerializeSignedAttributes();
            NUnit.Framework.Assert.AreEqual(SerializedAsString(EXPECTEDRESULT_1), SerializedAsString(serRes));
        }

        [NUnit.Framework.Test]
        public virtual void TestGetSerializedExtendedSignedAttributes() {
            SignerInfo si = new SignerInfo();
            si.SetSignatureAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_RSASSA_PSS));
            si.SetSigningCertificate(signCert);
            List<byte[]> fakeOcspREsponses = new List<byte[]>();
            fakeOcspREsponses.Add(Convert.FromBase64String(CMSTestHelper.BASE64_OCSP_RESPONSE));
            si.SetOcspResponses(fakeOcspREsponses);
            si.SetCrlResponses(testCrlResponse);
            si.SetMessageDigest(new byte[1024]);
            si.SetDigestAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_SHA512));
            byte[] serRes = si.SerializeSignedAttributes();
            NUnit.Framework.Assert.AreEqual(SerializedAsString(EXPECTEDRESULT_2), SerializedAsString(serRes));
        }

        [NUnit.Framework.Test]
        public virtual void TestGetSerializedExtendedSignedAttributesCrlOnly() {
            SignerInfo si = new SignerInfo();
            si.SetSignatureAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_RSASSA_PSS));
            si.SetSigningCertificate(signCert);
            si.SetCrlResponses(testCrlResponse);
            si.SetMessageDigest(new byte[1024]);
            si.SetDigestAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_SHA512));
            byte[] serRes = si.SerializeSignedAttributes();
            NUnit.Framework.Assert.AreEqual(SerializedAsString(EXPECTEDRESULT_5), SerializedAsString(serRes));
        }

        [NUnit.Framework.Test]
        public virtual void TestAddSignedAttribute() {
            SignerInfo si = new SignerInfo();
            NUnit.Framework.Assert.IsFalse(si.GetSignedAttributes().Any((a) => Object.Equals(a.GetType(), SecurityIDs.
                ID_SIGNING_TIME)));
            CmsAttribute attrib = new CmsAttribute(SecurityIDs.ID_SIGNING_TIME, FACTORY.CreateNullASN1Set());
            si.AddSignedAttribute(attrib);
            NUnit.Framework.Assert.IsTrue(si.GetSignedAttributes().Any((a) => Object.Equals(a.GetType(), SecurityIDs.ID_SIGNING_TIME
                )));
        }

        [NUnit.Framework.Test]
        public virtual void TestAddUnsignedAttribute() {
            SignerInfo si = new SignerInfo();
            CmsAttribute attrib = new CmsAttribute(SecurityIDs.ID_SIGNING_TIME, FACTORY.CreateNullASN1Set());
            si.AddUnSignedAttribute(attrib);
            NUnit.Framework.Assert.AreEqual(SecurityIDs.ID_SIGNING_TIME, SignTestPortUtil.GetFirstElement<CmsAttribute
                >(si.GetUnSignedAttributes()).GetType());
        }

        [NUnit.Framework.Test]
        public virtual void TestGetSerializedSignedAttributesWithCertificateId() {
            SignerInfo si = new SignerInfo();
            si.SetSignatureAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_RSASSA_PSS));
            si.SetSigningCertificate(signCert);
            si.SetMessageDigest(new byte[1024]);
            si.SetDigestAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_SHA512));
            si.AddSignerCertificateToSignedAttributes(signCert, "2.16.840.1.101.3.4.2.3");
            byte[] serRes = si.SerializeSignedAttributes();
            NUnit.Framework.Assert.AreEqual(SerializedAsString(EXPECTEDRESULT_3), SerializedAsString(serRes));
        }

        [NUnit.Framework.Test]
        public virtual void TestGetSerializedSignedAttributesWithCertificateIdTroughCertSetter() {
            SignerInfo si = new SignerInfo();
            si.SetSignatureAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_RSASSA_PSS));
            si.SetSigningCertificateAndAddToSignedAttributes(signCert, "2.16.840.1.101.3.4.2.3");
            si.SetMessageDigest(new byte[1024]);
            si.SetDigestAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_SHA512));
            byte[] serRes = si.SerializeSignedAttributes();
            NUnit.Framework.Assert.AreEqual(SerializedAsString(EXPECTEDRESULT_3), SerializedAsString(serRes));
        }

        [NUnit.Framework.Test]
        public virtual void TestGetAsDerSequence() {
            SignerInfo si = new SignerInfo();
            si.AddUnSignedAttribute(new CmsAttribute(SecurityIDs.ID_SIGNING_TIME, FACTORY.CreateDERSet(FACTORY.CreateASN1Integer
                (123456))));
            si.SetSignatureAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_RSASSA_PSS));
            si.SetSigningCertificateAndAddToSignedAttributes(signCert, "2.16.840.1.101.3.4.2.3");
            si.SetMessageDigest(new byte[1024]);
            si.SetDigestAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_SHA512));
            si.SetSignature(new byte[512]);
            IDerSequence res = si.GetAsDerSequence();
            NUnit.Framework.Assert.AreEqual(SerializedAsString(EXPECTEDRESULT_4), SerializedAsString(res.GetEncoded())
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestEstimatedSizeWithSignature() {
            SignerInfo si = new SignerInfo();
            si.SetSignatureAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_RSA_WITH_SHA256));
            si.AddUnSignedAttribute(new CmsAttribute(SecurityIDs.ID_SIGNING_TIME, FACTORY.CreateDERSet(FACTORY.CreateASN1Integer
                (123456))));
            si.SetSignatureAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_RSASSA_PSS));
            si.SetSigningCertificateAndAddToSignedAttributes(signCert, "2.16.840.1.101.3.4.2.3");
            si.SetMessageDigest(new byte[1024]);
            si.SetDigestAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_SHA512));
            si.SetSignature(new byte[512]);
            long res = si.GetEstimatedSize();
            NUnit.Framework.Assert.AreEqual(1977, res);
        }

        [NUnit.Framework.Test]
        public virtual void TestSignedAttributesSerializationRoundTrip() {
            SignerInfo si = new SignerInfo();
            si.SetSignatureAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_RSASSA_PSS));
            si.SetSigningCertificateAndAddToSignedAttributes(signCert, "2.16.840.1.101.3.4.2.3");
            si.SetMessageDigest(new byte[1024]);
            si.SetDigestAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_SHA512));
            si.SetSignature(new byte[512]);
            byte[] serialized = si.SerializeSignedAttributes();
            SignerInfo si2 = new SignerInfo();
            si2.SetSerializedSignedAttributes(serialized);
            NUnit.Framework.Assert.AreEqual(si.GetSignedAttributes().Count, si2.GetSignedAttributes().Count);
            foreach (CmsAttribute attribute in si.GetSignedAttributes()) {
                NUnit.Framework.Assert.IsTrue(si2.GetSignedAttributes().Any((a) => a.GetType().Equals(attribute.GetType())
                     && a.GetValue().Equals(attribute.GetValue())), MessageFormatUtil.Format("Expected to find an attribute with id {0} and value {1}"
                    , attribute.GetType(), attribute.GetValue().ToString()));
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestEstimatedSizeEstimatedSignature() {
            SignerInfo si = new SignerInfo();
            si.SetSignatureAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_RSA_WITH_SHA256));
            si.AddUnSignedAttribute(new CmsAttribute(SecurityIDs.ID_SIGNING_TIME, FACTORY.CreateDERSet(FACTORY.CreateASN1Integer
                (123456))));
            si.SetSignatureAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_RSASSA_PSS));
            si.SetSigningCertificateAndAddToSignedAttributes(signCert, "2.16.840.1.101.3.4.2.3");
            si.SetMessageDigest(new byte[1024]);
            si.SetDigestAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_SHA512));
            long res = si.GetEstimatedSize();
            NUnit.Framework.Assert.AreEqual(2489, res);
        }

        [NUnit.Framework.Test]
        public virtual void TestSerializeAndDeserializeSignerInfo() {
            SignerInfo si = new SignerInfo();
            si.SetSignatureAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_RSA_WITH_SHA256));
            si.AddUnSignedAttribute(new CmsAttribute(SecurityIDs.ID_SIGNING_TIME, FACTORY.CreateDERSet(FACTORY.CreateASN1Integer
                (123456))));
            si.SetSignatureAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_RSASSA_PSS));
            si.SetSigningCertificateAndAddToSignedAttributes(signCert, "2.16.840.1.101.3.4.2.3");
            si.SetMessageDigest(new byte[1024]);
            si.SetDigestAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_SHA512));
            IDerSequence encoded = si.GetAsDerSequence(false);
            SignerInfo si2 = new SignerInfo(encoded, JavaCollectionsUtil.SingletonList(signCert));
            NUnit.Framework.Assert.AreEqual(si.GetSignedAttributes().Count, si2.GetSignedAttributes().Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestSerializeAndDeserializeSignedAttributes() {
            SignerInfo si = new SignerInfo();
            si.AddSignerCertificateToSignedAttributes(signCert, SecurityIDs.ID_SHA256);
            si.SetMessageDigest(new byte[20]);
            byte[] attribs = si.SerializeSignedAttributes();
            SignerInfo si2 = new SignerInfo();
            si2.SetSerializedSignedAttributes(attribs);
            NUnit.Framework.Assert.AreEqual(si.GetSignedAttributes().Count, si2.GetSignedAttributes().Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestDeserializationMissingSignedAttributes() {
            IAsn1Encodable testData = FACTORY.CreateASN1Primitive(Convert.FromBase64String(CMSTestHelper.B64_ENCODED_NO_SIGNED_ATTRIBS
                ));
            SignerInfo si = new SignerInfo(testData, chain);
            NUnit.Framework.Assert.AreEqual(0, si.GetSignedAttributes().Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestMissingSignerCertificate() {
            IAsn1Encodable testData = FACTORY.CreateASN1Primitive(Convert.FromBase64String(CMSTestHelper.B64_ENCODED_NO_SIGNED_ATTRIBS
                ));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new SignerInfo(testData, chain.SubList
                (1, chain.Count - 1)));
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.CMS_CERTIFICATE_NOT_FOUND, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestSidWithSubjectKeyIdentifier() {
            IAsn1Encodable testData = FACTORY.CreateASN1Primitive(Convert.FromBase64String(CMSTestHelper.B64_ENCODED_SUBJECTKEY_IDENTIFIER
                ));
            SignerInfo si = new SignerInfo(testData, chain);
            NUnit.Framework.Assert.AreEqual(signCert.GetSerialNumber(), si.GetSigningCertificate().GetSerialNumber());
        }

        [NUnit.Framework.Test]
        public virtual void TestMissingCertificateWithSubjectKeyIdentifier() {
            IAsn1Encodable testData = FACTORY.CreateASN1Primitive(Convert.FromBase64String(CMSTestHelper.B64_ENCODED_SUBJECTKEY_IDENTIFIER
                ));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new SignerInfo(testData, chain.SubList
                (1, chain.Count - 1)));
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.CMS_CERTIFICATE_NOT_FOUND, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestInvalidStructure() {
            IAsn1EncodableVector v = FACTORY.CreateASN1EncodableVector();
            v.Add(FACTORY.CreateASN1ObjectIdentifier("1.2.840.113549.1.7.2"));
            //should be tagged with 0
            v.Add(FACTORY.CreateDERSequence(FACTORY.CreateASN1EncodableVector()));
            IAsn1Encodable testData = FACTORY.CreateASN1Sequence(v);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new SignerInfo(testData, chain.SubList
                (1, chain.Count - 1)));
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.CMS_INVALID_CONTAINER_STRUCTURE, e.Message);
        }

        private String ToUnixStringEnding(String @in) {
            return @in.Replace("\r\n", "\n");
        }

        private String SerializedAsString(byte[] serialized) {
            IAsn1InputStream @is = FACTORY.CreateASN1InputStream(serialized);
            IAsn1Object obj1 = @is.ReadObject();
            return ToUnixStringEnding(DUMP.DumpAsString(obj1, true));
        }
    }
}
