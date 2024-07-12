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
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class PKCS7ExternalSignatureContainerTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly bool FIPS_MODE = "BCFIPS".Equals(FACTORY.GetProviderName());

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/PKCS7ExternalSignatureContainerTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/PKCS7ExternalSignatureContainerTest/";

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private const String POLICY_IDENTIFIER = "2.16.724.1.3.1.1.2.1.9";

        private const String POLICY_HASH_BASE64 = "G7roucf600+f03r/o0bAOQ6WAs0=";

        private static readonly byte[] POLICY_HASH = Convert.FromBase64String(POLICY_HASH_BASE64);

        private const String POLICY_DIGEST_ALGORITHM = "SHA-256";

        private const String POLICY_URI = "https://sede.060.gob.es/politica_de_firma_anexo_1.pdf";

        private IX509Certificate[] chain;

        private IPrivateKey pk;

        private IX509Certificate caCert;

        private IPrivateKey caPrivateKey;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void Init() {
            pk = PemFileHelper.ReadFirstKey(CERTS_SRC + "signCertRsa01.pem", PASSWORD);
            chain = PemFileHelper.ReadFirstChain(CERTS_SRC + "signCertRsa01.pem");
            String caCertP12FileName = CERTS_SRC + "rootRsa.pem";
            caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertP12FileName)[0];
            caPrivateKey = PemFileHelper.ReadFirstKey(caCertP12FileName, PASSWORD);
        }

        [NUnit.Framework.Test]
        public virtual void TestTroughPdfSigner() {
            String outFileName = DESTINATION_FOLDER + "testTroughPdfSigner.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_testTroughPdfSigner.pdf";
            PdfSigner pdfSigner = new PdfSigner(new PdfReader(CreateSimpleDocument()), FileUtil.GetFileOutputStream(outFileName
                ), new StampingProperties());
            PKCS7ExternalSignatureContainer pkcs7ExternalSignatureContainer = new PKCS7ExternalSignatureContainer(pk, 
                chain, DigestAlgorithms.SHA256);
            pdfSigner.SignExternalContainer(pkcs7ExternalSignatureContainer, 12000);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
        }

        [NUnit.Framework.Test]
        public virtual void TestTroughPdfSignerWithCrlClient() {
            String outFileName = DESTINATION_FOLDER + "testTroughPdfSignerWithCrlClient.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_testTroughPdfSignerWithCrlClient.pdf";
            PdfSigner pdfSigner = new PdfSigner(new PdfReader(CreateSimpleDocument()), FileUtil.GetFileOutputStream(outFileName
                ), new StampingProperties());
            PKCS7ExternalSignatureContainer pkcs7ExternalSignatureContainer = new PKCS7ExternalSignatureContainer(pk, 
                chain, DigestAlgorithms.SHA256);
            TestCrlClient crlClient = new TestCrlClient();
            TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, caPrivateKey, TimeTestUtil.TEST_DATE_TIME.AddDays(-
                1));
            crlClient.AddBuilderForCertIssuer(crlBuilder);
            pkcs7ExternalSignatureContainer.SetCrlClient(crlClient);
            pdfSigner.SignExternalContainer(pkcs7ExternalSignatureContainer, 12000);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
        }

        [NUnit.Framework.Test]
        public virtual void TestTroughPdfSignerWithOcspClient() {
            String outFileName = DESTINATION_FOLDER + "testTroughPdfSignerWithOcspClient.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_testTroughPdfSignerWithOcspClient.pdf";
            PdfSigner pdfSigner = new PdfSigner(new PdfReader(CreateSimpleDocument()), FileUtil.GetFileOutputStream(outFileName
                ), new StampingProperties());
            PKCS7ExternalSignatureContainer pkcs7ExternalSignatureContainer = new PKCS7ExternalSignatureContainer(pk, 
                chain, DigestAlgorithms.SHA256);
            TestOcspClient ocspClient = new TestOcspClient();
            ocspClient.AddBuilderForCertIssuer(caCert, caPrivateKey);
            pkcs7ExternalSignatureContainer.SetOcspClient(ocspClient);
            pdfSigner.SignExternalContainer(pkcs7ExternalSignatureContainer, 12000);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
        }

        [NUnit.Framework.Test]
        public virtual void TestTroughPdfSignerWithTsaClient() {
            String outFileName = DESTINATION_FOLDER + "testTroughPdfSignerWithTsaClient.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_testTroughPdfSignerWithTsaClient.pdf";
            if (FIPS_MODE) {
                cmpFileName = cmpFileName.Replace(".pdf", "_FIPS.pdf");
            }
            PdfSigner pdfSigner = new PdfSigner(new PdfReader(CreateSimpleDocument()), FileUtil.GetFileOutputStream(outFileName
                ), new StampingProperties());
            PKCS7ExternalSignatureContainer pkcs7ExternalSignatureContainer = new PKCS7ExternalSignatureContainer(pk, 
                chain, DigestAlgorithms.SHA256);
            String tsaCertP12FileName = CERTS_SRC + "tsCertRsa.pem";
            pkcs7ExternalSignatureContainer.SetTsaClient(PrepareTsaClient(tsaCertP12FileName));
            pdfSigner.SignExternalContainer(pkcs7ExternalSignatureContainer, 12000);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
        }

        [NUnit.Framework.Test]
        public virtual void TestTroughPdfSignerWithCadesType() {
            String outFileName = DESTINATION_FOLDER + "testTroughPdfSignerWithCadesType.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_testTroughPdfSignerWithCadesType.pdf";
            PdfSigner pdfSigner = new PdfSigner(new PdfReader(CreateSimpleDocument()), FileUtil.GetFileOutputStream(outFileName
                ), new StampingProperties());
            PKCS7ExternalSignatureContainer pkcs7ExternalSignatureContainer = new PKCS7ExternalSignatureContainer(pk, 
                chain, DigestAlgorithms.SHA256);
            pkcs7ExternalSignatureContainer.SetSignatureType(PdfSigner.CryptoStandard.CADES);
            pdfSigner.SignExternalContainer(pkcs7ExternalSignatureContainer, 12000);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
        }

        [NUnit.Framework.Test]
        public virtual void TestTroughPdfSignerWithSignaturePolicy() {
            String outFileName = DESTINATION_FOLDER + "testTroughPdfSignerWithSignaturePolicy.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_testTroughPdfSignerWithSignaturePolicy.pdf";
            PdfSigner pdfSigner = new PdfSigner(new PdfReader(CreateSimpleDocument()), FileUtil.GetFileOutputStream(outFileName
                ), new StampingProperties());
            PKCS7ExternalSignatureContainer pkcs7ExternalSignatureContainer = new PKCS7ExternalSignatureContainer(pk, 
                chain, DigestAlgorithms.SHA256);
            SignaturePolicyInfo policy = new SignaturePolicyInfo(POLICY_IDENTIFIER, POLICY_HASH, POLICY_DIGEST_ALGORITHM
                , POLICY_URI);
            pkcs7ExternalSignatureContainer.SetSignaturePolicy(policy);
            pdfSigner.SignExternalContainer(pkcs7ExternalSignatureContainer, 12000);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
        }

        private static MemoryStream CreateSimpleDocument() {
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            WriterProperties writerProperties = new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0);
            PdfDocument document = new PdfDocument(new PdfWriter(outputStream, writerProperties));
            document.AddNewPage();
            document.Close();
            return new MemoryStream(outputStream.ToArray());
        }

        private static TestTsaClient PrepareTsaClient(String tsaCertP12FileName) {
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertP12FileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertP12FileName, PASSWORD);
            return new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
        }
    }
}
