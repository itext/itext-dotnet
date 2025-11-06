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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Layout.Element;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Pdfua.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUATableTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUATableTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<PdfUAConformance> Data() {
            return UaValidationTestFramework.GetConformanceList();
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithoutHeaders01(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            for (int i = 0; i < 16; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithoutHeaders01", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithoutHeaders02(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddHeaderCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            for (int i = 0; i < 8; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddFooterCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithoutHeaders02", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn01(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn01", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn02(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn02", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn03(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn03", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.LAST_ROW_IS_NOT_COMPLETE, Count = 2)]
        public virtual void TableWithHeaderScopeColumn04(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 4", 1, 1, "Column"));
            // Notice, that body table is not completely filled up
            for (int i = 0; i < 10; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("tableWithHeaderScopeColumn04", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.LAST_ROW_IS_NOT_COMPLETE, Count = 8)]
        public virtual void NotRegularRowGroupingsInTableTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 2, 1, "Column"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 2, "Column"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 2, 1, "Column"));
            // Table is not completely filled up
            for (int i = 0; i < 11; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            tableBuilder.AddFooterCell(new PdfUATableTest.DataCellSupplier("Footer 1", 3, 1, null));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("notRegularRowGroupingsInTable", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                .ROWS_SPAN_DIFFERENT_NUMBER_OF_COLUMNS, 1, 2), false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn05(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            //Colspan
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 2, 1, "Column"));
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn05", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn06(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 2, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            //Colspan
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 1, 1, "Column"));
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn06", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn07(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 4, 1, "Column"));
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn07", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn08(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 2, 1, null));
            }
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn08", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn09(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 4", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 2, 2, null));
            for (int i = 0; i < 6; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn09", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn10(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 2, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 2, 2, null));
            for (int i = 0; i < 6; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn10", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn11(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 2, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 2, 2, null));
            for (int i = 0; i < 6; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn11", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn12(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(5);
            for (int i = 0; i < 10; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 2, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 2, 1, "Column"));
            for (int i = 0; i < 10; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn12", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn13(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(new _Generator_355());
            for (int i = 0; i < 9; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("tableWithHeaderScopeColumn13", pdfUAConformance);
        }

        private sealed class _Generator_355 : UaValidationTestFramework.Generator<Cell> {
            public _Generator_355() {
            }

            public Cell Generate() {
                Cell cell = new Cell();
                cell.SetNeutralRole();
                return cell;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn14(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            if (PdfUAConformance.PDF_UA_1 == pdfUAConformance) {
                framework.AssertBothFail("tableWithHeaderScopeColumn14", pdfUAConformance);
            }
            if (PdfUAConformance.PDF_UA_2 == pdfUAConformance) {
                // Rule 8.2.5.26-5 in VeraPDF passes since scope is resolved to default (see Table 384 in ISO 32000-2:2020)
                framework.AssertBothValid("tableWithHeaderScopeColumn14", pdfUAConformance);
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn15(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn15", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn16(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Header 2", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 4", 1, 1, "Column"));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn16", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderRowScope01(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddHeaderCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddHeaderCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddFooterCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddFooterCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddFooterCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderRowScope01", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderRowScope02(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 3, 1, null));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderRowScope02", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderRowScope03(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 3, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderRowScope03", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderRowScope04(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 2, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderRowScope04", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderRowScope05(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 4, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderRowScope05", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderRowScope06(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 4, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            PdfUATableTest.TableBuilder tableBuilder1 = new PdfUATableTest.TableBuilder(3);
            tableBuilder1.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder1.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder1.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 4, "Row"));
            tableBuilder1.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder1.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder1.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder1.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder1.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder1.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            PdfUATableTest.TableBuilder tableBuilder2 = new PdfUATableTest.TableBuilder(3);
            tableBuilder2.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 3, "Row"));
            tableBuilder2.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder2.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder2.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder2.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder2.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder2.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 3, "Row"));
            tableBuilder2.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder2.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder2.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder2.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder, tableBuilder1, tableBuilder2);
            framework.AssertBothValid("tableWithHeaderRowScope06", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderRowScope07(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new _Generator_573());
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("tableWithHeaderRowScope07", pdfUAConformance);
        }

        private sealed class _Generator_573 : UaValidationTestFramework.Generator<Cell> {
            public _Generator_573() {
            }

            public Cell Generate() {
                Cell cell = new Cell();
                return cell.SetNeutralRole();
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderRowScope08(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderRowScope08", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderRowScope09(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            if (PdfUAConformance.PDF_UA_1 == pdfUAConformance) {
                framework.AssertBothFail("tableWithHeaderRowScope09", pdfUAConformance);
            }
            if (PdfUAConformance.PDF_UA_2 == pdfUAConformance) {
                // Rule 8.2.5.26-5 in VeraPDF passes since scope is resolved to default (see Table 384 in ISO 32000-2:2020)
                framework.AssertBothValid("tableWithHeaderRowScope09", pdfUAConformance);
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderBothScope01(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header", 3, 1, "Both"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderBothScope01", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderBothScope02(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header", 1, 1, "Both"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderBothScope02", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderBothScope03(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header", 3, 1, "Both"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderBothScope03", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId01(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id3", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            if (PdfUAConformance.PDF_UA_1 == pdfUAConformance) {
                framework.AssertBothFail("tableWithId01", pdfUAConformance);
            }
            if (PdfUAConformance.PDF_UA_2 == pdfUAConformance) {
                // Rule 8.2.5.26-5 in VeraPDF passes since scope is resolved to default (see Table 384 in ISO 32000-2:2020)
                framework.AssertBothValid("tableWithId01", pdfUAConformance);
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId02(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id3", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            if (PdfUAConformance.PDF_UA_1 == pdfUAConformance) {
                framework.AssertBothFail("tableWithId02", pdfUAConformance);
            }
            if (PdfUAConformance.PDF_UA_2 == pdfUAConformance) {
                // Rule 8.2.5.26-5 in VeraPDF passes since scope is resolved to default (see Table 384 in ISO 32000-2:2020)
                framework.AssertBothValid("tableWithId02", pdfUAConformance);
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId03(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id3", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id2")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id3")));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId03", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId04(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 1, 1, "None"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header", 1, 1, "None"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier("id3", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id2")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id3")));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId04", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId05(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 1, 1, "None"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header", 1, 1, "None"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier("id3", "Header", 1, 1, "None"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddHeaderCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id2")));
            tableBuilder.AddHeaderCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id3")));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId05", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId06(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 1, 1, "None"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header", 1, 1, "None"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier("id3", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id2")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id3")));
            tableBuilder.AddFooterCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddFooterCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id2")));
            tableBuilder.AddFooterCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id3")));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId06", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId07(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id2")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id3")));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id3", "Header", 1, 1, "None"));
            tableBuilder.AddFooterCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddFooterCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id2")));
            tableBuilder.AddFooterCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id3")));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId07", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId08(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id2")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id3")));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id3", "Header", 1, 1, "None"));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId08", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId09(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 3, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId09", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId10(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 3, 1, "None"));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId10", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId11(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 1, 3, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId11", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId12(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id2")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id3")));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("notexisting", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id3", "Header", 1, 1, "None"));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("tableWithId12", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId13(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id3")));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id3", "Header", 1, 1, "None"));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId13", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId14(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaUtil.ArraysAsList("id1", "id2"
                )));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id2")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id3")));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id3", "Header", 1, 1, "None"));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId14", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId15(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("notexisting", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id3", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id2")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id3")));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            if (PdfUAConformance.PDF_UA_1 == pdfUAConformance) {
                framework.AssertBothFail("tableWithId15", pdfUAConformance);
            }
            if (PdfUAConformance.PDF_UA_2 == pdfUAConformance) {
                framework.AssertBothValid("tableWithId15", pdfUAConformance);
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void Combination01(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header1", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id3", "Header3", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaUtil.ArraysAsList("id1", "id2"
                )));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data2", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data3", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("combination01", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        [LogMessage(PdfUALogMessageConstants.PAGE_FLUSHING_DISABLED, Count = 2)]
        public virtual void Combination02(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            for (int i = 0; i < 201; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id" + i, "Header1", 1, 1, "None"));
            }
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("combination02", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        [LogMessage(PdfUALogMessageConstants.PAGE_FLUSHING_DISABLED, Count = 2)]
        public virtual void Combination04(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddHeaderCell(new PdfUATableTest.DataCellSupplier("Data1H", 1, 1, JavaCollectionsUtil.SingletonList
                    ("id" + i)));
            }
            for (int i = 0; i < 201; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id" + i, "Header1", 1, 1, "None"));
            }
            for (int i = 0; i < 201; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                    ("id" + i)));
            }
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddFooterCell(new PdfUATableTest.DataCellSupplier("Data1F", 1, 1, JavaCollectionsUtil.SingletonList
                    ("id" + i)));
            }
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("combination04", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void Combination05(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header1", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header3", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaUtil.ArraysAsList("id1", "id2"
                )));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data2", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data3", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("combination05", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void Combination06(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header1", 1, 1, "None"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header2", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header3", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaUtil.ArraysAsList("id1", "id2"
                )));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data2", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data3", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("combination06", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void Combination07(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header1", 1, 1, "None"));
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header2", 1, 1, "Column"));
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier(null, "Header3", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaUtil.ArraysAsList("id1", "id2"
                )));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data2", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data3", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("combination07", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void Combination08(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header1", 1, 1, "None"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header2", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header3", 1, 1, "Row"));
            tableBuilder.AddFooterCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaUtil.ArraysAsList("id1", 
                "id2")));
            tableBuilder.AddFooterCell(new PdfUATableTest.DataCellSupplier("Data2", 1, 1, null));
            tableBuilder.AddFooterCell(new PdfUATableTest.DataCellSupplier("Data3", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("combination08", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void Combination09(PdfUAConformance pdfUAConformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header1", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id3", "Header3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id2")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data2", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data3", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("combination09", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void RoleMapping01(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(2);
            framework.AddBeforeGenerationHook(((pdfDocument) => {
                PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
                root.AddRoleMapping("FancyHeading", StandardRoles.TH);
                root.AddRoleMapping("FancyTD", StandardRoles.TD);
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0).AddNamespaceRoleMapping("FancyHeading"
                        , StandardRoles.TH).AddNamespaceRoleMapping("FancyTD", StandardRoles.TD);
                    pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(@namespace);
                    pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                }
            }
            ));
            tableBuilder.AddBodyCell(new _Generator_1134());
            tableBuilder.AddBodyCell(new _Generator_1144());
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableCustomRoles", pdfUAConformance);
        }

        private sealed class _Generator_1134 : UaValidationTestFramework.Generator<Cell> {
            public _Generator_1134() {
            }

            public Cell Generate() {
                Cell c = new Cell();
                c.Add(new Paragraph("Heading 1").SetFont(PdfUATableTest.GetFont()));
                c.GetAccessibilityProperties().SetRole("FancyHeading");
                return c;
            }
        }

        private sealed class _Generator_1144 : UaValidationTestFramework.Generator<Cell> {
            public _Generator_1144() {
            }

            public Cell Generate() {
                Cell c = new Cell();
                c.Add(new Paragraph("Heading 2").SetFont(PdfUATableTest.GetFont()));
                c.GetAccessibilityProperties().SetRole("FancyHeading");
                return c;
            }
        }

//\cond DO_NOT_DOCUMENT
        internal class TableBuilder : UaValidationTestFramework.Generator<IBlockElement> {
            private readonly int amountOfColumns;

            private readonly IList<UaValidationTestFramework.Generator<Cell>> headerCells = new List<UaValidationTestFramework.Generator
                <Cell>>();

            private readonly IList<UaValidationTestFramework.Generator<Cell>> bodyCells = new List<UaValidationTestFramework.Generator
                <Cell>>();

            private readonly IList<UaValidationTestFramework.Generator<Cell>> footerCells = new List<UaValidationTestFramework.Generator
                <Cell>>();

//\cond DO_NOT_DOCUMENT
            internal TableBuilder(int amountOfColumns) {
                this.amountOfColumns = amountOfColumns;
            }
//\endcond

            public virtual PdfUATableTest.TableBuilder AddHeaderCell(UaValidationTestFramework.Generator<Cell> sup) {
                this.headerCells.Add(sup);
                return this;
            }

            public virtual PdfUATableTest.TableBuilder AddBodyCell(UaValidationTestFramework.Generator<Cell> sup) {
                this.bodyCells.Add(sup);
                return this;
            }

            public virtual PdfUATableTest.TableBuilder AddFooterCell(UaValidationTestFramework.Generator<Cell> sup) {
                this.footerCells.Add(sup);
                return this;
            }

            public virtual IBlockElement Generate() {
                Table table = new Table(amountOfColumns);
                foreach (UaValidationTestFramework.Generator<Cell> headerCell in headerCells) {
                    table.AddHeaderCell(headerCell.Generate());
                }
                foreach (UaValidationTestFramework.Generator<Cell> bodyCell in bodyCells) {
                    table.AddCell(bodyCell.Generate());
                }
                foreach (UaValidationTestFramework.Generator<Cell> supplier in footerCells) {
                    table.AddFooterCell(supplier.Generate());
                }
                return table;
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal class DataCellSupplier : UaValidationTestFramework.Generator<Cell> {
            private readonly String content;

            private readonly int colspan;

            private readonly int rowspan;

            private readonly IList<String> headers;

            public DataCellSupplier(String content, int colspan, int rowspan, IList<String> headers) {
                this.content = content;
                this.colspan = colspan;
                this.rowspan = rowspan;
                this.headers = headers;
            }

            public virtual Cell Generate() {
                try {
                    Cell cell = new Cell(rowspan, colspan).Add(new Paragraph(content).SetFont(PdfFontFactory.CreateFont(FONT))
                        );
                    if (headers != null) {
                        PdfArray headers = new PdfArray();
                        foreach (String header in this.headers) {
                            headers.Add(new PdfString(header));
                        }
                        cell.GetAccessibilityProperties().AddAttributes(new PdfUATableTest.InternalPdfStructureAttributes("Table")
                            .AddPdfObject(PdfName.Headers, new PdfArray(headers)));
                    }
                    return cell;
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal class InternalPdfStructureAttributes : PdfStructureAttributes {
            public InternalPdfStructureAttributes(String owner)
                : base(owner) {
            }

            public virtual PdfStructureAttributes AddPdfObject(PdfName headers, PdfArray pdfObjects) {
                GetPdfObject().Put(headers, pdfObjects);
                SetModified();
                return this;
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal class HeaderCellSupplier : UaValidationTestFramework.Generator<Cell> {
            private readonly String id;

            private readonly String content;

            private readonly int colspan;

            private readonly int rowspan;

            private readonly String scope;

            public HeaderCellSupplier(String id, String content, int colspan, int rowspan, String scope) {
                this.id = id;
                this.content = content;
                this.colspan = colspan;
                this.rowspan = rowspan;
                this.scope = scope;
            }

            public virtual Cell Generate() {
                try {
                    Cell cell = new Cell(rowspan, colspan).Add(new Paragraph(content).SetFont(PdfFontFactory.CreateFont(FONT))
                        );
                    cell.GetAccessibilityProperties().SetRole(StandardRoles.TH);
                    if (scope != null) {
                        cell.GetAccessibilityProperties().AddAttributes(new PdfStructureAttributes("Table").AddEnumAttribute("Scope"
                            , scope));
                    }
                    if (id != null) {
                        cell.GetAccessibilityProperties().SetStructureElementIdString(id);
                    }
                    return cell;
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
            }
        }
//\endcond

        private static PdfFont GetFont() {
            try {
                return PdfFontFactory.CreateFont(FONT);
            }
            catch (System.IO.IOException) {
                throw new Exception();
            }
        }
    }
}
