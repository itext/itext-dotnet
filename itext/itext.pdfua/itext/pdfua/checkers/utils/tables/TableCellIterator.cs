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

namespace iText.Pdfua.Checkers.Utils.Tables {
    /// <summary>Class that iterates over the cells of a table.</summary>
    internal sealed class TableCellIterator : ITableIterator<Cell> {
        private IList<IElement> children;

        private int index;

        private iText.Pdfua.Checkers.Utils.Tables.TableCellIterator headerIterator;

        private iText.Pdfua.Checkers.Utils.Tables.TableCellIterator footerIterator;

        private Table table;

        private PdfName location;

        /// <summary>
        /// Creates a new
        /// <see cref="TableCellIterator"/>
        /// instance.
        /// </summary>
        /// <param name="table">the table that will be iterated.</param>
        public TableCellIterator(Table table) {
            if (table == null) {
                return;
            }
            this.table = table;
            this.children = table.GetChildren();
            headerIterator = new iText.Pdfua.Checkers.Utils.Tables.TableCellIterator(table.GetHeader());
            footerIterator = new iText.Pdfua.Checkers.Utils.Tables.TableCellIterator(table.GetFooter());
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
                return headerIterator.Next();
            }
            if (children != null && index < children.Count) {
                location = PdfName.TBody;
                return (Cell)children[index++];
            }
            if (footerIterator != null && footerIterator.HasNext()) {
                location = PdfName.TFoot;
                return footerIterator.Next();
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
        public PdfName GetLocation() {
            return this.location;
        }
    }
}
