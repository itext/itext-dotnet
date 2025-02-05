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
using iText.Kernel.Pdf;
using iText.Kernel.Validation;

namespace iText.Kernel.Validation.Context {
    /// <summary>
    /// Class for
    /// <see cref="iText.Kernel.Pdf.PdfXrefTable"/>
    /// validation context.
    /// </summary>
    public class XrefTableValidationContext : IValidationContext {
        private readonly PdfXrefTable xrefTable;

        /// <summary>
        /// Instantiates a new
        /// <see cref="XrefTableValidationContext"/>
        /// based on pdf xref table.
        /// </summary>
        /// <param name="xrefTable">the pdf xref table</param>
        public XrefTableValidationContext(PdfXrefTable xrefTable) {
            this.xrefTable = xrefTable;
        }

        /// <summary>Gets the pdf xref table.</summary>
        /// <returns>the pdf xref table</returns>
        public virtual PdfXrefTable GetXrefTable() {
            return xrefTable;
        }

        public virtual ValidationType GetType() {
            return ValidationType.XREF_TABLE;
        }
    }
}
