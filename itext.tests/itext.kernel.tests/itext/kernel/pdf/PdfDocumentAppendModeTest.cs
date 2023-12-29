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
using iText.Kernel.Logs;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfDocumentAppendModeTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfDocumentAppendModeTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfDocumentAppendModeTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUp() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.FULL_COMPRESSION_APPEND_MODE_XREF_TABLE_INCONSISTENCY)]
        public virtual void TestAppendModeWithFullCompressionRequestedWhenOriginalDocumentHasXrefTable() {
            String inFile = SOURCE_FOLDER + "documentWithXrefTable.pdf";
            String outFile = DESTINATION_FOLDER + "documentWithXrefTableAfterAppending.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_documentWithXrefTableAfterAppending.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFile), CompareTool.CreateTestPdfWriter(outFile, 
                new WriterProperties().SetFullCompressionMode(true)), new StampingProperties().UseAppendMode());
            pdfDocument.AddNewPage();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.FULL_COMPRESSION_APPEND_MODE_XREF_STREAM_INCONSISTENCY)]
        public virtual void TestAppendModeWithFullCompressionSetToFalseWhenOriginalDocumentHasXrefStream() {
            String inFile = SOURCE_FOLDER + "documentWithXrefStream.pdf";
            String outFile = DESTINATION_FOLDER + "documentWithXrefStreamAfterAppending.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_documentWithXrefStreamAfterAppending.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFile), CompareTool.CreateTestPdfWriter(outFile, 
                new WriterProperties().SetFullCompressionMode(false)), new StampingProperties().UseAppendMode());
            pdfDocument.AddNewPage();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, DESTINATION_FOLDER));
        }
    }
}
