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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using iText.Forms.Form.Element;
using iText.Kernel.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Exceptions;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PdfPadesSignerTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();
        
        private static readonly bool FIPS_MODE = "BCFIPS".Equals(FACTORY.GetProviderName());

        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfPadesSignerTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PdfPadesSignerTest/";

        private static readonly char[] password = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void DirectoryPathIsNotADirectoryTest() {
            String fileName = "directoryPathIsNotADirectoryTest.pdf";
            String outFileName = destinationFolder + fileName;
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "signCertRsa01.pem";
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            String caCertFileName = certsSrc + "rootRsa.pem";
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, password);
            IExternalSignature pks = new PrivateKeySignature(signRsaPrivateKey, DigestAlgorithms.SHA256);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, password);
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, password);
            SignerProperties signerProperties = CreateSignerProperties();
            PdfPadesSigner padesSigner = CreatePdfPadesSigner(srcFileName, outFileName);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            ICrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            padesSigner.SetTemporaryDirectoryPath(destinationFolder + "newPdf.pdf");
            padesSigner.SetOcspClient(ocspClient).SetCrlClient(crlClient);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => padesSigner.SignWithBaselineLTProfile
                (signerProperties, signRsaChain, pks, testTsa));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(SignExceptionMessageConstant.PATH_IS_NOT_DIRECTORY
                , destinationFolder + "newPdf.pdf"), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NoSignaturesToProlongTest() {
            String fileName = "noSignaturesToProlongTest.pdf";
            String outFileName = destinationFolder + fileName;
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            String caCertFileName = certsSrc + "rootRsa.pem";
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, password);
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, password);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            ICrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            PdfPadesSigner padesSigner = CreatePdfPadesSigner(srcFileName, outFileName);
            padesSigner.SetOcspClient(ocspClient).SetCrlClient(crlClient);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => padesSigner.ProlongSignatures
                (testTsa));
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.NO_SIGNATURES_TO_PROLONG, exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultClientsCannotBeCreated() {
            String fileName = "defaultClientsCannotBeCreated.pdf";
            String outFileName = destinationFolder + fileName;
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "signCertRsa01.pem";
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, password);
            IExternalSignature pks = new PrivateKeySignature(signRsaPrivateKey, DigestAlgorithms.SHA256);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, password);
            SignerProperties signerProperties = CreateSignerProperties();
            PdfPadesSigner padesSigner = CreatePdfPadesSigner(srcFileName, outFileName);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            padesSigner.SetTemporaryDirectoryPath(destinationFolder);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => padesSigner.SignWithBaselineLTProfile
                (signerProperties, signRsaChain, pks, testTsa));
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.DEFAULT_CLIENTS_CANNOT_BE_CREATED, exception.
                Message);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultSignerPropertiesTest() {
            String fileName = "defaultSignerPropertiesTest" + (FIPS_MODE ? "_FIPS.pdf" : ".pdf");
            String outFileName = destinationFolder + fileName;
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            String signCertFileName = certsSrc + "signCertRsa01.pem";
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            String caCertFileName = certsSrc + "rootRsa.pem";
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, password);
            IExternalSignature pks = new PrivateKeySignature(signRsaPrivateKey, DigestAlgorithms.SHA256);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, password);
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, password);
            SignerProperties signerProperties = new SignerProperties();
            PdfPadesSigner padesSigner = CreatePdfPadesSigner(srcFileName, outFileName);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            ICrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            padesSigner.SetOcspClient(ocspClient).SetCrlClient(crlClient);
            padesSigner.SignWithBaselineLTAProfile(signerProperties, signRsaChain, pks, testTsa);
            TestSignUtils.BasicCheckSignedDoc(outFileName, "Signature1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
        }
        
        [NUnit.Framework.Test]
        public virtual void SmallTokenSizeEstimationTest() {
            String fileName = "smallTokenSizeEstimationTest.pdf";
            String outFileName = destinationFolder + fileName;
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "signCertRsa01.pem";
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            String caCertFileName = certsSrc + "rootRsa.pem";
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, password);
            IExternalSignature pks = new PrivateKeySignature(signRsaPrivateKey, DigestAlgorithms.SHA256);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, password);
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, password);
            SignerProperties signerProperties = new SignerProperties();
            PdfPadesSigner padesSigner = CreatePdfPadesSigner(srcFileName, outFileName);
            TestTsaClient testTsa = new TestTsaClientWithCustomSizeEstimation(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            ICrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            padesSigner.SetOcspClient(ocspClient).SetCrlClient(crlClient);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(IOException), () => padesSigner.SignWithBaselineLTAProfile
                (signerProperties, signRsaChain, pks, testTsa));
        }
        
        [NUnit.Framework.Test]
        public virtual void PadesSignatureEd25519Test() {
            NUnit.Framework.Assume.That(!FACTORY.IsInApprovedOnlyMode());
            String fileName = "padesSignatureEd25519Test.pdf";
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "signCertEd25519.pem";
            IX509Certificate[] signEdDSAChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signEdDSAPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, password);
            SignerProperties signerProperties = CreateSignerProperties();
            PdfPadesSigner padesSigner = CreatePdfPadesSigner(srcFileName, outFileName);
            if (FIPS_MODE) {
                // algorithm identifier in key not recognised
                Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => padesSigner.SignWithBaselineBProfile
                    (signerProperties, signEdDSAChain, signEdDSAPrivateKey));
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(SignExceptionMessageConstant.ALGORITHMS_NOT_SUPPORTED,
                    "SHA512withEd25519", "Ed25519"), exception.Message);
            } else {
                padesSigner.SignWithBaselineBProfile(signerProperties, signEdDSAChain, signEdDSAPrivateKey);
                TestSignUtils.BasicCheckSignedDoc(outFileName, "Signature1");
                NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
            }
        }

        [NUnit.Framework.Test]
        public virtual void PadesSignatureEd448Test() {
            NUnit.Framework.Assume.That(!FACTORY.IsInApprovedOnlyMode());
            String fileName = "padesSignatureEd448Test.pdf";
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + fileName;
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "signCertEd448.pem";
            IX509Certificate[] signEdDSAChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signEdDSAPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, password);
            SignerProperties signerProperties = CreateSignerProperties();
            PdfPadesSigner padesSigner = CreatePdfPadesSigner(srcFileName, outFileName);
            if (FIPS_MODE) {
                // SHAKE256 is currently not supported in BCFIPS
                Exception exception = NUnit.Framework.Assert.Catch(typeof(AbstractGeneralSecurityException), () => padesSigner.SignWithBaselineBProfile
                    (signerProperties, signEdDSAChain, signEdDSAPrivateKey));
            }
            else {
                padesSigner.SignWithBaselineBProfile(signerProperties, signEdDSAChain, signEdDSAPrivateKey);
                TestSignUtils.BasicCheckSignedDoc(outFileName, "Signature1");
                NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
            }
        }

        private SignerProperties CreateSignerProperties() {
            SignerProperties signerProperties = new SignerProperties();
            signerProperties.SetFieldName("Signature1");
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID).SetContent
                ("Approval test signature.\nCreated by iText.");
            signerProperties.SetPageRect(new Rectangle(50, 650, 200, 100)).SetSignatureAppearance(appearance);
            return signerProperties;
        }

        private PdfPadesSigner CreatePdfPadesSigner(String srcFileName, String outFileName) {
            return new PdfPadesSigner(new PdfReader(FileUtil.GetInputStreamForFile(srcFileName)), FileUtil.GetFileOutputStream
                (outFileName));
        }

        private sealed class TestTsaClientWithCustomSizeEstimation : TestTsaClient {
            public TestTsaClientWithCustomSizeEstimation(IList<IX509Certificate> tsaCertificateChain,
                IPrivateKey tsaPrivateKey) : base(tsaCertificateChain, tsaPrivateKey) {
            }

            public override int GetTokenSizeEstimate() {
                return 1024;
            }
        }
    }
}
