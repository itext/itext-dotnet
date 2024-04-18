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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Element;
using iText.Pdfua.Checkers.Utils;

namespace iText.Pdfua.Checkers.Utils.Tables {
    /// <summary>Class that has the result of the algorithm that checks the table for PDF/UA compliance.</summary>
    internal sealed class CellResultMatrix : AbstractResultMatrix<Cell> {
        /// <summary>
        /// Creates a new
        /// <see cref="CellResultMatrix"/>
        /// instance.
        /// </summary>
        /// <param name="table">The table that needs to be checked.</param>
        /// <param name="context">The validation context.</param>
        public CellResultMatrix(Table table, PdfUAValidationContext context)
            : base(new TableCellIterator(table, context)) {
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
            return ((TableCellIterator)iterator).context.ResolveToStandardRole(cell.GetAccessibilityProperties().GetRole
                ());
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
