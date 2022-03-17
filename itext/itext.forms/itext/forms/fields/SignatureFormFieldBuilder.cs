using System;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Forms.Fields {
    /// <summary>Builder for signature form field.</summary>
    public class SignatureFormFieldBuilder : TerminalFormFieldBuilder<iText.Forms.Fields.SignatureFormFieldBuilder
        > {
        /// <summary>
        /// Creates builder for
        /// <see cref="PdfSignatureFormField"/>
        /// creation.
        /// </summary>
        /// <param name="document">document to be used for form field creation</param>
        /// <param name="formFieldName">name of the form field</param>
        public SignatureFormFieldBuilder(PdfDocument document, String formFieldName)
            : base(document, formFieldName) {
        }

        /// <summary>Creates signature form field based on provided parameters.</summary>
        /// <returns>
        /// new
        /// <see cref="PdfSignatureFormField"/>
        /// instance.
        /// </returns>
        public virtual PdfSignatureFormField CreateSignature() {
            PdfSignatureFormField signatureFormField;
            if (GetWidgetRectangle() == null) {
                signatureFormField = new PdfSignatureFormField(GetDocument());
            }
            else {
                PdfWidgetAnnotation annotation = new PdfWidgetAnnotation(GetWidgetRectangle());
                if (GetConformanceLevel() != null) {
                    annotation.SetFlag(PdfAnnotation.PRINT);
                }
                signatureFormField = new PdfSignatureFormField(annotation, GetDocument());
                SetPageToField(signatureFormField);
            }
            signatureFormField.pdfAConformanceLevel = GetConformanceLevel();
            signatureFormField.SetFieldName(GetFormFieldName());
            return signatureFormField;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override iText.Forms.Fields.SignatureFormFieldBuilder GetThis() {
            return this;
        }
    }
}
