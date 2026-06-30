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
using iText.Kernel.Pdf;
using iText.Kernel.Validation;

namespace iText.Kernel.Validation.Context {
    /// <summary>
    /// Class for
    /// <see cref="iText.Kernel.Pdf.Filespec.PdfFileSpec"/>
    /// stream validation context.
    /// </summary>
    public class PdfFileSpecDataValidationContext : IValidationContext {
        private readonly PdfStream fileSpecDataStream;

        /// <summary>
        /// Creates a new
        /// <see cref="PdfFileSpecDataValidationContext"/>
        /// instance.
        /// </summary>
        /// <param name="fileSpecDataStream">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// which represents data for validation
        /// </param>
        public PdfFileSpecDataValidationContext(PdfStream fileSpecDataStream) {
            this.fileSpecDataStream = fileSpecDataStream;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// presentation of file spec data.
        /// </summary>
        /// <returns>file spec data stream object</returns>
        public virtual PdfStream GetFileSpecDataStream() {
            return fileSpecDataStream;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual ValidationType GetType() {
            return ValidationType.FILE_SPEC_DATA;
        }
    }
}
