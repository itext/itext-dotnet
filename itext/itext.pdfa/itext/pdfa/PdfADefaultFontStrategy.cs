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
using iText.Kernel.Font;
using iText.Kernel.Pdf;

namespace iText.Pdfa {
    /// <summary>
    /// The class presents default font strategy for PDF/A documents which
    /// doesn't provide default font because all used fonts must be embedded.
    /// </summary>
    public class PdfADefaultFontStrategy : DefaultFontStrategy {
        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfADefaultFontStrategy"/>
        /// instance based on the document which will use that strategy.
        /// </summary>
        /// <param name="pdfDocument">the pdf document which will use that strategy</param>
        public PdfADefaultFontStrategy(PdfDocument pdfDocument)
            : base(pdfDocument) {
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public override PdfFont GetFont() {
            return null;
        }
    }
}
