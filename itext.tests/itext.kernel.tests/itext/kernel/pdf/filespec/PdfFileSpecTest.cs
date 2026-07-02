/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf.Filespec {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfFileSpecTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/filespec/PdfFileSpecTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/kernel/pdf/filespec/PdfFileSpecTest/";

        private const int DUPLICATE_ADDS = 2;

        // 1MB payload
        private const int PAYLOAD_BYTES = 1024 * 1024;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.NAME_ALREADY_EXISTS_IN_THE_NAME_TREE, Count = 1)]
        public virtual void CreateEmbeddedFileSpecWithByteArraysTest() {
            String filename = DESTINATION_FOLDER + "byteArrays.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename))) {
                pdfDoc.AddNewPage();
                for (int i = 0; i < DUPLICATE_ADDS; ++i) {
                    byte[] payload = new byte[PAYLOAD_BYTES];
                    // Make first byte indexed so each payload differs (rules out content-based dedup).
                    payload[0] = (byte)(i & 0xFF);
                    PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, payload, "Iteration " + i, "attachment-bytes.bin"
                        , new PdfName("application/octet-stream"), null, PdfName.Data);
                    pdfDoc.AddFileAttachment("attachment-bytes.bin", spec);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, SOURCE_FOLDER + "cmp_byteArrays.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateEmbeddedFileSpecWithFilePathTest() {
            String filename = DESTINATION_FOLDER + "filePaths.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename))) {
                pdfDoc.AddNewPage();
                PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, SOURCE_FOLDER + "attachment-64.txt", "FileSpec Stream Closing Test"
                    , "attachment-64.txt", new PdfName("application/octet-stream"), null, PdfName.Data);
                pdfDoc.AddFileAttachment("attachment-64.txt", spec);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, SOURCE_FOLDER + "cmp_attachment-64.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateEmbeddedFileSpecWithStreamClosingTest() {
            String filename = DESTINATION_FOLDER + "streamClosing.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename))) {
                pdfDoc.AddNewPage();
                using (Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "attachment-64.txt")) {
                    PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, @is, "FileSpec Stream Closing Test", "attachment-64.txt"
                        , new PdfName("application/octet-stream"), null, PdfName.Data);
                    pdfDoc.AddFileAttachment("attachment-64.txt", spec);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, SOURCE_FOLDER + "cmp_attachment-64.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }
    }
}
