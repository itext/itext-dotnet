using System;
using iText.Kernel.Pdf;

namespace iText.Forms.Fields {
    /// <summary>Builder for non-terminal form field.</summary>
    public class NonTerminalFormFieldBuilder : FormFieldBuilder<iText.Forms.Fields.NonTerminalFormFieldBuilder
        > {
        /// <summary>
        /// Creates builder for non-terminal
        /// <see cref="PdfFormField"/>
        /// creation.
        /// </summary>
        /// <param name="document">document to be used for form field creation</param>
        /// <param name="formFieldName">name of the form field</param>
        public NonTerminalFormFieldBuilder(PdfDocument document, String formFieldName)
            : base(document, formFieldName) {
        }

        /// <summary>Creates non-terminal form field based on provided parameters.</summary>
        /// <returns>
        /// new
        /// <see cref="PdfFormField"/>
        /// instance
        /// </returns>
        public virtual PdfFormField CreateNonTerminalFormField() {
            PdfFormField field = new PdfFormField(GetDocument());
            field.pdfAConformanceLevel = GetConformanceLevel();
            field.SetFieldName(GetFormFieldName());
            return field;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override iText.Forms.Fields.NonTerminalFormFieldBuilder GetThis() {
            return this;
        }
    }
}
