/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using NUnit.Framework;
using Org.BouncyCastle.Security;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using iText.Forms.Form.Element;
using iText.Kernel.Crypto;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Cms;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    [NUnit.Framework.TestFixtureSource("CreateParametersTestFixtureData")]
    public class PadesTwoPhaseSigningLevelsTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly bool FIPS_MODE = "BCFIPS".Equals(FACTORY.GetProviderName());

        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PadesTwoPhaseSigningLevelsTest/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PadesTwoPhaseSigningLevelsTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PadesTwoPhaseSigningLevelsTest/";

        private readonly bool? useTempFolder;

        private readonly int? comparisonPdfId;

        private readonly String digestAlgorithm;

        private readonly String signAlgorithm;

        private String signCertName;

        private String rootCertName;

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.SetUp]
        public virtual void BeforeTest() {
            if ("ED448".Equals(signAlgorithm)) {
                NUnit.Framework.Assume.That(!FACTORY.IsInApprovedOnlyMode());
            }
            if ("RSASSA".Equals(signAlgorithm)) {
                NUnit.Framework.Assume.That(!FIPS_MODE);
            }
        }

        public PadesTwoPhaseSigningLevelsTest(Object useTempFolder, Object digestAlgorithm, Object signAlgorithm, 
            Object comparisonPdfId) {
            this.useTempFolder = (bool?)useTempFolder;
            this.digestAlgorithm = (String)digestAlgorithm;
            this.comparisonPdfId = (int?)comparisonPdfId;
            this.signAlgorithm = (String)signAlgorithm;
            switch (this.signAlgorithm) {
                case "RSA": {
                    signCertName = "signCertRsa01.pem";
                    rootCertName = "rootRsa.pem";
                    break;
                }

                case "RSASSA": {
                    signCertName = "signRSASSA.pem";
                    rootCertName = "rootRSASSA.pem";
                    break;
                }

                case "ED448": {
                    signCertName = "signEd448.pem";
                    rootCertName = "rootEd448.pem";
                    break;
                }
            }
        }

        public PadesTwoPhaseSigningLevelsTest(Object[] array)
            : this(array[0], array[1], array[2], array[3]) {
        }

        public static IEnumerable<Object[]> CreateParameters() {
            return JavaUtil.ArraysAsList(new Object[] { true, DigestAlgorithms.SHA256, "RSA", 1 }, new Object[] { false
                , DigestAlgorithms.SHA256, "RSASSA", 2 }, new Object[] { false, DigestAlgorithms.SHAKE256, "ED448", 3 }
                , new Object[] { false, DigestAlgorithms.SHA3_384, "RSA", 4 });
        }

        public static ICollection<NUnit.Framework.TestFixtureData> CreateParametersTestFixtureData() {
            return CreateParameters().Select(array => new NUnit.Framework.TestFixtureData(array)).ToList();
        }

        [NUnit.Framework.Test]
        public virtual void TwoStepSigningBaselineBTest() {
            String fileName = "twoStepSigningBaselineBTest" + comparisonPdfId + ".pdf";
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + signCertName;
            String rootCertFileName = certsSrc + rootCertName;
            IX509Certificate signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IX509Certificate[] certChain = new IX509Certificate[] { signCert, rootCert };
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            PadesTwoPhaseSigningHelper twoPhaseSigningHelper = new PadesTwoPhaseSigningHelper();
            if ((bool)useTempFolder) {
                twoPhaseSigningHelper.SetTemporaryDirectoryPath(destinationFolder);
            }
            using (MemoryStream preparedDoc = new MemoryStream()) {
                if (DigestAlgorithms.SHAKE256.Equals(digestAlgorithm) && FIPS_MODE) {
                    NUnit.Framework.Assert.Catch(typeof(AbstractGeneralSecurityException), () => twoPhaseSigningHelper.CreateCMSContainerWithoutSignature
                        (certChain, digestAlgorithm, new PdfReader(srcFileName), preparedDoc, CreateSignerProperties()));
                    return;
                }
                CMSContainer container = twoPhaseSigningHelper.CreateCMSContainerWithoutSignature(certChain, digestAlgorithm
                    , new PdfReader(srcFileName), preparedDoc, CreateSignerProperties());
                IExternalSignature externalSignature;
                if ("RSASSA".Equals(signAlgorithm)) {
                    externalSignature = new PrivateKeySignature(signPrivateKey, digestAlgorithm, "RSASSA-PSS", null);
                }
                else {
                    externalSignature = new PrivateKeySignature(signPrivateKey, digestAlgorithm);
                }
                twoPhaseSigningHelper.SignCMSContainerWithBaselineBProfile(externalSignature, new PdfReader(new MemoryStream
                    (preparedDoc.ToArray())), FileUtil.GetFileOutputStream(outFileName), "Signature1", container);
            }
            TestSignUtils.BasicCheckSignedDoc(outFileName, "Signature1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
        }

        [NUnit.Framework.Test]
        public virtual void TwoStepSigningBaselineTTest() {
            String fileName = "twoStepSigningBaselineTTest" + comparisonPdfId + (FIPS_MODE ? "_FIPS.pdf" : ".pdf");
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + signCertName;
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            String rootCertFileName = certsSrc + rootCertName;
            IX509Certificate signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IX509Certificate[] certChain = new IX509Certificate[] { signCert, rootCert };
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
            PadesTwoPhaseSigningHelper twoPhaseSigningHelper = new PadesTwoPhaseSigningHelper();
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            twoPhaseSigningHelper.SetTSAClient(testTsa);
            if ((bool)useTempFolder) {
                twoPhaseSigningHelper.SetTemporaryDirectoryPath(destinationFolder);
            }
            using (MemoryStream preparedDoc = new MemoryStream()) {
                if (DigestAlgorithms.SHAKE256.Equals(digestAlgorithm) && FIPS_MODE) {
                    NUnit.Framework.Assert.Catch(typeof(AbstractGeneralSecurityException), () => twoPhaseSigningHelper.CreateCMSContainerWithoutSignature
                        (certChain, digestAlgorithm, new PdfReader(srcFileName), preparedDoc, CreateSignerProperties()));
                    return;
                }
                CMSContainer container = twoPhaseSigningHelper.CreateCMSContainerWithoutSignature(certChain, digestAlgorithm
                    , new PdfReader(srcFileName), preparedDoc, CreateSignerProperties());
                IExternalSignature externalSignature;
                if ("RSASSA".Equals(signAlgorithm)) {
                    externalSignature = new PrivateKeySignature(signPrivateKey, digestAlgorithm, "RSASSA-PSS", null);
                }
                else {
                    externalSignature = new PrivateKeySignature(signPrivateKey, digestAlgorithm);
                }
                twoPhaseSigningHelper.SignCMSContainerWithBaselineTProfile(externalSignature, new PdfReader(new MemoryStream
                    (preparedDoc.ToArray())), FileUtil.GetFileOutputStream(outFileName), "Signature1", container);
            }
            TestSignUtils.BasicCheckSignedDoc(outFileName, "Signature1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
        }

        [NUnit.Framework.Test]
        public virtual void TwoStepSigningBaselineLTTest() {
            String fileName = "twoStepSigningBaselineLTTest" + comparisonPdfId + (FIPS_MODE ? "_FIPS.pdf" : ".pdf");
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + signCertName;
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            String caCertFileName = certsSrc + "rootRsa.pem";
            String rootCertFileName = certsSrc + rootCertName;
            IX509Certificate signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IX509Certificate[] certChain = new IX509Certificate[] { signCert, rootCert };
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, PASSWORD);
            PadesTwoPhaseSigningHelper twoPhaseSigningHelper = new PadesTwoPhaseSigningHelper();
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            TestCrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            crlClient.AddBuilderForCertIssuer(rootCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            ocspClient.AddBuilderForCertIssuer(rootCert, caPrivateKey);
            twoPhaseSigningHelper.SetCrlClient(crlClient).SetOcspClient(ocspClient).SetTSAClient(testTsa);
            if ((bool)useTempFolder) {
                twoPhaseSigningHelper.SetTemporaryDirectoryPath(destinationFolder);
            }
            using (MemoryStream preparedDoc = new MemoryStream()) {
                if (DigestAlgorithms.SHAKE256.Equals(digestAlgorithm) && FIPS_MODE) {
                    NUnit.Framework.Assert.Catch(typeof(AbstractGeneralSecurityException), () => twoPhaseSigningHelper.CreateCMSContainerWithoutSignature
                        (certChain, digestAlgorithm, new PdfReader(srcFileName), preparedDoc, CreateSignerProperties()));
                    return;
                }
                CMSContainer container = twoPhaseSigningHelper.CreateCMSContainerWithoutSignature(certChain, digestAlgorithm
                    , new PdfReader(srcFileName), preparedDoc, CreateSignerProperties());
                IExternalSignature externalSignature;
                if ("RSASSA".Equals(signAlgorithm)) {
                    externalSignature = new PrivateKeySignature(signPrivateKey, digestAlgorithm, "RSASSA-PSS", null);
                }
                else {
                    externalSignature = new PrivateKeySignature(signPrivateKey, digestAlgorithm);
                }
                twoPhaseSigningHelper.SignCMSContainerWithBaselineLTProfile(externalSignature, new PdfReader(new MemoryStream
                    (preparedDoc.ToArray())), FileUtil.GetFileOutputStream(outFileName), "Signature1", container);
            }
            TestSignUtils.BasicCheckSignedDoc(outFileName, "Signature1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
        }

        [NUnit.Framework.Test]
        public virtual void TwoStepSigningBaselineLTATest() {
            String fileName = "twoStepSigningBaselineLTATest" + comparisonPdfId + (FIPS_MODE ? "_FIPS.pdf" : ".pdf");
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + signCertName;
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            String caCertFileName = certsSrc + "rootRsa.pem";
            String rootCertFileName = certsSrc + rootCertName;
            IX509Certificate signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IX509Certificate[] certChain = new IX509Certificate[] { signCert, rootCert };
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, PASSWORD);
            PadesTwoPhaseSigningHelper twoPhaseSigningHelper = new PadesTwoPhaseSigningHelper();
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            TestCrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            crlClient.AddBuilderForCertIssuer(rootCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            ocspClient.AddBuilderForCertIssuer(rootCert, caPrivateKey);
            twoPhaseSigningHelper.SetCrlClient(crlClient).SetOcspClient(ocspClient).SetTSAClient(testTsa).SetTimestampSignatureName
                ("timestampSig1");
            if ((bool)useTempFolder) {
                twoPhaseSigningHelper.SetTemporaryDirectoryPath(destinationFolder);
            }
            using (MemoryStream preparedDoc = new MemoryStream()) {
                if (DigestAlgorithms.SHAKE256.Equals(digestAlgorithm) && FIPS_MODE) {
                    NUnit.Framework.Assert.Catch(typeof(AbstractGeneralSecurityException), () => twoPhaseSigningHelper.CreateCMSContainerWithoutSignature
                        (certChain, digestAlgorithm, new PdfReader(srcFileName), preparedDoc, CreateSignerProperties()));
                    return;
                }
                CMSContainer container = twoPhaseSigningHelper.CreateCMSContainerWithoutSignature(certChain, digestAlgorithm
                    , new PdfReader(srcFileName), preparedDoc, CreateSignerProperties());
                IExternalSignature externalSignature;
                if ("RSASSA".Equals(signAlgorithm)) {
                    externalSignature = new PrivateKeySignature(signPrivateKey, digestAlgorithm, "RSASSA-PSS", null);
                }
                else {
                    externalSignature = new PrivateKeySignature(signPrivateKey, digestAlgorithm);
                }
                twoPhaseSigningHelper.SignCMSContainerWithBaselineLTAProfile(externalSignature, new PdfReader(new MemoryStream
                    (preparedDoc.ToArray())), FileUtil.GetFileOutputStream(outFileName), "Signature1", container);
            }
            TestSignUtils.BasicCheckSignedDoc(outFileName, "Signature1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
        }

        private SignerProperties CreateSignerProperties() {
            SignerProperties signerProperties = new SignerProperties();
            signerProperties.SetFieldName("Signature1");
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID).SetContent
                ("Approval test signature.\nCreated by iText.");
            signerProperties.SetPageRect(new Rectangle(50, 650, 200, 100)).SetSignatureAppearance(appearance);
            return signerProperties;
        }
    }
}
