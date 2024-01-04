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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Signatures;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures.Verify.Pdfinsecurity {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class IncrementalSavingAttackTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/verify/pdfinsecurity/IncrementalSavingAttackTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void TestISA03() {
            String filePath = sourceFolder + "isa-3.pdf";
            String signatureName = "Signature1";
            PdfDocument document = new PdfDocument(new PdfReader(filePath));
            SignatureUtil sigUtil = new SignatureUtil(document);
            PdfPKCS7 pdfPKCS7 = sigUtil.ReadSignatureData(signatureName);
            NUnit.Framework.Assert.IsTrue(pdfPKCS7.VerifySignatureIntegrityAndAuthenticity());
            NUnit.Framework.Assert.IsFalse(sigUtil.SignatureCoversWholeDocument(signatureName));
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestISAValidPdf() {
            String filePath = sourceFolder + "isaValidPdf.pdf";
            String signatureName = "Signature1";
            PdfDocument document = new PdfDocument(new PdfReader(filePath));
            SignatureUtil sigUtil = new SignatureUtil(document);
            PdfPKCS7 pdfPKCS7 = sigUtil.ReadSignatureData(signatureName);
            NUnit.Framework.Assert.IsTrue(pdfPKCS7.VerifySignatureIntegrityAndAuthenticity());
            NUnit.Framework.Assert.IsFalse(sigUtil.SignatureCoversWholeDocument(signatureName));
            String textFromPage = PdfTextExtractor.GetTextFromPage(document.GetPage(1));
            // We are working with the latest revision of the document, that's why we should get amended page text.
            // However Signature shall be marked as not covering the complete document, indicating its invalidity
            // for the current revision.
            NUnit.Framework.Assert.AreEqual("This is manipulated malicious text, ha-ha!", textFromPage);
            NUnit.Framework.Assert.AreEqual(2, sigUtil.GetTotalRevisions());
            NUnit.Framework.Assert.AreEqual(1, sigUtil.GetRevision(signatureName));
            Stream sigInputStream = sigUtil.ExtractRevision(signatureName);
            PdfDocument sigRevDocument = new PdfDocument(new PdfReader(sigInputStream));
            SignatureUtil sigRevUtil = new SignatureUtil(sigRevDocument);
            PdfPKCS7 sigRevSignatureData = sigRevUtil.ReadSignatureData(signatureName);
            NUnit.Framework.Assert.IsTrue(sigRevSignatureData.VerifySignatureIntegrityAndAuthenticity());
            NUnit.Framework.Assert.IsTrue(sigRevUtil.SignatureCoversWholeDocument(signatureName));
            sigRevDocument.Close();
            document.Close();
        }
    }
}
