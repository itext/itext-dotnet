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
