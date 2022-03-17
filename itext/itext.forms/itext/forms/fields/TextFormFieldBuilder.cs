using System;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Forms.Fields {
    /// <summary>Builder for text form field.</summary>
    public class TextFormFieldBuilder : TerminalFormFieldBuilder<iText.Forms.Fields.TextFormFieldBuilder> {
        private const String TEXT_FORM_FIELD_DEFAULT_VALUE = "";

        /// <summary>
        /// Creates builder for
        /// <see cref="PdfTextFormField"/>
        /// creation.
        /// </summary>
        /// <param name="document">document to be used for form field creation</param>
        /// <param name="formFieldName">name of the form field</param>
        public TextFormFieldBuilder(PdfDocument document, String formFieldName)
            : base(document, formFieldName) {
        }

        /// <summary>Creates text form field based on provided parameters.</summary>
        /// <returns>
        /// new
        /// <see cref="PdfTextFormField"/>
        /// instance
        /// </returns>
        public virtual PdfTextFormField CreateText() {
            return CreateText(false);
        }

        private PdfTextFormField CreateText(bool multiline) {
            PdfTextFormField field;
            if (GetWidgetRectangle() == null) {
                field = new PdfTextFormField(GetDocument());
            }
            else {
                PdfWidgetAnnotation annotation = new PdfWidgetAnnotation(GetWidgetRectangle());
                if (null != GetConformanceLevel()) {
                    annotation.SetFlag(PdfAnnotation.PRINT);
                }
                field = new PdfTextFormField(annotation, GetDocument());
                SetPageToField(field);
            }
            field.pdfAConformanceLevel = GetConformanceLevel();
            field.UpdateFontAndFontSize(GetDocument().GetDefaultFont(), PdfFormField.DEFAULT_FONT_SIZE);
            field.SetMultiline(multiline);
            field.SetFieldName(GetFormFieldName());
            field.SetValue(TEXT_FORM_FIELD_DEFAULT_VALUE);
            return field;
        }

        /// <summary>Creates multiline text form field based on provided parameters.</summary>
        /// <returns>
        /// new
        /// <see cref="PdfTextFormField"/>
        /// instance
        /// </returns>
        public virtual PdfTextFormField CreateMultilineText() {
            return CreateText(true);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override iText.Forms.Fields.TextFormFieldBuilder GetThis() {
            return this;
        }
    }
}
