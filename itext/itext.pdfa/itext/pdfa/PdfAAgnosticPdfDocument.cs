/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Kernel.Pdf;

namespace iText.Pdfa {
    /// <summary>
    /// This class extends
    /// <see cref="PdfADocument"/>
    /// and serves as
    /// <see cref="PdfADocument"/>
    /// for
    /// PDF/A compliant documents and as
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// for non PDF/A documents.
    /// </summary>
    /// <remarks>
    /// This class extends
    /// <see cref="PdfADocument"/>
    /// and serves as
    /// <see cref="PdfADocument"/>
    /// for
    /// PDF/A compliant documents and as
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// for non PDF/A documents.
    /// <para />
    /// This class can throw various exceptions like
    /// <see cref="iText.Kernel.Exceptions.PdfException"/>
    /// as well as
    /// <see cref="iText.Pdfa.Exceptions.PdfAConformanceException"/>
    /// for PDF/A documents.
    /// </remarks>
    public class PdfAAgnosticPdfDocument : PdfADocument {
        private PdfFont defaultFont;

        /// <summary>Opens a PDF/A document in stamping mode.</summary>
        /// <param name="reader">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// </param>
        /// <param name="writer">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object to write to
        /// </param>
        public PdfAAgnosticPdfDocument(PdfReader reader, PdfWriter writer)
            : this(reader, writer, new StampingProperties()) {
        }

        /// <summary>Opens a PDF/A document in stamping mode.</summary>
        /// <param name="reader">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// </param>
        /// <param name="writer">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object to write to
        /// </param>
        /// <param name="properties">
        /// 
        /// <see cref="iText.Kernel.Pdf.StampingProperties"/>
        /// of the stamping process
        /// </param>
        public PdfAAgnosticPdfDocument(PdfReader reader, PdfWriter writer, StampingProperties properties)
            : base(reader, writer, properties, true) {
        }

        /// <summary>Get default font for the document: Helvetica, WinAnsi.</summary>
        /// <remarks>
        /// Get default font for the document: Helvetica, WinAnsi.
        /// One instance per document.
        /// </remarks>
        /// <returns>
        /// instance of
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// or
        /// <see langword="null"/>
        /// on error.
        /// </returns>
        public override PdfFont GetDefaultFont() {
            // TODO DEVSIX-7850 investigate embedding default font into PDF/A documents while signing
            if (defaultFont == null) {
                try {
                    defaultFont = PdfFontFactory.CreateFont();
                    if (writer != null) {
                        defaultFont.MakeIndirect(this);
                    }
                }
                catch (System.IO.IOException e) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(PdfDocument));
                    logger.LogError(e, iText.IO.Logs.IoLogMessageConstant.EXCEPTION_WHILE_CREATING_DEFAULT_FONT);
                    defaultFont = null;
                }
            }
            return defaultFont;
        }
    }
}
