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
using NUnit.Framework;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Crypto {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class EncryptionInApprovedModeTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/crypto" + "/EncryptionInApprovedModeTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/crypto" + "/EncryptionInApprovedModeTest/";

        /// <summary>User password.</summary>
        public static byte[] USER = "Hello".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);

        /// <summary>Owner password.</summary>
        public static byte[] OWNER = "World".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            NUnit.Framework.Assume.That(FACTORY.IsInApprovedOnlyMode());
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT)]
        public virtual void CheckMD5LogMessageWhileReadingPdfTest() {
            String fileName = "checkMD5LogMessageWhileReadingPdf.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + fileName))) {
            }
        }

        // this test checks log message
        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT)]
        public virtual void CheckMD5LogMessageWhileCreatingPdfTest() {
            String fileName = "checkMD5LogMessageWhileCreatingPdf.pdf";
            using (PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + fileName, new WriterProperties
                ().SetStandardEncryption(USER, OWNER, EncryptionConstants.ALLOW_SCREENREADERS, EncryptionConstants.ENCRYPTION_AES_256
                ).AddXmpMetadata()))) {
            }
        }

        // this test checks log message
        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Count = 3)]
        public virtual void CheckMD5LogMessageForEachPdfTest() {
            String fileName = "checkMD5LogMessageForEachPdf.pdf";
            for (int i = 0; i < 3; ++i) {
                using (PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + fileName, new WriterProperties
                    ().SetStandardEncryption(USER, OWNER, EncryptionConstants.ALLOW_SCREENREADERS, EncryptionConstants.ENCRYPTION_AES_256
                    ).AddXmpMetadata()))) {
                }
            }
        }
        // this test checks log message
    }
}
