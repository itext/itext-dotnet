/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Kernel.Pdf;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class PdfPKCS7BasicTest : ExtendedITextTest {
        protected internal static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/PdfPKCS7Test/";

        protected internal static IX509Certificate[] chain;

        protected internal static IPrivateKey pk;

        protected internal static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator
            .GetFactory();

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Init() {
            pk = PemFileHelper.ReadFirstKey(CERTS_SRC + "signCertRsa01.pem", PASSWORD);
            chain = PemFileHelper.ReadFirstChain(CERTS_SRC + "signCertRsa01.pem");
        }

        internal static void VerifyIsoExtensionExample(String expectedSigMechanismName, String fileName) {
            String infile = SOURCE_FOLDER + "extensions/" + fileName;
            using (PdfReader r = new PdfReader(infile)) {
                using (PdfDocument pdfDoc = new PdfDocument(r)) {
                    SignatureUtil u = new SignatureUtil(pdfDoc);
                    /*
                    We specify the security provider explicitly; we're not testing security provider fallback here.
                    
                    Also, default providers (in 2022) don't always have the parameters for Brainpool curves,
                    but a curve param mismatch doesn't factor into the algorithm support fallback logic, so
                    it causes a runtime error.
                    */
                    PdfPKCS7 data = u.ReadSignatureData("Signature");
                    NUnit.Framework.Assert.AreEqual(expectedSigMechanismName, data.GetSignatureMechanismName());
                    NUnit.Framework.Assert.IsTrue(data.VerifySignatureIntegrityAndAuthenticity());
                }
            }
        }
    }
}
