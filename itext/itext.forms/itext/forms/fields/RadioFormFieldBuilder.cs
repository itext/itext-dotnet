using System;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Forms.Fields {
    /// <summary>Builder for radio form field.</summary>
    public class RadioFormFieldBuilder : TerminalFormFieldBuilder<iText.Forms.Fields.RadioFormFieldBuilder> {
        /// <summary>Creates builder for radio form field creation.</summary>
        /// <param name="document">document to be used for form field creation</param>
        /// <param name="formFieldName">name of the form field</param>
        public RadioFormFieldBuilder(PdfDocument document, String formFieldName)
            : base(document, formFieldName) {
        }

        // TODO DEVSIX-6319 Remove this constructor when radio buttons will become widgets instead of form fields.
        /// <summary>Creates builder for radio button creation.</summary>
        /// <param name="document">document to be used for form field creation</param>
        public RadioFormFieldBuilder(PdfDocument document)
            : base(document, null) {
        }

        /// <summary>Creates radio group form field instance based on provided parameters.</summary>
        /// <returns>
        /// new
        /// <see cref="PdfButtonFormField"/>
        /// instance
        /// </returns>
        public virtual PdfButtonFormField CreateRadioGroup() {
            PdfButtonFormField radio = new PdfButtonFormField(GetDocument());
            radio.UpdateFontAndFontSize(GetDocument().GetDefaultFont(), PdfFormField.DEFAULT_FONT_SIZE);
            radio.pdfAConformanceLevel = GetConformanceLevel();
            radio.SetFieldName(GetFormFieldName());
            radio.SetFieldFlags(PdfButtonFormField.FF_RADIO);
            return radio;
        }

        /// <summary>Creates radio button form field instance based on provided parameters.</summary>
        /// <param name="radioGroup">radio group to which new radio button will be added</param>
        /// <param name="appearanceName">name of the "on" appearance state.</param>
        /// <returns>new radio button instance</returns>
        public virtual PdfFormField CreateRadioButton(PdfButtonFormField radioGroup, String appearanceName) {
            PdfFormField radio;
            if (GetWidgetRectangle() == null) {
                radio = new PdfButtonFormField(GetDocument());
            }
            else {
                PdfWidgetAnnotation annotation = new PdfWidgetAnnotation(GetWidgetRectangle());
                if (null != GetConformanceLevel()) {
                    annotation.SetFlag(PdfAnnotation.PRINT);
                }
                PdfObject radioGroupValue = radioGroup.GetValue();
                PdfName appearanceState = new PdfName(appearanceName);
                if (appearanceState.Equals(radioGroupValue)) {
                    annotation.SetAppearanceState(appearanceState);
                }
                else {
                    annotation.SetAppearanceState(new PdfName(PdfFormField.OFF_STATE_VALUE));
                }
                radio = new PdfButtonFormField(annotation, GetDocument());
            }
            radio.pdfAConformanceLevel = GetConformanceLevel();
            radio.UpdateFontAndFontSize(GetDocument().GetDefaultFont(), PdfFormField.DEFAULT_FONT_SIZE);
            if (GetWidgetRectangle() != null) {
                radio.DrawRadioAppearance(GetWidgetRectangle().GetWidth(), GetWidgetRectangle().GetHeight(), appearanceName
                    );
                SetPageToField(radio);
            }
            radioGroup.AddKid(radio);
            return radio;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override iText.Forms.Fields.RadioFormFieldBuilder GetThis() {
            return this;
        }
    }
}
