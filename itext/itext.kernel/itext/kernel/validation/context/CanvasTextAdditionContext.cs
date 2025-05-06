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
using iText.Kernel.Pdf;
using iText.Kernel.Validation;

namespace iText.Kernel.Validation.Context {
    /// <summary>Class which contains context in which text was added to canvas.</summary>
    public class CanvasTextAdditionContext : IValidationContext {
        private readonly String text;

        private PdfNumber mcId;

        private readonly PdfDictionary attributes;

        private readonly PdfStream contentStream;

        /// <summary>
        /// Creates
        /// <see cref="CanvasTextAdditionContext"/>
        /// instance.
        /// </summary>
        /// <param name="text">text which was added to canvas</param>
        /// <param name="attributes">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// attributes which correspond to this text
        /// </param>
        /// <param name="contentStream">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// in which text is written
        /// </param>
        public CanvasTextAdditionContext(String text, PdfDictionary attributes, PdfStream contentStream) {
            this.text = text;
            this.attributes = attributes;
            this.contentStream = contentStream;
            if (attributes != null) {
                this.mcId = attributes.GetAsNumber(PdfName.MCID);
            }
        }

        /// <summary>Gets text which was added to canvas.</summary>
        /// <returns>text which was added to canvas</returns>
        public virtual String GetText() {
            return text;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.PdfNumber"/>
        /// which represents MCID of this text.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfNumber"/>
        /// which represents MCID of this text
        /// </returns>
        public virtual PdfNumber GetMcId() {
            return mcId;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// attributes which correspond to the added text.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// attributes which correspond to the added text
        /// </returns>
        public virtual PdfDictionary GetAttributes() {
            return attributes;
        }

        /// <summary>
        /// Returns
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// on which text is written.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// on which text is written
        /// </returns>
        public virtual PdfStream GetContentStream() {
            return contentStream;
        }

        public virtual ValidationType GetType() {
            return ValidationType.CANVAS_TEXT_ADDITION;
        }
    }
}
