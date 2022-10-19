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
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;
using iText.Test.Signutils;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("IntegrationTest")]
    public class EncryptedSigningTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/EncryptedSigningTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/EncryptedSigningTest/";

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] PASSWORD = "testpass".ToCharArray();

        private X509Certificate[] chain;

        private ICipherParameters pk;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void Init() {
            pk = Pkcs12FileHelper.ReadFirstKey(CERTS_SRC + "signCertRsa01.p12", PASSWORD, PASSWORD);
            chain = Pkcs12FileHelper.ReadFirstChain(CERTS_SRC + "signCertRsa01.p12", PASSWORD);
        }

        [NUnit.Framework.Test]
        public virtual void SignEncryptedPdfTest() {
            String srcFile = SOURCE_FOLDER + "encrypted.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_signedEncrypted.pdf";
            String outPdf = DESTINATION_FOLDER + "signedEncrypted.pdf";
            String fieldName = "Signature1";
            byte[] ownerPass = "World".GetBytes();
            PdfReader reader = new PdfReader(srcFile, new ReaderProperties().SetPassword(ownerPass));
            PdfSigner signer = new PdfSigner(reader, new FileStream(outPdf, FileMode.Create), new StampingProperties()
                .UseAppendMode());
            signer.SetFieldName(fieldName);
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            //Password to open out and cmp files are the same
            ReaderProperties properties = new ReaderProperties().SetPassword(ownerPass);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outPdf, cmpPdf, properties, properties
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SignCertificateSecurityPdfTest() {
            String srcFile = SOURCE_FOLDER + "signCertificateSecurityPdf.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_signCertificateSecurityPdf.pdf";
            String outPdf = DESTINATION_FOLDER + "signCertificateSecurityPdf.pdf";
            PdfReader reader = new PdfReader(srcFile, new ReaderProperties().SetPublicKeySecurityParams(chain[0], pk));
            PdfSigner signer = new PdfSigner(reader, new FileStream(outPdf, FileMode.Create), new StampingProperties()
                .UseAppendMode());
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            ReaderProperties properties = new ReaderProperties().SetPublicKeySecurityParams(chain[0], pk);
            //Public key to open out and cmp files are the same
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outPdf, cmpPdf, properties, properties
                ));
        }
    }
}
