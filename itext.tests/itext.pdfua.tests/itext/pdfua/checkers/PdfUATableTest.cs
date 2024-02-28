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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Layout.Element;
using iText.Pdfua;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUATableTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUATableTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        private UaValidationTestFramework framework;

        [NUnit.Framework.SetUp]
        public virtual void InitializeFramework() {
            framework = new UaValidationTestFramework(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithoutHeaders01() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            for (int i = 0; i < 16; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithoutHeaders01");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithoutHeaders02() {
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
            framework.AssertBothValid("tableWithoutHeaders02");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderScopeColumn01() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn01");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderScopeColumn02() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn02");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderScopeColumn03() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn03");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.LAST_ROW_IS_NOT_COMPLETE, Count = 2)]
        public virtual void TableWithHeaderScopeColumn04() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 4", 1, 1, "Column"));
            //notice body table is not completly filled up
            for (int i = 0; i < 10; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("tableWithHeaderScopeColumn04");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderScopeColumn05() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            //Colspan
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 2, 1, "Column"));
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn05");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderScopeColumn06() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 2, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            //Colspan
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 1, 1, "Column"));
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn06");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderScopeColumn07() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 4, 1, "Column"));
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn07");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderScopeColumn08() {
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
            framework.AssertBothValid("tableWithHeaderScopeColumn08");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderScopeColumn09() {
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
            framework.AssertBothValid("tableWithHeaderScopeColumn09");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderScopeColumn10() {
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
            framework.AssertBothValid("tableWithHeaderScopeColumn10");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderScopeColumn11() {
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
            framework.AssertBothValid("tableWithHeaderScopeColumn11");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderScopeColumn12() {
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
            framework.AssertBothValid("tableWithHeaderScopeColumn12");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderScopeColumn13() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(new _Generator_301());
            for (int i = 0; i < 9; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("tableWithHeaderScopeColumn13");
        }

        private sealed class _Generator_301 : UaValidationTestFramework.Generator<Cell> {
            public _Generator_301() {
            }

            public Cell Generate() {
                Cell cell = new Cell();
                cell.SetNeutralRole();
                return cell;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderScopeColumn14() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 2", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("tableWithHeaderScopeColumn14");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderScopeColumn15() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 4", 1, 1, "Column"));
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn15");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderScopeColumn16() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            for (int i = 0; i < 4; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            }
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Header 2", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 4", 1, 1, "Column"));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderScopeColumn16");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderRowScope01() {
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
            framework.AssertBothValid("tableWithHeaderRowScope01");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderRowScope02() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 3, 1, null));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderRowScope02");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderRowScope03() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 3, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderRowScope03");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderRowScope04() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(4);
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 2, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderRowScope04");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderRowScope05() {
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
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderRowScope05");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderRowScope06() {
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
            framework.AddSuppliers(tableBuilder, tableBuilder1, tableBuilder2);
            framework.AssertBothValid("tableWithHeaderRowScope06");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderRowScope07() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new _Generator_505());
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("tableWithHeaderRowScope07");
        }

        private sealed class _Generator_505 : UaValidationTestFramework.Generator<Cell> {
            public _Generator_505() {
            }

            public Cell Generate() {
                Cell cell = new Cell();
                return cell.SetNeutralRole();
            }
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderRowScope08() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderRowScope08");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderRowScope09() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header 1", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data 1", 1, 1, null));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("tableWithHeaderRowScope09");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderBothScope01() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header", 3, 1, "Both"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderBothScope01");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderBothScope02() {
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
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderBothScope02");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderBothScope03() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header", 3, 1, "Both"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithHeaderBothScope03");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithId01() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id3", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("tableWithId01");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithId02() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id3", "Header", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, null));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("tableWithId02");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithId03() {
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
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId03");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithId04() {
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
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId04");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithId05() {
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
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId05");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithId06() {
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
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId06");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithId07() {
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
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId07");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithId08() {
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
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId08");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithId09() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 3, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId09");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithId10() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id1")));
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header", 3, 1, "None"));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId10");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithId11() {
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
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId11");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithId12() {
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
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("tableWithId12");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithId13() {
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
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId13");
        }

        [NUnit.Framework.Test]
        public virtual void TableWithId14() {
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
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("tableWithId14");
        }

        [NUnit.Framework.Test]
        public virtual void Combination01() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header1", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id3", "Header3", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaUtil.ArraysAsList("id1", "id2"
                )));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data2", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data3", 1, 1, null));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("combination01");
        }

        [NUnit.Framework.Test]
        public virtual void Combination02() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            for (int i = 0; i < 201; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id" + i, "Header1", 1, 1, "None"));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("combination02");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.NAME_ALREADY_EXISTS_IN_THE_NAME_TREE, Count = 22)]
        public virtual void Combination03() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            for (int i = 0; i < 12; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id", "Header1", 1, 1, "None"));
            }
            for (int i = 0; i < 3; i++) {
                tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                    ("id")));
            }
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("combination03");
        }

        [NUnit.Framework.Test]
        public virtual void Combination04() {
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
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("combination04");
        }

        [NUnit.Framework.Test]
        public virtual void Combination05() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header1", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header3", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaUtil.ArraysAsList("id1", "id2"
                )));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data2", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data3", 1, 1, null));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("combination05");
        }

        [NUnit.Framework.Test]
        public virtual void Combination06() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header1", 1, 1, "None"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header2", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header3", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaUtil.ArraysAsList("id1", "id2"
                )));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data2", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data3", 1, 1, null));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("combination06");
        }

        [NUnit.Framework.Test]
        public virtual void Combination07() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header1", 1, 1, "None"));
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header2", 1, 1, "Column"));
            tableBuilder.AddFooterCell(new PdfUATableTest.HeaderCellSupplier(null, "Header3", 1, 1, "Row"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaUtil.ArraysAsList("id1", "id2"
                )));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data2", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data3", 1, 1, null));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("combination07");
        }

        [NUnit.Framework.Test]
        public virtual void Combination08() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier("id1", "Header1", 1, 1, "None"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header2", 1, 1, "Column"));
            tableBuilder.AddHeaderCell(new PdfUATableTest.HeaderCellSupplier(null, "Header3", 1, 1, "Row"));
            tableBuilder.AddFooterCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaUtil.ArraysAsList("id1", 
                "id2")));
            tableBuilder.AddFooterCell(new PdfUATableTest.DataCellSupplier("Data2", 1, 1, null));
            tableBuilder.AddFooterCell(new PdfUATableTest.DataCellSupplier("Data3", 1, 1, null));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothFail("combination08");
        }

        [NUnit.Framework.Test]
        public virtual void Combination09() {
            PdfUATableTest.TableBuilder tableBuilder = new PdfUATableTest.TableBuilder(3);
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier(null, "Header1", 1, 1, "None"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id2", "Header2", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.HeaderCellSupplier("id3", "Header3", 1, 1, "Column"));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data1", 1, 1, JavaCollectionsUtil.SingletonList
                ("id2")));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data2", 1, 1, null));
            tableBuilder.AddBodyCell(new PdfUATableTest.DataCellSupplier("Data3", 1, 1, null));
            framework.AddSuppliers(tableBuilder);
            framework.AssertBothValid("combination09");
        }

        internal class TableBuilder : UaValidationTestFramework.Generator<IBlockElement> {
            private readonly int amountOfColumns;

            private readonly IList<UaValidationTestFramework.Generator<Cell>> headerCells = new List<UaValidationTestFramework.Generator
                <Cell>>();

            private readonly IList<UaValidationTestFramework.Generator<Cell>> bodyCells = new List<UaValidationTestFramework.Generator
                <Cell>>();

            private readonly IList<UaValidationTestFramework.Generator<Cell>> footerCells = new List<UaValidationTestFramework.Generator
                <Cell>>();

            internal TableBuilder(int amountOfColumns) {
                this.amountOfColumns = amountOfColumns;
            }

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
    }
}
