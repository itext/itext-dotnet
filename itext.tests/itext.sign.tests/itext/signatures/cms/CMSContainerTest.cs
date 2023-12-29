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
using System.Collections.Generic;
using System.Linq;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
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
    public class CMSContainerTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private static readonly byte[] EXPECTEDRESULT_1 = Convert.FromBase64String(CMSTestHelper.EXPECTED_RESULT_CMS_CONTAINER_TEST
            );

        private IX509Certificate[] chain;

        private IX509Certificate signCert;

        private byte[] testCrlResponse;

        [NUnit.Framework.SetUp]
        public virtual void Init() {
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS_SRC + "signCertRsaWithChain.pem");
            chain = new IX509Certificate[certChain.Length];
            for (int i = 0; i < certChain.Length; i++) {
                chain[i] = (IX509Certificate)certChain[i];
            }
            signCert = chain[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(CERTS_SRC + "signCertRsaWithChain.pem", PASSWORD);
            TestCrlBuilder testCrlBuilder = new TestCrlBuilder(signCert, caPrivateKey);
            testCrlBuilder.AddCrlEntry(signCert, FACTORY.CreateCRLReason().GetKeyCompromise());
            testCrlResponse = testCrlBuilder.MakeCrl();
        }

        [NUnit.Framework.Test]
        public virtual void TestSerialize() {
            CMSContainer sut = new CMSContainer();
            sut.AddCertificates((IX509Certificate[])chain);
            SignerInfo si = new SignerInfo();
            si.SetSigningCertificate(signCert);
            List<byte[]> fakeOcspREsponses = new List<byte[]>();
            fakeOcspREsponses.Add(new byte[250]);
            si.SetMessageDigest(new byte[256]);
            si.SetOcspResponses(fakeOcspREsponses);
            si.SetCrlResponses(JavaCollectionsUtil.SingletonList(testCrlResponse));
            si.SetDigestAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_SHA512));
            si.SetSigningCertificateAndAddToSignedAttributes(signCert, SecurityIDs.ID_SHA512);
            si.SetSignature(new byte[256]);
            sut.SetSignerInfo(si);
            byte[] serRes = sut.Serialize();
            NUnit.Framework.Assert.AreEqual(SerializedAsString(EXPECTEDRESULT_1), SerializedAsString(serRes));
        }

        [NUnit.Framework.Test]
        public virtual void TestGetSizeEstimation() {
            CMSContainer sut = new CMSContainer();
            sut.AddCertificates((IX509Certificate[])chain);
            SignerInfo si = new SignerInfo();
            si.SetSigningCertificate(signCert);
            List<byte[]> fakeOcspREsponses = new List<byte[]>();
            fakeOcspREsponses.Add(new byte[250]);
            si.SetMessageDigest(new byte[256]);
            si.SetOcspResponses(fakeOcspREsponses);
            si.SetCrlResponses(JavaCollectionsUtil.SingletonList(testCrlResponse));
            si.SetDigestAlgorithm(new AlgorithmIdentifier(SecurityIDs.ID_SHA512));
            si.SetSigningCertificateAndAddToSignedAttributes(signCert, SecurityIDs.ID_SHA512);
            si.SetSignature(new byte[256]);
            sut.SetSignerInfo(si);
            long size = sut.GetSizeEstimation();
            NUnit.Framework.Assert.AreEqual(4825, size);
        }

        [NUnit.Framework.Test]
        public virtual void TestDeserialisation() {
            byte[] rawData = Convert.FromBase64String(CMSTestHelper.SERIALIZED_B64_CASE1);
            CMSContainer sd = new CMSContainer(rawData);
            NUnit.Framework.Assert.AreEqual("2.16.840.1.101.3.4.2.1", sd.GetDigestAlgorithm().GetAlgorithmOid());
            NUnit.Framework.Assert.AreEqual("1.2.840.113549.1.7.1", sd.GetEncapContentInfo().GetContentType());
            NUnit.Framework.Assert.AreEqual(3, sd.GetCertificates().Count);
            NUnit.Framework.Assert.IsTrue(sd.GetCertificates().Any((c) => "140282000747862710817410059465802198354".Equals
                (c.GetSerialNumber().ToString())));
            NUnit.Framework.Assert.IsTrue(sd.GetCertificates().Any((c) => "151118660848720701053205649823964411794".Equals
                (c.GetSerialNumber().ToString())));
            NUnit.Framework.Assert.IsTrue(sd.GetCertificates().Any((c) => "8380897714609953925".Equals(c.GetSerialNumber
                ().ToString())));
            NUnit.Framework.Assert.AreEqual("8380897714609953925", sd.GetSignerInfo().GetSigningCertificate().GetSerialNumber
                ().ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TestDeserialisationWithRevocationData() {
            byte[] rawData = Convert.FromBase64String(CMSTestHelper.SERIALIZED_B64_CASE2);
            CMSContainer sd = new CMSContainer(rawData);
            NUnit.Framework.Assert.AreEqual("2.16.840.1.101.3.4.2.1", sd.GetDigestAlgorithm().GetAlgorithmOid());
            NUnit.Framework.Assert.AreEqual("1.2.840.113549.1.7.1", sd.GetEncapContentInfo().GetContentType());
            NUnit.Framework.Assert.AreEqual(3, sd.GetCertificates().Count);
            NUnit.Framework.Assert.IsTrue(sd.GetCertificates().Any((c) => "3081".Equals(c.GetSerialNumber().ToString()
                )));
            NUnit.Framework.Assert.IsTrue(sd.GetCertificates().Any((c) => "2776".Equals(c.GetSerialNumber().ToString()
                )));
            NUnit.Framework.Assert.IsTrue(sd.GetCertificates().Any((c) => "1".Equals(c.GetSerialNumber().ToString())));
            NUnit.Framework.Assert.AreEqual("3081", sd.GetSignerInfo().GetSigningCertificate().GetSerialNumber().ToString
                ());
        }

        [NUnit.Framework.Test]
        public virtual void TestMultipleDigestAlgorithms() {
            byte[] rawData = Convert.FromBase64String(CMSTestHelper.SERIALIZED_B64_2DIGEST_ALGOS);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                CMSContainer sd = new CMSContainer(rawData);
            }
            );
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.CMS_ONLY_ONE_SIGNER_ALLOWED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestMultipleSignerInfos() {
            byte[] rawData = Convert.FromBase64String(CMSTestHelper.SERIALIZED_B64_2SIGNERS);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                CMSContainer sd = new CMSContainer(rawData);
            }
            );
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.CMS_ONLY_ONE_SIGNER_ALLOWED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestCertificatesMissing() {
            byte[] rawData = Convert.FromBase64String(CMSTestHelper.SERIALIZED_B64_MISSING_CERTIFICATES);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                CMSContainer sd = new CMSContainer(rawData);
            }
            );
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.CMS_MISSING_CERTIFICATES, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestCertificatesEmpty() {
            byte[] rawData = Convert.FromBase64String(CMSTestHelper.SERIALIZED_B64_EMPTY_CERTIFICATES);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                CMSContainer sd = new CMSContainer(rawData);
            }
            );
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.CMS_MISSING_CERTIFICATES, e.Message);
        }

        private String ToUnixStringEnding(String @in) {
            return @in.Replace("\r\n", "\n");
        }

        private String SerializedAsString(byte[] serialized) {
            IAsn1InputStream @is = FACTORY.CreateASN1InputStream(serialized);
            IAsn1Object obj1 = @is.ReadObject();
            return ToUnixStringEnding(FACTORY.CreateASN1Dump().DumpAsString(obj1, true));
        }
    }
}
