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
using iText.Kernel.Pdf.Event;

namespace iText.Signatures.Mac {
    /// <summary>Represents an event firing before embedding the signature into the document.</summary>
    public class SignatureDocumentClosingEvent : AbstractPdfDocumentEvent {
        public const String START_SIGNATURE_PRE_CLOSE = "StartSignaturePreClose";

        private readonly PdfIndirectReference signatureReference;

        /// <summary>Creates an event firing before embedding the signature into the document.</summary>
        /// <remarks>
        /// Creates an event firing before embedding the signature into the document.
        /// It contains the reference to the signature object.
        /// </remarks>
        /// <param name="signatureReference">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfIndirectReference"/>
        /// to the signature object
        /// </param>
        public SignatureDocumentClosingEvent(PdfIndirectReference signatureReference)
            : base(START_SIGNATURE_PRE_CLOSE) {
            this.signatureReference = signatureReference;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.PdfIndirectReference"/>
        /// to the signature object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfIndirectReference"/>
        /// to the signature object
        /// </returns>
        public virtual PdfIndirectReference GetSignatureReference() {
            return signatureReference;
        }
    }
}
