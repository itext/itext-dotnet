using System;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Forms.Fields {
    /// <summary>Builder for checkbox form field.</summary>
    public class CheckBoxFormFieldBuilder : TerminalFormFieldBuilder<iText.Forms.Fields.CheckBoxFormFieldBuilder
        > {
        private int checkType = PdfFormField.TYPE_CROSS;

        /// <summary>
        /// Creates builder for
        /// <see cref="PdfButtonFormField"/>
        /// creation.
        /// </summary>
        /// <param name="document">document to be used for form field creation</param>
        /// <param name="formFieldName">name of the form field</param>
        public CheckBoxFormFieldBuilder(PdfDocument document, String formFieldName)
            : base(document, formFieldName) {
        }

        /// <summary>Gets check type for checkbox form field.</summary>
        /// <returns>check type to be set for checkbox form field</returns>
        public virtual int GetCheckType() {
            return checkType;
        }

        /// <summary>Sets check type for checkbox form field.</summary>
        /// <remarks>
        /// Sets check type for checkbox form field. Default value is
        /// <see cref="PdfFormField.TYPE_CROSS"/>.
        /// </remarks>
        /// <param name="checkType">check type to be set for checkbox form field</param>
        /// <returns>this builder</returns>
        public virtual iText.Forms.Fields.CheckBoxFormFieldBuilder SetCheckType(int checkType) {
            this.checkType = checkType;
            return this;
        }

        /// <summary>Creates checkbox form field based on provided parameters.</summary>
        /// <returns>
        /// new
        /// <see cref="PdfButtonFormField"/>
        /// instance
        /// </returns>
        public virtual PdfButtonFormField CreateCheckBox() {
            PdfButtonFormField check;
            if (GetWidgetRectangle() == null) {
                check = new PdfButtonFormField(GetDocument());
            }
            else {
                PdfWidgetAnnotation annotation = new PdfWidgetAnnotation(GetWidgetRectangle());
                annotation.SetAppearanceState(new PdfName(PdfFormField.OFF_STATE_VALUE));
                if (GetConformanceLevel() != null) {
                    annotation.SetFlag(PdfAnnotation.PRINT);
                }
                check = new PdfButtonFormField(annotation, GetDocument());
            }
            check.pdfAConformanceLevel = GetConformanceLevel();
            check.UpdateFontAndFontSize(GetDocument().GetDefaultFont(), 0);
            check.SetCheckType(checkType);
            check.SetFieldName(GetFormFieldName());
            check.Put(PdfName.V, new PdfName(PdfFormField.OFF_STATE_VALUE));
            if (GetWidgetRectangle() != null) {
                if (GetConformanceLevel() == null) {
                    check.DrawCheckAppearance(GetWidgetRectangle().GetWidth(), GetWidgetRectangle().GetHeight(), PdfFormField.
                        ON_STATE_VALUE);
                }
                else {
                    check.DrawPdfA2CheckAppearance(GetWidgetRectangle().GetWidth(), GetWidgetRectangle().GetHeight(), PdfFormField
                        .ON_STATE_VALUE, checkType);
                }
                SetPageToField(check);
            }
            return check;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override iText.Forms.Fields.CheckBoxFormFieldBuilder GetThis() {
            return this;
        }
    }
}
