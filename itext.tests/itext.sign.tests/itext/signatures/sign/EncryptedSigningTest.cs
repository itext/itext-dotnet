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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Crypto;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class EncryptedSigningTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/EncryptedSigningTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/EncryptedSigningTest/";

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private IX509Certificate[] chain;

        private IPrivateKey pk;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void Init() {
            pk = PemFileHelper.ReadFirstKey(CERTS_SRC + "signCertRsa01.pem", PASSWORD);
            chain = PemFileHelper.ReadFirstChain(CERTS_SRC + "signCertRsa01.pem");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void SignEncryptedPdfTest() {
            String srcFile = SOURCE_FOLDER + "encrypted.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_signedEncrypted.pdf";
            String outPdf = DESTINATION_FOLDER + "signedEncrypted.pdf";
            String fieldName = "Signature1";
            byte[] ownerPass = "World".GetBytes();
            PdfReader reader = new PdfReader(srcFile, new ReaderProperties().SetPassword(ownerPass));
            PdfSigner signer = new PdfSigner(reader, FileUtil.GetFileOutputStream(outPdf), new StampingProperties().UseAppendMode
                ());
            SignerProperties signerProperties = new SignerProperties().SetFieldName(fieldName);
            signer.SetSignerProperties(signerProperties);
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES
                );
            //Password to open out and cmp files are the same
            ReaderProperties properties = new ReaderProperties().SetPassword(ownerPass);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outPdf, cmpPdf, properties, properties
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SignCertificateSecurityPdfTest() {
            //RSA keys in FIPS are supported for signature verification only
            if (!FACTORY.GetProviderName().Contains("FIPS")) {
                String srcFile = SOURCE_FOLDER + "signCertificateSecurityPdf.pdf";
                String cmpPdf = SOURCE_FOLDER + "cmp_signCertificateSecurityPdf.pdf";
                String outPdf = DESTINATION_FOLDER + "signCertificateSecurityPdf.pdf";
                PdfReader reader = new PdfReader(srcFile, new ReaderProperties().SetPublicKeySecurityParams(chain[0], pk));
                PdfSigner signer = new PdfSigner(reader, FileUtil.GetFileOutputStream(outPdf), new StampingProperties().UseAppendMode
                    ());
                // Creating the signature
                IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
                signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES
                    );
                ReaderProperties properties = new ReaderProperties().SetPublicKeySecurityParams(chain[0], pk);
                //Public key to open out and cmp files are the same
                NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outPdf, cmpPdf, properties, properties
                    ));
            }
        }
    }
}
