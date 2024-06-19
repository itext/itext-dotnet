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
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Pdfua.Checkers.Utils;

namespace iText.Pdfua.Checkers.Utils.Tables {
//\cond DO_NOT_DOCUMENT
    /// <summary>Class that iterates over the cells of a table.</summary>
    internal sealed class TableCellIterator : ITableIterator<Cell> {
//\cond DO_NOT_DOCUMENT
        internal readonly PdfUAValidationContext context;
//\endcond

        private IList<IElement> children;

        private int index;

        private iText.Pdfua.Checkers.Utils.Tables.TableCellIterator headerIterator;

        private iText.Pdfua.Checkers.Utils.Tables.TableCellIterator footerIterator;

        private Table table;

        private PdfName location;

        private Cell currentCell;

        /// <summary>
        /// Creates a new
        /// <see cref="TableCellIterator"/>
        /// instance.
        /// </summary>
        /// <param name="table">the table that will be iterated.</param>
        /// <param name="context">the validation context.</param>
        public TableCellIterator(Table table, PdfUAValidationContext context) {
            this.context = context;
            if (table == null) {
                return;
            }
            this.table = table;
            this.children = table.GetChildren();
            headerIterator = new iText.Pdfua.Checkers.Utils.Tables.TableCellIterator(table.GetHeader(), context);
            footerIterator = new iText.Pdfua.Checkers.Utils.Tables.TableCellIterator(table.GetFooter(), context);
        }

        /// <summary><inheritDoc/></summary>
        public bool HasNext() {
            if (headerIterator != null && headerIterator.HasNext()) {
                return true;
            }
            if (children != null && index < children.Count) {
                return true;
            }
            if (footerIterator != null && footerIterator.HasNext()) {
                return true;
            }
            return false;
        }

        /// <summary><inheritDoc/></summary>
        public Cell Next() {
            if (headerIterator != null && headerIterator.HasNext()) {
                location = PdfName.THead;
                currentCell = headerIterator.Next();
                return currentCell;
            }
            if (children != null && index < children.Count) {
                location = PdfName.TBody;
                currentCell = (Cell)children[index++];
                return currentCell;
            }
            if (footerIterator != null && footerIterator.HasNext()) {
                location = PdfName.TFoot;
                currentCell = footerIterator.Next();
                return currentCell;
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public int GetAmountOfRowsBody() {
            return table.GetNumberOfRows();
        }

        /// <summary><inheritDoc/></summary>
        public int GetAmountOfRowsHeader() {
            if (table.GetHeader() != null) {
                return table.GetHeader().GetNumberOfRows();
            }
            return 0;
        }

        /// <summary><inheritDoc/></summary>
        public int GetAmountOfRowsFooter() {
            if (this.table.GetFooter() != null) {
                return this.table.GetFooter().GetNumberOfRows();
            }
            return 0;
        }

        /// <summary><inheritDoc/></summary>
        public int GetNumberOfColumns() {
            return this.table.GetNumberOfColumns();
        }

        /// <summary><inheritDoc/></summary>
        public int GetRow() {
            PdfName location = GetLocation();
            int row = currentCell.GetRow();
            if (location == PdfName.TBody) {
                row += this.GetAmountOfRowsHeader();
            }
            if (location == PdfName.TFoot) {
                row += this.GetAmountOfRowsHeader();
                row += this.GetAmountOfRowsBody();
            }
            return row;
        }

        /// <summary><inheritDoc/></summary>
        public int GetCol() {
            return currentCell.GetCol();
        }

        /// <summary><inheritDoc/></summary>
        public int GetRowspan() {
            return currentCell.GetRowspan();
        }

        /// <summary><inheritDoc/></summary>
        public int GetColspan() {
            return currentCell.GetColspan();
        }

        private PdfName GetLocation() {
            return this.location;
        }
    }
//\endcond
}
