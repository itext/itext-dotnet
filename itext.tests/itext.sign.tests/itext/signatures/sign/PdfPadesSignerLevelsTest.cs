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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Forms.Form.Element;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    [NUnit.Framework.TestFixtureSource("CreateParametersTestFixtureData")]
    public class PdfPadesSignerLevelsTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfPadesSignerLevelsTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PdfPadesSignerLevelsTest/";

        private readonly bool? useTempFolder;

        private readonly int? comparisonPdfId;

        private readonly bool? useSignature;

        private static readonly char[] password = "testpassphrase".ToCharArray();
        private static bool runningInFipsMode;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
            runningInFipsMode = "BCFIPS".Equals(BouncyCastleFactoryCreator.GetFactory().GetProviderName());
        }

        public PdfPadesSignerLevelsTest(Object useTempFolder, Object useSignature, Object comparisonPdfId) {
            this.useTempFolder = (bool?)useTempFolder;
            this.useSignature = (bool?)useSignature;
            this.comparisonPdfId = (int?)comparisonPdfId;
        }

        public PdfPadesSignerLevelsTest(Object[] array)
            : this(array[0], array[1], array[2]) {
        }

        public static IEnumerable<Object[]> CreateParameters() {
            return JavaUtil.ArraysAsList(new Object[] { true, true, 1 }, new Object[] { false, true, 2 }, new Object[]
                 { false, false, 3 });
        }

        public static ICollection<NUnit.Framework.TestFixtureData> CreateParametersTestFixtureData() {
            return CreateParameters().Select(array => new NUnit.Framework.TestFixtureData(array)).ToList();
        }

        [NUnit.Framework.Test]
        public virtual void PadesSignatureLevelBTest() {
            String fileName = "padesSignatureLevelBTest" + comparisonPdfId + ".pdf";
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            if (runningInFipsMode)
            {
                cmpFileName = cmpFileName.Replace(".pdf", "_FIPS.pdf");
            }
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "signCertRsa01.pem";
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, password);
            SignerProperties signerProperties = CreateSignerProperties();
            PdfPadesSigner padesSigner = CreatePdfPadesSigner(srcFileName, outFileName);
            if ((bool)useTempFolder) {
                padesSigner.SetTemporaryDirectoryPath(destinationFolder);
            }
            if ((bool)useSignature) {
                IExternalSignature pks = new PrivateKeySignature(signRsaPrivateKey, DigestAlgorithms.SHA256);
                padesSigner.SignWithBaselineBProfile(signerProperties, signRsaChain, pks);
            }
            else {
                padesSigner.SignWithBaselineBProfile(signerProperties, signRsaChain, signRsaPrivateKey);
            }
            TestSignUtils.BasicCheckSignedDoc(outFileName, "Signature1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
        }

        [NUnit.Framework.Test]
        public virtual void PadesSignatureLevelTTest() {
            String fileName = "padesSignatureLevelTTest" + comparisonPdfId + ".pdf";
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            if (runningInFipsMode)
            {
                cmpFileName = cmpFileName.Replace(".pdf", "_FIPS.pdf");
            }
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "signCertRsa01.pem";
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, password);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, password);
            SignerProperties signerProperties = CreateSignerProperties();
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            PdfPadesSigner padesSigner = CreatePdfPadesSigner(srcFileName, outFileName);
            if ((bool)useTempFolder) {
                padesSigner.SetTemporaryDirectoryPath(destinationFolder);
            }
            if ((bool)useSignature) {
                IExternalSignature pks = new PrivateKeySignature(signRsaPrivateKey, DigestAlgorithms.SHA256);
                padesSigner.SignWithBaselineTProfile(signerProperties, signRsaChain, pks, testTsa);
            }
            else {
                padesSigner.SignWithBaselineTProfile(signerProperties, signRsaChain, signRsaPrivateKey, testTsa);
            }
            TestSignUtils.BasicCheckSignedDoc(outFileName, "Signature1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
        }

        [NUnit.Framework.Test]
        public virtual void PadesSignatureLevelLTTest() {
            String fileName = "padesSignatureLevelLTTest" + comparisonPdfId + ".pdf";
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            if (runningInFipsMode)
            {
                cmpFileName = cmpFileName.Replace(".pdf", "_FIPS.pdf");
            }
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "signCertRsa01.pem";
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            String caCertFileName = certsSrc + "rootRsa.pem";
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, password);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, password);
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, password);
            SignerProperties signerProperties = CreateSignerProperties();
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            ICrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            PdfPadesSigner padesSigner = CreatePdfPadesSigner(srcFileName, outFileName);
            if ((bool)useTempFolder) {
                padesSigner.SetTemporaryDirectoryPath(destinationFolder);
            }
            padesSigner.SetOcspClient(ocspClient).SetCrlClient(crlClient);
            if ((bool)useSignature) {
                IExternalSignature pks = new PrivateKeySignature(signRsaPrivateKey, DigestAlgorithms.SHA256);
                padesSigner.SignWithBaselineLTProfile(signerProperties, signRsaChain, pks, testTsa);
            }
            else {
                padesSigner.SignWithBaselineLTProfile(signerProperties, signRsaChain, signRsaPrivateKey, testTsa);
            }
            TestSignUtils.BasicCheckSignedDoc(outFileName, "Signature1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
        }

        [NUnit.Framework.Test]
        public virtual void PadesSignatureLevelLTATest() {
            String fileName = "padesSignatureLevelLTATest" + comparisonPdfId + ".pdf";
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            if (runningInFipsMode)
            {
                cmpFileName = cmpFileName.Replace(".pdf", "_FIPS.pdf");
            }
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "signCertRsa01.pem";
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            String caCertFileName = certsSrc + "rootRsa.pem";
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, password);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, password);
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, password);
            SignerProperties signerProperties = CreateSignerProperties();
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            ICrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            PdfPadesSigner padesSigner = CreatePdfPadesSigner(srcFileName, outFileName);
            if ((bool)useTempFolder) {
                padesSigner.SetTemporaryDirectoryPath(destinationFolder);
            }
            padesSigner.SetOcspClient(ocspClient).SetCrlClient(crlClient).SetTimestampSignatureName("timestampSig1");
            if ((bool)useSignature) {
                IExternalSignature pks = new PrivateKeySignature(signRsaPrivateKey, DigestAlgorithms.SHA256);
                padesSigner.SignWithBaselineLTAProfile(signerProperties, signRsaChain, pks, testTsa);
            }
            else {
                padesSigner.SignWithBaselineLTAProfile(signerProperties, signRsaChain, signRsaPrivateKey, testTsa);
            }
            TestSignUtils.BasicCheckSignedDoc(outFileName, "Signature1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
        }

        [NUnit.Framework.Test]
        public virtual void ProlongDocumentSignaturesTest() {
            String fileName = "prolongDocumentSignaturesTest" + comparisonPdfId + ".pdf";
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            String srcFileName = sourceFolder + "padesSignatureLevelLTA.pdf";
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            String caCertFileName = certsSrc + "rootRsa.pem";
            if (runningInFipsMode)
            {
                cmpFileName = cmpFileName.Replace(".pdf", "_FIPS.pdf");
            }
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, password);
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, password);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            ICrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            PdfPadesSigner padesSigner = CreatePdfPadesSigner(srcFileName, outFileName);
            if ((bool)useTempFolder) {
                padesSigner.SetTemporaryDirectoryPath(destinationFolder);
            }
            padesSigner.SetOcspClient(ocspClient).SetCrlClient(crlClient);
            if ((bool)useSignature) {
                padesSigner.ProlongSignatures(testTsa);
            }
            else {
                padesSigner.ProlongSignatures();
            }
            TestSignUtils.BasicCheckSignedDoc(outFileName, "Signature1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
        }

        private SignerProperties CreateSignerProperties() {
            SignerProperties signerProperties = new SignerProperties();
            signerProperties.SetFieldName("Signature1");
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(signerProperties.GetFieldName())
                .SetContent("Approval test signature.\nCreated by iText.");
            signerProperties.SetPageRect(new Rectangle(50, 650, 200, 100))
                .SetSignatureAppearance(appearance);

            return signerProperties;
        }

        private PdfPadesSigner CreatePdfPadesSigner(String srcFileName, String outFileName) {
            return new PdfPadesSigner(new PdfReader(FileUtil.GetInputStreamForFile(srcFileName)),
                FileUtil.GetFileOutputStream(outFileName));
        }
    }
}
