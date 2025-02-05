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
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Test;

namespace iText.Signatures.Verify.Pdfinsecurity {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class SignatureWrappingAttackTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/verify/pdfinsecurity/SignatureWrappingAttackTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.Test]
        public virtual void TestSWA01() {
            String filePath = sourceFolder + "siwa.pdf";
            String signatureName = "Signature1";
            PdfDocument document = new PdfDocument(new PdfReader(filePath));
            SignatureUtil sigUtil = new SignatureUtil(document);
            PdfPKCS7 pdfPKCS7 = sigUtil.ReadSignatureData(signatureName);
            NUnit.Framework.Assert.IsTrue(pdfPKCS7.VerifySignatureIntegrityAndAuthenticity());
            NUnit.Framework.Assert.IsFalse(sigUtil.SignatureCoversWholeDocument(signatureName));
            document.Close();
        }
    }
}
