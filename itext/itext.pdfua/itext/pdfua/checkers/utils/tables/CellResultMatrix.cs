using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Element;

namespace iText.Pdfua.Checkers.Utils.Tables {
    /// <summary>Class that has the result of the algorithm that checks the table for PDF/UA compliance.</summary>
    internal sealed class CellResultMatrix : AbstractResultMatrix<Cell> {
        /// <summary>
        /// Creates a new
        /// <see cref="CellResultMatrix"/>
        /// instance.
        /// </summary>
        /// <param name="cols">The number of columns in the table.</param>
        /// <param name="table">The table that needs to be checked.</param>
        public CellResultMatrix(int cols, Table table)
            : base(cols, new TableCellIterator(table)) {
        }

        /// <summary><inheritDoc/></summary>
        internal override int GetCol(Cell cell) {
            return cell.GetCol();
        }

        /// <summary><inheritDoc/></summary>
        internal override int GetRow(Cell cell) {
            PdfName location = this.iterator.GetLocation();
            int row = cell.GetRow();
            if (location == PdfName.TBody) {
                row += this.iterator.GetAmountOfRowsHeader();
            }
            if (location == PdfName.TFoot) {
                row += this.iterator.GetAmountOfRowsHeader();
                row += this.iterator.GetAmountOfRowsBody();
            }
            return row;
        }

        /// <summary><inheritDoc/></summary>
        internal override int GetRowspan(Cell data) {
            return data.GetRowspan();
        }

        /// <summary><inheritDoc/></summary>
        internal override int GetColspan(Cell data) {
            return data.GetColspan();
        }

        /// <summary><inheritDoc/></summary>
        internal override IList<byte[]> GetHeaders(Cell cell) {
            PdfObject headerArray = GetAttribute(cell.GetAccessibilityProperties(), PdfName.Headers);
            if (headerArray == null) {
                return null;
            }
            //If it's not an array, we return an empty list to trigger failure.
            if (!headerArray.IsArray()) {
                return new List<byte[]>();
            }
            PdfArray array = (PdfArray)headerArray;
            IList<byte[]> result = new List<byte[]>();
            foreach (PdfObject pdfObject in array) {
                result.Add(((PdfString)pdfObject).GetValueBytes());
            }
            return result;
        }

        /// <summary><inheritDoc/></summary>
        internal override String GetScope(Cell cell) {
            PdfName pdfStr = (PdfName)GetAttribute(cell.GetAccessibilityProperties(), PdfName.Scope);
            return pdfStr == null ? null : pdfStr.GetValue();
        }

        /// <summary><inheritDoc/></summary>
        internal override byte[] GetElementId(Cell cell) {
            return cell.GetAccessibilityProperties().GetStructureElementId();
        }

        /// <summary><inheritDoc/></summary>
        internal override String GetRole(Cell cell) {
            return cell.GetAccessibilityProperties().GetRole();
        }

        private static PdfObject GetAttribute(AccessibilityProperties props, PdfName name) {
            foreach (PdfStructureAttributes attributes in props.GetAttributesList()) {
                PdfObject obj = attributes.GetPdfObject().Get(name);
                if (obj != null) {
                    return obj;
                }
            }
            return null;
        }
    }
}
