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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Kernel.Font;

namespace iText.Kernel.Pdf {
    /// <summary>
    /// The class defines a default font strategy for
    /// <see cref="PdfDocument"/>
    /// which is used in the scope of
    /// <see cref="PdfDocument.GetDefaultFont()"/>.
    /// </summary>
    public class DefaultFontStrategy {
        private readonly PdfDocument pdfDocument;

        private PdfFont defaultFont = null;

        /// <summary>
        /// Instantiates a new instance of
        /// <see cref="DefaultFontStrategy"/>
        /// which
        /// will be used for passed
        /// <see cref="PdfDocument"/>
        /// instance.
        /// </summary>
        /// <param name="pdfDocument">the pdf document for which the strategy will be used to</param>
        public DefaultFontStrategy(PdfDocument pdfDocument) {
            this.pdfDocument = pdfDocument;
        }

        /// <summary>Gets default font.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// instance
        /// </returns>
        public virtual PdfFont GetFont() {
            if (defaultFont == null) {
                try {
                    defaultFont = PdfFontFactory.CreateFont();
                    if (pdfDocument.GetWriter() != null) {
                        defaultFont.MakeIndirect(pdfDocument);
                    }
                }
                catch (System.IO.IOException e) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.DefaultFontStrategy));
                    logger.LogError(e, iText.IO.Logs.IoLogMessageConstant.EXCEPTION_WHILE_CREATING_DEFAULT_FONT);
                    defaultFont = null;
                }
            }
            return defaultFont;
        }
    }
}
