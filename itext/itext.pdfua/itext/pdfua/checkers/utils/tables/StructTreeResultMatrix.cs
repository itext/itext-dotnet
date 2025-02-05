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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Pdfua.Checkers.Utils;

namespace iText.Pdfua.Checkers.Utils.Tables {
//\cond DO_NOT_DOCUMENT
    /// <summary>The result matrix to validate PDF UA1 tables based on the TagTreeStructure of the document.</summary>
    internal class StructTreeResultMatrix : AbstractResultMatrix<PdfStructElem> {
        /// <summary>
        /// Creates a new
        /// <see cref="StructTreeResultMatrix"/>
        /// instance.
        /// </summary>
        /// <param name="elem">a table structure element.</param>
        /// <param name="context">The validation context.</param>
        public StructTreeResultMatrix(PdfStructElem elem, PdfUAValidationContext context)
            : base(new TableStructElementIterator(elem, context)) {
        }

//\cond DO_NOT_DOCUMENT
        /// <summary><inheritDoc/></summary>
        internal override IList<byte[]> GetHeaders(PdfStructElem cell) {
            PdfObject @object = cell.GetAttributes(false);
            PdfArray pdfArr = null;
            if (@object is PdfArray) {
                PdfArray array = (PdfArray)@object;
                foreach (PdfObject pdfObject in array) {
                    if (pdfObject is PdfDictionary) {
                        pdfArr = ((PdfDictionary)pdfObject).GetAsArray(PdfName.Headers);
                    }
                }
            }
            else {
                if (@object is PdfDictionary) {
                    pdfArr = ((PdfDictionary)@object).GetAsArray(PdfName.Headers);
                }
            }
            if (pdfArr == null) {
                return null;
            }
            IList<byte[]> list = new List<byte[]>();
            foreach (PdfObject pdfObject in pdfArr) {
                PdfString str = (PdfString)pdfObject;
                list.Add(str.GetValueBytes());
            }
            return list;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary><inheritDoc/></summary>
        internal override String GetScope(PdfStructElem cell) {
            PdfObject @object = cell.GetAttributes(false);
            if (@object is PdfArray) {
                PdfArray array = (PdfArray)@object;
                foreach (PdfObject pdfObject in array) {
                    if (pdfObject is PdfDictionary) {
                        PdfName f = ((PdfDictionary)pdfObject).GetAsName(PdfName.Scope);
                        if (f != null) {
                            return f.GetValue();
                        }
                    }
                }
            }
            else {
                if (@object is PdfDictionary) {
                    PdfName f = ((PdfDictionary)@object).GetAsName(PdfName.Scope);
                    if (f != null) {
                        return f.GetValue();
                    }
                }
            }
            return null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary><inheritDoc/></summary>
        internal override byte[] GetElementId(PdfStructElem cell) {
            if (cell == null) {
                return null;
            }
            if (cell.GetStructureElementId() == null) {
                return null;
            }
            return cell.GetStructureElementId().GetValueBytes();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary><inheritDoc/></summary>
        internal override String GetRole(PdfStructElem cell) {
            return ((TableStructElementIterator)iterator).context.ResolveToStandardRole(cell);
        }
//\endcond
    }
//\endcond
}
