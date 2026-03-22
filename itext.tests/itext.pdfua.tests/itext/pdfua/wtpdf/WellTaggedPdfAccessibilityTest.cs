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
using iText.Kernel.Pdf;
using iText.Pdfua;
using iText.Pdfua.Checkers;
using iText.Pdfua.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfua.Wtpdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class WellTaggedPdfAccessibilityTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/wtpdf/WellTaggedPdfAccessibilityTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/wtpdf" + "/WellTaggedPdfAccessibilityTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfUALogMessageConstants.PDF_TO_WTPDF_CONVERSION_IS_NOT_SUPPORTED, LogLevel = LogLevelConstants
            .WARN)]
        public virtual void OpenNotWellTaggedPdfDocumentTest() {
            NUnit.Framework.Assert.DoesNotThrow(() => new WellTaggedPdfDocument(new PdfReader(SOURCE_FOLDER + "usualPdf.pdf"
                ), new PdfWriter(new MemoryStream()), new WellTaggedPdfConfig(WellTaggedPdfConformance.FOR_ACCESSIBILITY
                , "simple doc", "eng")));
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfUALogMessageConstants.WRITER_PROPERTIES_PDF_VERSION_WAS_OVERRIDDEN, LogLevel = LogLevelConstants
            .WARN)]
        public virtual void SettingWrongPdfVersionTest() {
            using (WellTaggedPdfDocument doc = new WellTaggedPdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_1_4)), new WellTaggedPdfConfig(WellTaggedPdfConformance.FOR_ACCESSIBILITY
                , "en-us", "title"))) {
                NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_2_0, doc.GetPdfVersion());
            }
        }

        [NUnit.Framework.Test]
        public virtual void WellTaggedPdfTableAccesibilityTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, PdfConformance.WELL_TAGGED_PDF_FOR_REUSE
                );
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(PdfUATableTest.NewHeaderCell(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(PdfUATableTest.NewHeaderCell(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(PdfUATableTest.NewHeaderCell(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(PdfUATableTest.NewHeaderCell(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddBodyCell(PdfUATableTest.NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("wellTaggedPdfTableTest");
        }

        [NUnit.Framework.Test]
        public virtual void WellTaggedPdfTableReuseTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, PdfConformance.WELL_TAGGED_PDF_FOR_ACCESSIBILITY
                );
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(PdfUATableTest.NewHeaderCell(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(PdfUATableTest.NewHeaderCell(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(PdfUATableTest.NewHeaderCell(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(PdfUATableTest.NewHeaderCell(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddBodyCell(PdfUATableTest.NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("wellTaggedPdfTableTest");
        }
    }
}
