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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
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

        public static IList<PdfConformance> Data() {
            return UaValidationTestFramework.GetConformanceList();
        }

        public static Func<Cell> NewHeaderCell(String id, String content, int colspan, int rowspan, String scope) {
            return () => {
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
                    throw new PdfException(e.Message);
                }
            }
            ;
        }

        public static Func<Cell> NewDataCell(String content, int colspan, int rowspan, IList<String> headers) {
            return () => {
                try {
                    Cell cell = new Cell(rowspan, colspan).Add(new Paragraph(content).SetFont(PdfFontFactory.CreateFont(FONT))
                        );
                    if (headers != null) {
                        PdfArray list = new PdfArray();
                        foreach (String header in headers) {
                            list.Add(new PdfString(header));
                        }
                        cell.GetAccessibilityProperties().AddAttributes(new PdfUATableTest.InternalPdfStructureAttributes("Table")
                            .AddPdfObject(PdfName.Headers, new PdfArray(list)));
                    }
                    return cell;
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e.Message);
                }
            }
            ;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithoutHeaders01(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            for (int i = 0; i < 16; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithoutHeaders01");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithoutHeaders02(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddHeaderCell(NewDataCell("Data 1", 1, 1, null));
            }
            for (int i = 0; i < 8; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddFooterCell(NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithoutHeaders02");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn01(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderScopeColumn01");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn02(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddHeaderCell(NewHeaderCell(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(NewHeaderCell(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(NewHeaderCell(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(NewHeaderCell(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderScopeColumn02");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn03(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddFooterCell(NewHeaderCell(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddFooterCell(NewHeaderCell(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddFooterCell(NewHeaderCell(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddFooterCell(NewHeaderCell(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderScopeColumn03");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.LAST_ROW_IS_NOT_COMPLETE, Count = 2)]
        public virtual void TableWithHeaderScopeColumn04(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 4", 1, 1, "Column"));
            // Notice, that body table is not completely filled up
            for (int i = 0; i < 10; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothFail("tableWithHeaderScopeColumn04");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.LAST_ROW_IS_NOT_COMPLETE, Count = 8)]
        public virtual void NotRegularRowGroupingsInTableTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddHeaderCell(NewHeaderCell(null, "Header 1", 2, 1, "Column"));
            tableBuilder.AddHeaderCell(NewHeaderCell(null, "Header 2", 1, 2, "Column"));
            tableBuilder.AddHeaderCell(NewHeaderCell(null, "Header 3", 2, 1, "Column"));
            // Table is not completely filled up
            for (int i = 0; i < 11; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            tableBuilder.AddFooterCell(NewDataCell("Footer 1", 3, 1, null));
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothFail("notRegularRowGroupingsInTable", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                .ROWS_SPAN_DIFFERENT_NUMBER_OF_COLUMNS, 1, 2), false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn05(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 2", 1, 1, "Column"));
            //Colspan
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 3", 2, 1, "Column"));
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderScopeColumn05");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn06(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 2, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 2", 1, 1, "Column"));
            //Colspan
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 3", 1, 1, "Column"));
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderScopeColumn06");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn07(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 4, 1, "Column"));
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderScopeColumn07");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn08(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 2, 1, null));
            }
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderScopeColumn08");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn09(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 4", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 2, 2, null));
            for (int i = 0; i < 6; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderScopeColumn09");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn10(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 3", 2, 1, "Column"));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 2, 2, null));
            for (int i = 0; i < 6; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderScopeColumn10");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn11(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddFooterCell(NewHeaderCell(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddFooterCell(NewHeaderCell(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddFooterCell(NewHeaderCell(null, "Header 3", 2, 1, "Column"));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 2, 2, null));
            for (int i = 0; i < 6; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderScopeColumn11");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn12(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(5);
            for (int i = 0; i < 10; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 2, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 3", 2, 1, "Column"));
            for (int i = 0; i < 10; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderScopeColumn12");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn13(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddHeaderCell(NewHeaderCell(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(NewHeaderCell(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(() => {
                Cell cell = new Cell();
                cell.SetNeutralRole();
                return cell;
            }
            );
            for (int i = 0; i < 9; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothFail("tableWithHeaderScopeColumn13");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn14(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 2", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("tableWithHeaderScopeColumn14");
            }
            else {
                // Rule 8.2.5.26-5 in VeraPDF passes since scope is resolved to default (see Table 384 in ISO 32000-2:2020)
                framework.AssertBothValid("tableWithHeaderScopeColumn14");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn15(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderScopeColumn15");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderScopeColumn16(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            }
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewDataCell("Header 2", 1, 1, null));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 4", 1, 1, "Column"));
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderScopeColumn16");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderRowScope01(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddHeaderCell(NewHeaderCell(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddHeaderCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddHeaderCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddHeaderCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddFooterCell(NewHeaderCell(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddFooterCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddFooterCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddFooterCell(NewDataCell("Data 1", 1, 1, null));
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderRowScope01");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderRowScope02(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 3, 1, null));
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderRowScope02");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderRowScope03(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 3, 1, null));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "Row"));
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderRowScope03");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderRowScope04(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 2, 1, null));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "Row"));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderRowScope04");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderRowScope05(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 4, "Row"));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderRowScope05");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderRowScope06(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 4, "Row"));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            PdfUATableTest.TableBuilder tableBuilder1 = new PdfUATableTest.TableBuilder(3);
            tableBuilder1.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder1.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder1.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 4, "Row"));
            tableBuilder1.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder1.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder1.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder1.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder1.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder1.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            PdfUATableTest.TableBuilder tableBuilder2 = new PdfUATableTest.TableBuilder(3);
            tableBuilder2.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 3, "Row"));
            tableBuilder2.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder2.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder2.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder2.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder2.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder2.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 3, "Row"));
            tableBuilder2.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder2.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder2.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder2.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc(), tableBuilder1.GenerateFunc(), tableBuilder2.GenerateFunc
                ());
            framework.AssertBothValid("tableWithHeaderRowScope06");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderRowScope07(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(() => {
                Cell cell = new Cell();
                return cell.SetNeutralRole();
            }
            );
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothFail("tableWithHeaderRowScope07");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderRowScope08(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderRowScope08");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderRowScope09(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header 1", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data 1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("tableWithHeaderRowScope09");
            }
            else {
                // Rule 8.2.5.26-5 in VeraPDF passes since scope is resolved to default (see Table 384 in ISO 32000-2:2020)
                framework.AssertBothValid("tableWithHeaderRowScope09");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderBothScope01(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header", 3, 1, "Both"));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderBothScope01");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderBothScope02(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header", 1, 1, "Both"));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderBothScope02");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithHeaderBothScope03(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header", 3, 1, "Both"));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithHeaderBothScope03");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId01(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewHeaderCell("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("id2", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("id3", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("tableWithId01");
            }
            else {
                // Rule 8.2.5.26-5 in VeraPDF passes since scope is resolved to default (see Table 384 in ISO 32000-2:2020)
                framework.AssertBothValid("tableWithId01");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId02(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewHeaderCell("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("id3", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("tableWithId02");
            }
            else {
                framework.AssertBothValid("tableWithId02");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId03(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewHeaderCell("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("id2", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("id3", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id2")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id3")));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithId03");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId04(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddHeaderCell(NewHeaderCell("id1", "Header", 1, 1, "None"));
            tableBuilder.AddHeaderCell(NewHeaderCell("id2", "Header", 1, 1, "None"));
            tableBuilder.AddHeaderCell(NewHeaderCell("id3", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id2")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id3")));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithId04");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId05(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddHeaderCell(NewHeaderCell("id1", "Header", 1, 1, "None"));
            tableBuilder.AddHeaderCell(NewHeaderCell("id2", "Header", 1, 1, "None"));
            tableBuilder.AddHeaderCell(NewHeaderCell("id3", "Header", 1, 1, "None"));
            tableBuilder.AddHeaderCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddHeaderCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id2")));
            tableBuilder.AddHeaderCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id3")));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithId05");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId06(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddHeaderCell(NewHeaderCell("id1", "Header", 1, 1, "None"));
            tableBuilder.AddHeaderCell(NewHeaderCell("id2", "Header", 1, 1, "None"));
            tableBuilder.AddHeaderCell(NewHeaderCell("id3", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id2")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id3")));
            tableBuilder.AddFooterCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddFooterCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id2")));
            tableBuilder.AddFooterCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id3")));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithId06");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId07(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id2")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id3")));
            tableBuilder.AddBodyCell(NewHeaderCell("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("id2", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("id3", "Header", 1, 1, "None"));
            tableBuilder.AddFooterCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddFooterCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id2")));
            tableBuilder.AddFooterCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id3")));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithId07");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId08(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id2")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id3")));
            tableBuilder.AddBodyCell(NewHeaderCell("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("id2", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("id3", "Header", 1, 1, "None"));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithId08");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId09(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewHeaderCell("id1", "Header", 3, 1, "None"));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithId09");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId10(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddFooterCell(NewHeaderCell("id1", "Header", 3, 1, "None"));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithId10");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId11(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewHeaderCell("id1", "Header", 1, 3, "None"));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithId11");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId12(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id2")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id3")));
            tableBuilder.AddBodyCell(NewHeaderCell("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("notexisting", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("id3", "Header", 1, 1, "None"));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothFail("tableWithId12");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId13(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id3")));
            tableBuilder.AddBodyCell(NewHeaderCell("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("id3", "Header", 1, 1, "None"));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithId13");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId14(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaUtil.ArraysAsList("id1", "id2")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id2")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id3")));
            tableBuilder.AddBodyCell(NewHeaderCell("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("id2", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("id3", "Header", 1, 1, "None"));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableWithId14");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TableWithId15(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewHeaderCell("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("notexisting", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("id3", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id1")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id2")));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id3")));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("tableWithId15");
            }
            else {
                framework.AssertBothValid("tableWithId15");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void Combination01(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewHeaderCell("id1", "Header1", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("id2", "Header2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell("id3", "Header3", 1, 1, "Row"));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaUtil.ArraysAsList("id1", "id2")));
            tableBuilder.AddBodyCell(NewDataCell("Data2", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data3", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothFail("combination01");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        [LogMessage(PdfUALogMessageConstants.PAGE_FLUSHING_DISABLED, Count = 2)]
        public virtual void Combination02(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            for (int i = 0; i < 201; i++) {
                tableBuilder.AddBodyCell(NewHeaderCell("id" + i, "Header1", 1, 1, "None"));
            }
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("combination02");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        [LogMessage(PdfUALogMessageConstants.PAGE_FLUSHING_DISABLED, Count = 2)]
        public virtual void Combination04(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddHeaderCell(NewDataCell("Data1H", 1, 1, JavaCollectionsUtil.SingletonList("id" + i)));
            }
            for (int i = 0; i < 201; i++) {
                tableBuilder.AddBodyCell(NewHeaderCell("id" + i, "Header1", 1, 1, "None"));
            }
            for (int i = 0; i < 201; i++) {
                tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id" + i)));
            }
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddFooterCell(NewDataCell("Data1F", 1, 1, JavaCollectionsUtil.SingletonList("id" + i)));
            }
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("combination04");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void Combination05(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewHeaderCell("id1", "Header1", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("id2", "Header2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header3", 1, 1, "Row"));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaUtil.ArraysAsList("id1", "id2")));
            tableBuilder.AddBodyCell(NewDataCell("Data2", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data3", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothFail("combination05");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void Combination06(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddHeaderCell(NewHeaderCell("id1", "Header1", 1, 1, "None"));
            tableBuilder.AddHeaderCell(NewHeaderCell("id2", "Header2", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(NewHeaderCell(null, "Header3", 1, 1, "Row"));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaUtil.ArraysAsList("id1", "id2")));
            tableBuilder.AddBodyCell(NewDataCell("Data2", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data3", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothFail("combination06");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void Combination07(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddFooterCell(NewHeaderCell("id1", "Header1", 1, 1, "None"));
            tableBuilder.AddFooterCell(NewHeaderCell("id2", "Header2", 1, 1, "Column"));
            tableBuilder.AddFooterCell(NewHeaderCell(null, "Header3", 1, 1, "Row"));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaUtil.ArraysAsList("id1", "id2")));
            tableBuilder.AddBodyCell(NewDataCell("Data2", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data3", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothFail("combination07");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void Combination08(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddHeaderCell(NewHeaderCell("id1", "Header1", 1, 1, "None"));
            tableBuilder.AddHeaderCell(NewHeaderCell("id2", "Header2", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(NewHeaderCell(null, "Header3", 1, 1, "Row"));
            tableBuilder.AddFooterCell(NewDataCell("Data1", 1, 1, JavaUtil.ArraysAsList("id1", "id2")));
            tableBuilder.AddFooterCell(NewDataCell("Data2", 1, 1, null));
            tableBuilder.AddFooterCell(NewDataCell("Data3", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothFail("combination08");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void Combination09(PdfConformance conformance) {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(NewHeaderCell(null, "Header1", 1, 1, "None"));
            tableBuilder.AddBodyCell(NewHeaderCell("id2", "Header2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewHeaderCell("id3", "Header3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(NewDataCell("Data1", 1, 1, JavaCollectionsUtil.SingletonList("id2")));
            tableBuilder.AddBodyCell(NewDataCell("Data2", 1, 1, null));
            tableBuilder.AddBodyCell(NewDataCell("Data3", 1, 1, null));
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("combination09");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void RoleMapping01(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(2);
            framework.AddBeforeGenerationHook(((pdfDocument) => {
                PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
                root.AddRoleMapping("FancyHeading", StandardRoles.TH);
                root.AddRoleMapping("FancyTD", StandardRoles.TD);
                if (framework.IsPdf2Based(conformance)) {
                    PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0).AddNamespaceRoleMapping("FancyHeading"
                        , StandardRoles.TH).AddNamespaceRoleMapping("FancyTD", StandardRoles.TD);
                    pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(@namespace);
                    pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                }
            }
            ));
            tableBuilder.AddBodyCell(() => {
                Cell c = new Cell();
                c.Add(new Paragraph("Heading 1").SetFont(GetFont()));
                c.GetAccessibilityProperties().SetRole("FancyHeading");
                return c;
            }
            );
            tableBuilder.AddBodyCell(() => {
                Cell c = new Cell();
                c.Add(new Paragraph("Heading 2").SetFont(GetFont()));
                c.GetAccessibilityProperties().SetRole("FancyHeading");
                return c;
            }
            );
            framework.AddSuppliers(tableBuilder.GenerateFunc());
            framework.AssertBothValid("tableCustomRoles");
        }

        private static PdfFont GetFont() {
            try {
                return PdfFontFactory.CreateFont(FONT);
            }
            catch (System.IO.IOException e) {
                throw new PdfException(e);
            }
        }

        public class TableBuilder {
            private readonly int amountOfColumns;

            private readonly IList<Func<Cell>> headerCells = new List<Func<Cell>>();

            private readonly IList<Func<Cell>> bodyCells = new List<Func<Cell>>();

            private readonly IList<Func<Cell>> footerCells = new List<Func<Cell>>();

            public TableBuilder(int amountOfColumns) {
                this.amountOfColumns = amountOfColumns;
            }

            public virtual PdfUATableTest.TableBuilder AddHeaderCell(Func<Cell> sup) {
                this.headerCells.Add(sup);
                return this;
            }

            public virtual PdfUATableTest.TableBuilder AddBodyCell(Func<Cell> sup) {
                this.bodyCells.Add(sup);
                return this;
            }

            public virtual PdfUATableTest.TableBuilder AddFooterCell(Func<Cell> sup) {
                this.footerCells.Add(sup);
                return this;
            }

            public virtual Func<PdfDocument, IBlockElement> GenerateFunc() {
                return ((pdfDocument) => {
                    Table table = new Table(amountOfColumns);
                    foreach (Func<Cell> headerCell in headerCells) {
                        table.AddHeaderCell(headerCell());
                    }
                    foreach (Func<Cell> bodyCell in bodyCells) {
                        table.AddCell(bodyCell());
                    }
                    foreach (Func<Cell> supplier in footerCells) {
                        table.AddFooterCell(supplier());
                    }
                    return table;
                }
                );
            }
        }

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
    }
}
