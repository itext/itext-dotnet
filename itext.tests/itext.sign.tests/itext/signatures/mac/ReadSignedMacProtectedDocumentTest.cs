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
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures.Mac {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class ReadSignedMacProtectedDocumentTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/mac/ReadSignedMacProtectedDocumentTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/mac/ReadSignedMacProtectedDocumentTest/";

        private static readonly byte[] ENCRYPTION_PASSWORD = "123".GetBytes();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void ReadSignedMacProtectedInvalidDocTest() {
            String srcFileName = SOURCE_FOLDER + "signedMacProtectedInvalidDoc.pdf";
            String exceptionMessage = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                using (PdfDocument ignored = new PdfDocument(new PdfReader(srcFileName, new ReaderProperties().SetPassword
                    (ENCRYPTION_PASSWORD)))) {
                }
            }
            ).Message;
            // Do nothing.
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.MAC_VALIDATION_FAILED, exceptionMessage);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void UpdateSignedMacProtectedDocumentTest() {
            String fileName = "updateSignedMacProtectedDocumentTest.pdf";
            String srcFileName = SOURCE_FOLDER + "thirdPartyMacProtectedAndSignedDocument.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            using (PdfDocument ignored = new PdfDocument(new PdfReader(srcFileName, new ReaderProperties().SetPassword
                (ENCRYPTION_PASSWORD)), new PdfWriter(FileUtil.GetFileOutputStream(outputFileName)), new StampingProperties
                ().UseAppendMode())) {
            }
            // Do nothing.
            // This call produces INFO log from AESCipher caused by exception while decrypting. The reason is that,
            // while comparing encrypted signed documents, CompareTool needs to mark signature value as unencrypted.
            // Instead, it tries to decrypt not encrypted value which results in exception.
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpFileName, DESTINATION_FOLDER
                , "diff", ENCRYPTION_PASSWORD, ENCRYPTION_PASSWORD));
        }
    }
}
