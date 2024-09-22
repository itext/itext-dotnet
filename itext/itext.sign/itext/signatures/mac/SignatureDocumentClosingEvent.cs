using System;
using iText.Kernel.Events;
using iText.Kernel.Pdf;

namespace iText.Signatures.Mac {
    /// <summary>Represents an event firing before embedding the signature into the document.</summary>
    public class SignatureDocumentClosingEvent : Event {
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
