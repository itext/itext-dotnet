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
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Crypto.Pdfencryption {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfPreserveEncryptionTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/crypto/pdfencryption/PdfPreserveEncryptionTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/crypto/pdfencryption/PdfPreserveEncryptionTest/";

        public PdfEncryptionTestUtils encryptionUtil = new PdfEncryptionTestUtils(destinationFolder, sourceFolder);

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        [LogMessage(VersionConforming.DEPRECATED_ENCRYPTION_ALGORITHMS)]
        public virtual void StampAndUpdateVersionPreserveStandard40() {
            String filename = "stampAndUpdateVersionPreserveStandard40.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "encryptedWithPasswordStandard40.pdf", new 
                ReaderProperties().SetPassword(PdfEncryptionTestUtils.OWNER)), CompareTool.CreateTestPdfWriter(destinationFolder
                 + filename, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)), new StampingProperties().PreserveEncryption
                ());
            doc.Close();
            encryptionUtil.CompareEncryptedPdf(filename);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        [LogMessage(VersionConforming.DEPRECATED_AES256_REVISION)]
        public virtual void StampAndUpdateVersionPreserveAes256() {
            String filename = "stampAndUpdateVersionPreserveAes256.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "encryptedWithPasswordAes256.pdf", new ReaderProperties
                ().SetPassword(PdfEncryptionTestUtils.OWNER)), CompareTool.CreateTestPdfWriter(destinationFolder + filename
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)), new StampingProperties().PreserveEncryption
                ());
            doc.Close();
            encryptionUtil.CompareEncryptedPdf(filename);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptAes256EncryptedStampingPreserve() {
            String filename = "encryptAes256EncryptedStampingPreserve.pdf";
            String src = sourceFolder + "encryptedWithPlainMetadata.pdf";
            String @out = destinationFolder + filename;
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src, new ReaderProperties().SetPassword(PdfEncryptionTestUtils
                .OWNER)), CompareTool.CreateTestPdfWriter(@out, new WriterProperties()), new StampingProperties().PreserveEncryption
                ());
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare();
            String compareResult = compareTool.CompareByContent(@out, sourceFolder + "cmp_" + filename, destinationFolder
                , "diff_", PdfEncryptionTestUtils.USER, PdfEncryptionTestUtils.USER);
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PreserveEncryptionShorterDocumentId() {
            String filename = "preserveEncryptionWithShortId.pdf";
            String src = sourceFolder + "encryptedWithShortId.pdf";
            String @out = destinationFolder + filename;
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src, new ReaderProperties().SetPassword(PdfEncryptionTestUtils
                .OWNER)), new PdfWriter(@out, new WriterProperties()), new StampingProperties().PreserveEncryption());
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare();
            String compareResult = compareTool.CompareByContent(@out, sourceFolder + "cmp_" + filename, destinationFolder
                , "diff_", null, null);
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }
    }
}
