using System;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Xobject;

namespace iText.Forms.Fields {
    /// <summary>Builder for push button form field.</summary>
    public class PushButtonFormFieldBuilder : TerminalFormFieldBuilder<iText.Forms.Fields.PushButtonFormFieldBuilder
        > {
        private String caption = "";

        /// <summary>
        /// Creates builder for
        /// <see cref="PdfButtonFormField"/>
        /// creation.
        /// </summary>
        /// <param name="document">document to be used for form field creation</param>
        /// <param name="formFieldName">name of the form field</param>
        public PushButtonFormFieldBuilder(PdfDocument document, String formFieldName)
            : base(document, formFieldName) {
        }

        /// <summary>Gets caption for button form field creation.</summary>
        /// <returns>caption value to be used for form field creation</returns>
        public virtual String GetCaption() {
            return caption;
        }

        /// <summary>Sets caption for button form field creation.</summary>
        /// <param name="caption">caption value to be used for form field creation</param>
        /// <returns>this builder</returns>
        public virtual iText.Forms.Fields.PushButtonFormFieldBuilder SetCaption(String caption) {
            this.caption = caption;
            return GetThis();
        }

        /// <summary>Creates push button form field base on provided parameters.</summary>
        /// <returns>
        /// new
        /// <see cref="PdfButtonFormField"/>
        /// instance
        /// </returns>
        public virtual PdfButtonFormField CreatePushButton() {
            PdfButtonFormField field;
            PdfWidgetAnnotation annotation = null;
            if (GetWidgetRectangle() == null) {
                field = new PdfButtonFormField(GetDocument());
            }
            else {
                annotation = new PdfWidgetAnnotation(GetWidgetRectangle());
                field = new PdfButtonFormField(annotation, GetDocument());
                if (null != GetConformanceLevel()) {
                    annotation.SetFlag(PdfAnnotation.PRINT);
                }
            }
            field.pdfAConformanceLevel = GetConformanceLevel();
            field.SetPushButton(true);
            field.SetFieldName(GetFormFieldName());
            field.text = caption;
            field.UpdateFontAndFontSize(GetDocument().GetDefaultFont(), PdfFormField.DEFAULT_FONT_SIZE);
            field.backgroundColor = ColorConstants.LIGHT_GRAY;
            if (annotation != null) {
                PdfFormXObject xObject = field.DrawPushButtonAppearance(GetWidgetRectangle().GetWidth(), GetWidgetRectangle
                    ().GetHeight(), caption, GetDocument().GetDefaultFont(), PdfFormField.DEFAULT_FONT_SIZE);
                annotation.SetNormalAppearance(xObject.GetPdfObject());
                PdfDictionary mk = new PdfDictionary();
                mk.Put(PdfName.CA, new PdfString(caption));
                mk.Put(PdfName.BG, new PdfArray(field.backgroundColor.GetColorValue()));
                annotation.SetAppearanceCharacteristics(mk);
                if (GetConformanceLevel() != null) {
                    PdfFormField.CreatePushButtonAppearanceState(annotation.GetPdfObject());
                }
                SetPageToField(field);
            }
            return field;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override iText.Forms.Fields.PushButtonFormFieldBuilder GetThis() {
            return this;
        }
    }
}
