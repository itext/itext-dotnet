/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using iText.Forms.Form.Element;
using iText.IO.Source;
using iText.Kernel.Crypto;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures.Cms;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Test;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PadesTwoPhaseSigningLevelsTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly bool FIPS_MODE = "BCFIPS".Equals(FACTORY.GetProviderName());

        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PadesTwoPhaseSigningLevelsTest/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PadesTwoPhaseSigningLevelsTest/";

        private static readonly String destinationFolder = TestUtil.GetOutputPath() + "/signatures/sign/PadesTwoPhaseSigningLevelsTest/";
        

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        private void BeforeTest(string signAlgorithm) {
            if ("ED448".Equals(signAlgorithm)) {
                NUnit.Framework.Assume.That(!FACTORY.IsInApprovedOnlyMode());
            }
            if ("RSASSA".Equals(signAlgorithm)) {
                NUnit.Framework.Assume.That(!FIPS_MODE);
            }
        }

        private string GetSignCertName(string signAlgorithm)
        {
            var aignCertName = "";
            switch (signAlgorithm) {
                case "RSA": {
                    aignCertName = "signCertRsa01.pem";
                    break;
                }

                case "RSASSA": {
                    aignCertName = "signRSASSA.pem";
                    break;
                }

                case "ED448": {
                    aignCertName = "signEd448.pem";
                    break;
                }
            }

            return aignCertName;
        }
        
        private string GetRootCertName(string signAlgorithm)
        {
            var rootCertName = "";
            switch (signAlgorithm) {
                case "RSA": {
                    rootCertName = "rootRsa.pem";
                    break;
                }

                case "RSASSA": {
                    rootCertName = "rootRSASSA.pem";
                    break;
                }

                case "ED448": {
                    rootCertName = "rootEd448.pem";
                    break;
                }
            }

            return rootCertName;
        }

        public static IEnumerable<Object[]> DataSource() {
            return JavaUtil.ArraysAsList(new Object[] { true, DigestAlgorithms.SHA256, "RSA", 1 }, new Object[] { false
                , DigestAlgorithms.SHA256, "RSASSA", 2 }, new Object[] { false, DigestAlgorithms.SHAKE256, "ED448", 3 }
                , new Object[] { false, DigestAlgorithms.SHA3_384, "RSA", 4 });
        }

        [NUnit.Framework.TestCaseSource("DataSource")]
        public virtual void TwoStepSigningBaselineBTest(bool useTempFolder, string digestAlgorithm, string signAlgorithm, int comparisonPdfId) {
            BeforeTest(signAlgorithm);
            var fileName = "twoStepSigningBaselineBTest" + comparisonPdfId + ".pdf";
            var outFileName = destinationFolder + fileName;
            var cmpFileName = sourceFolder + "cmp_" + fileName;
            var srcFileName = sourceFolder + "helloWorldDoc.pdf";
            var signCertFileName = certsSrc + GetSignCertName(signAlgorithm);
            var rootCertFileName = certsSrc + GetRootCertName(signAlgorithm);
            var signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            var rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            var certChain = new IX509Certificate[] { signCert, rootCert };
            var signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            var twoPhaseSigningHelper = new PadesTwoPhaseSigningHelper();
            if ((bool)useTempFolder) {
                twoPhaseSigningHelper.SetTemporaryDirectoryPath(destinationFolder);
            }
            using (var preparedDoc = new MemoryStream()) {
                if (DigestAlgorithms.SHAKE256.Equals(digestAlgorithm) && FIPS_MODE) {
                    NUnit.Framework.Assert.Catch(typeof(AbstractGeneralSecurityException), () => twoPhaseSigningHelper.CreateCMSContainerWithoutSignature
                        (certChain, digestAlgorithm, new PdfReader(srcFileName), preparedDoc, CreateSignerProperties()));
                    return;
                }
                var container = twoPhaseSigningHelper.CreateCMSContainerWithoutSignature(certChain, digestAlgorithm
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

        [NUnit.Framework.TestCaseSource("DataSource")]
        public virtual void TwoStepSigningBaselineTTest(bool useTempFolder, string digestAlgorithm, string signAlgorithm, int comparisonPdfId) {
            BeforeTest(signAlgorithm);
            var fileName = "twoStepSigningBaselineTTest" + comparisonPdfId + (FIPS_MODE ? "_FIPS.pdf" : ".pdf");
            var outFileName = destinationFolder + fileName;
            var cmpFileName = sourceFolder + "cmp_" + fileName;
            var srcFileName = sourceFolder + "helloWorldDoc.pdf";
            var signCertFileName = certsSrc + GetSignCertName(signAlgorithm);
            var tsaCertFileName = certsSrc + "tsCertRsa.pem";
            var rootCertFileName = certsSrc + GetRootCertName(signAlgorithm);
            var signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            var rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            var certChain = new IX509Certificate[] { signCert, rootCert };
            var signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            var tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            var tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
            var twoPhaseSigningHelper = new PadesTwoPhaseSigningHelper();
            var testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            twoPhaseSigningHelper.SetTSAClient(testTsa);
            if ((bool)useTempFolder) {
                twoPhaseSigningHelper.SetTemporaryDirectoryPath(destinationFolder);
            }
            using (var preparedDoc = new MemoryStream()) {
                if (DigestAlgorithms.SHAKE256.Equals(digestAlgorithm) && FIPS_MODE) {
                    NUnit.Framework.Assert.Catch(typeof(AbstractGeneralSecurityException), () => twoPhaseSigningHelper.CreateCMSContainerWithoutSignature
                        (certChain, digestAlgorithm, new PdfReader(srcFileName), preparedDoc, CreateSignerProperties()));
                    return;
                }
                var container = twoPhaseSigningHelper.CreateCMSContainerWithoutSignature(certChain, digestAlgorithm
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

        [NUnit.Framework.TestCaseSource("DataSource")]
        public virtual void TwoStepSigningBaselineLTTest(bool useTempFolder, string digestAlgorithm, string signAlgorithm, int comparisonPdfId) {
            BeforeTest(signAlgorithm);
            var fileName = "twoStepSigningBaselineLTTest" + comparisonPdfId + (FIPS_MODE ? "_FIPS.pdf" : ".pdf");
            var outFileName = destinationFolder + fileName;
            var cmpFileName = sourceFolder + "cmp_" + fileName;
            var srcFileName = sourceFolder + "helloWorldDoc.pdf";
            var signCertFileName = certsSrc + GetSignCertName(signAlgorithm);
            var tsaCertFileName = certsSrc + "tsCertRsa.pem";
            var caCertFileName = certsSrc + "rootRsa.pem";
            var rootCertFileName = certsSrc + GetRootCertName(signAlgorithm);
            var signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            var rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            var certChain = new IX509Certificate[] { signCert, rootCert };
            var signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            var tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            var tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
            var caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            var caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, PASSWORD);
            var twoPhaseSigningHelper = new PadesTwoPhaseSigningHelper();
            var testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            var crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            crlClient.AddBuilderForCertIssuer(rootCert, caPrivateKey);
            var ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            ocspClient.AddBuilderForCertIssuer(rootCert, caPrivateKey);
            twoPhaseSigningHelper.SetCrlClient(crlClient).SetOcspClient(ocspClient).SetTSAClient(testTsa);
            if ((bool)useTempFolder) {
                twoPhaseSigningHelper.SetTemporaryDirectoryPath(destinationFolder);
            }
            using (var preparedDoc = new MemoryStream()) {
                if (DigestAlgorithms.SHAKE256.Equals(digestAlgorithm) && FIPS_MODE) {
                    NUnit.Framework.Assert.Catch(typeof(AbstractGeneralSecurityException), () => twoPhaseSigningHelper.CreateCMSContainerWithoutSignature
                        (certChain, digestAlgorithm, new PdfReader(srcFileName), preparedDoc, CreateSignerProperties()));
                    return;
                }
                var container = twoPhaseSigningHelper.CreateCMSContainerWithoutSignature(certChain, digestAlgorithm
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

        [NUnit.Framework.TestCaseSource("DataSource")]
        public virtual void TwoStepSigningBaselineLTATest(bool useTempFolder, string digestAlgorithm, string signAlgorithm, int comparisonPdfId) {
            BeforeTest(signAlgorithm);
            var fileName = "twoStepSigningBaselineLTATest" + comparisonPdfId + (FIPS_MODE ? "_FIPS.pdf" : ".pdf");
            var outFileName = destinationFolder + fileName;
            var cmpFileName = sourceFolder + "cmp_" + fileName;
            var srcFileName = sourceFolder + "helloWorldDoc.pdf";
            var signCertFileName = certsSrc + GetSignCertName(signAlgorithm);
            var tsaCertFileName = certsSrc + "tsCertRsa.pem";
            var caCertFileName = certsSrc + "rootRsa.pem";
            var rootCertFileName = certsSrc + GetRootCertName(signAlgorithm);
            var signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            var rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            var certChain = new IX509Certificate[] { signCert, rootCert };
            var signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            var tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            var tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
            var caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            var caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, PASSWORD);
            var twoPhaseSigningHelper = new PadesTwoPhaseSigningHelper();
            var testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            var crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            crlClient.AddBuilderForCertIssuer(rootCert, caPrivateKey);
            var ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            ocspClient.AddBuilderForCertIssuer(rootCert, caPrivateKey);
            twoPhaseSigningHelper.SetCrlClient(crlClient).SetOcspClient(ocspClient).SetTSAClient(testTsa).SetTimestampSignatureName
                ("timestampSig1");
            if ((bool)useTempFolder) {
                twoPhaseSigningHelper.SetTemporaryDirectoryPath(destinationFolder);
            }
            using (var preparedDoc = new MemoryStream()) {
                if (DigestAlgorithms.SHAKE256.Equals(digestAlgorithm) && FIPS_MODE) {
                    NUnit.Framework.Assert.Catch(typeof(AbstractGeneralSecurityException), () => twoPhaseSigningHelper.CreateCMSContainerWithoutSignature
                        (certChain, digestAlgorithm, new PdfReader(srcFileName), preparedDoc, CreateSignerProperties()));
                    return;
                }
                var container = twoPhaseSigningHelper.CreateCMSContainerWithoutSignature(certChain, digestAlgorithm
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

        public static IEnumerable<Object[]> CreateSignatureCreatorParameters()
        {
            return JavaUtil.ArraysAsList(new Object[] { null },new Object[] { "Custom Signature creator" });
        }

        [NUnit.Framework.TestCaseSource("CreateSignatureCreatorParameters")]
        public virtual void TwoStepSigningSignatureCreatorTest(String signatureCreator) {
            String signAlgorithm = "RSA";
            String digestAlgorithm = DigestAlgorithms.SHA256;
            String fileName = "twoStepSigningBaselineLTATest"
                    + (signatureCreator == null ? "NoSignatureCreator" : "CustomSignatureCreator") + ".pdf";
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + GetSignCertName(signAlgorithm);
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            String caCertFileName = certsSrc + "rootRsa.pem";
            String rootCertFileName = certsSrc + GetRootCertName(signAlgorithm);

            ExecuteTwoStepSigning(signCertFileName, rootCertFileName, tsaCertFileName, caCertFileName, digestAlgorithm,
                signAlgorithm, srcFileName, outFileName, false, signatureCreator);

            TestSignUtils.BasicCheckSignedDoc(outFileName, "Signature1");
            SignatureUtil signatureUtil = new SignatureUtil(new PdfDocument(new PdfReader(outFileName)));
            String actualTimeStampCreator = GetSignatureCreator(signatureUtil.GetSignature("timestampSig1"));
            NUnit.Framework.Assert.AreEqual("", actualTimeStampCreator);
            String actualSignatureCreator = GetSignatureCreator(signatureUtil.GetSignature("Signature1"));
            if (signatureCreator == null) {

                ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
                PdfDocument doc = new PdfDocument(new PdfWriter(outputStream));
                doc.AddNewPage();
                doc.Close();
                using (PdfDocument regularPdf = new PdfDocument(new PdfReader(new MemoryStream(outputStream.ToArray())))) {
                    NUnit.Framework.Assert.AreEqual(regularPdf.GetDocumentInfo().GetProducer(), actualSignatureCreator);
                }
            } else {
                NUnit.Framework.Assert.AreEqual(signatureCreator, actualSignatureCreator);
            }
        }

        private void ExecuteTwoStepSigning(String signCertFileName, String rootCertFileName, String tsaCertFileName,
            String caCertFileName, String digestAlgorithm, String signAlgorithm, String srcFileName, String outFileName, 
            Boolean useTempFolder) {
        ExecuteTwoStepSigning(signCertFileName, rootCertFileName, tsaCertFileName, caCertFileName, digestAlgorithm, 
            signAlgorithm, srcFileName, outFileName, useTempFolder, null);
        }
        private void ExecuteTwoStepSigning(String signCertFileName, String rootCertFileName, String tsaCertFileName,
            String caCertFileName, String digestAlgorithm, String signAlgorithm, String srcFileName, String outFileName, 
            Boolean useTempFolder, String signatureCreator) {
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

            twoPhaseSigningHelper.SetCrlClient(crlClient).SetOcspClient(ocspClient).SetTSAClient(testTsa)
                    .SetTimestampSignatureName("timestampSig1");

            SignerProperties prop = new SignerProperties();
            if (signatureCreator != null) {
                prop.SetSignatureCreator(signatureCreator);
            }

            if (useTempFolder) {
                twoPhaseSigningHelper.SetTemporaryDirectoryPath(destinationFolder);
            }

            using (MemoryStream preparedDoc = new MemoryStream()) {
                CMSContainer container = twoPhaseSigningHelper.CreateCMSContainerWithoutSignature(certChain,
                        digestAlgorithm, new PdfReader(srcFileName), preparedDoc, prop);

                IExternalSignature externalSignature = new PrivateKeySignature(signPrivateKey, digestAlgorithm);

                if ("RSASSA".Equals(signAlgorithm))
                {
                    externalSignature = new PrivateKeySignature(signPrivateKey, digestAlgorithm, "RSASSA-PSS", null);
                }
                else
                {
                    externalSignature = new PrivateKeySignature(signPrivateKey, digestAlgorithm);
                }
                twoPhaseSigningHelper.SignCMSContainerWithBaselineLTAProfile(externalSignature, new PdfReader(new MemoryStream
                    (preparedDoc.ToArray())), FileUtil.GetFileOutputStream(outFileName), "Signature1", container);
            }
        }

        private String GetSignatureCreator(PdfSignature signature)
        {
            return signature.GetPdfObject().GetAsDictionary(PdfName.Prop_Build).GetAsDictionary(PdfName.App)
                    .GetAsName(PdfName.Name).GetValue();
        }

        private SignerProperties CreateSignerProperties() {
            var signerProperties = new SignerProperties();
            signerProperties.SetFieldName("Signature1");
            var appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID).SetContent
                ("Approval test signature.\nCreated by iText.");
            signerProperties.SetPageRect(new Rectangle(50, 650, 200, 100)).SetSignatureAppearance(appearance);
            return signerProperties;
        }
    }
}
