/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

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
                field = PdfFormCreator.CreateButtonFormField(GetDocument());
            }
            else {
                annotation = new PdfWidgetAnnotation(GetWidgetRectangle());
                field = PdfFormCreator.CreateButtonFormField(annotation, GetDocument());
                if (null != GetConformanceLevel()) {
                    annotation.SetFlag(PdfAnnotation.PRINT);
                }
            }
            field.DisableFieldRegeneration();
            if (this.GetFont() != null) {
                field.SetFont(this.GetFont());
            }
            field.pdfAConformanceLevel = GetConformanceLevel();
            field.SetPushButton(true);
            field.SetFieldName(GetFormFieldName());
            field.text = caption;
            if (annotation != null) {
                field.GetFirstFormAnnotation().backgroundColor = ColorConstants.LIGHT_GRAY;
                PdfDictionary mk = new PdfDictionary();
                mk.Put(PdfName.CA, new PdfString(caption));
                mk.Put(PdfName.BG, new PdfArray(field.GetFirstFormAnnotation().backgroundColor.GetColorValue()));
                annotation.SetAppearanceCharacteristics(mk);
                SetPageToField(field);
            }
            field.EnableFieldRegeneration();
            return field;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override iText.Forms.Fields.PushButtonFormFieldBuilder GetThis() {
            return this;
        }
    }
}
