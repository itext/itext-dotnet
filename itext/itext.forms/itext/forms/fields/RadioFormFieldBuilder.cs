/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Forms.Exceptions;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Forms.Fields {
    /// <summary>Builder for radio form field.</summary>
    public class RadioFormFieldBuilder : TerminalFormFieldBuilder<iText.Forms.Fields.RadioFormFieldBuilder> {
        /// <summary>Creates builder for radio form field creation.</summary>
        /// <param name="document">document to be used for form field creation</param>
        /// <param name="radioGroupFormFieldName">name of the form field</param>
        public RadioFormFieldBuilder(PdfDocument document, String radioGroupFormFieldName)
            : base(document, radioGroupFormFieldName) {
        }

        /// <summary>Creates radio group form field instance based on provided parameters.</summary>
        /// <returns>
        /// new
        /// <see cref="PdfButtonFormField"/>
        /// instance
        /// </returns>
        public virtual PdfButtonFormField CreateRadioGroup() {
            PdfButtonFormField radioGroup = PdfFormCreator.CreateButtonFormField(GetDocument());
            radioGroup.DisableFieldRegeneration();
            radioGroup.pdfAConformanceLevel = GetConformanceLevel();
            radioGroup.SetFieldName(GetFormFieldName());
            radioGroup.SetFieldFlags(PdfButtonFormField.FF_RADIO);
            radioGroup.EnableFieldRegeneration();
            return radioGroup;
        }

        /// <summary>Creates radio button form field instance based on provided parameters.</summary>
        /// <param name="appearanceName">name of the "on" appearance state.</param>
        /// <param name="rectangle">the place where the widget should be placed.</param>
        /// <returns>new radio button instance</returns>
        public virtual PdfFormAnnotation CreateRadioButton(String appearanceName, Rectangle rectangle) {
            if (appearanceName == null || String.IsNullOrEmpty(appearanceName)) {
                throw new PdfException(FormsExceptionMessageConstant.APEARANCE_NAME_MUST_BE_PROVIDED);
            }
            Rectangle widgetRectangle = GetWidgetRectangle();
            if (rectangle != null) {
                widgetRectangle = rectangle;
            }
            if (widgetRectangle == null) {
                throw new PdfException(FormsExceptionMessageConstant.WIDGET_RECTANGLE_MUST_BE_PROVIDED);
            }
            PdfName appearancePdfName = new PdfName(appearanceName);
            PdfWidgetAnnotation annotation = new PdfWidgetAnnotation(widgetRectangle);
            annotation.SetAppearanceState(appearancePdfName);
            if (GetConformanceLevel() != null) {
                annotation.SetFlag(PdfAnnotation.PRINT);
            }
            PdfFormAnnotation radio = PdfFormCreator.CreateFormAnnotation(annotation, GetDocument());
            SetPageToField(radio);
            radio.pdfAConformanceLevel = GetConformanceLevel();
            return radio;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override iText.Forms.Fields.RadioFormFieldBuilder GetThis() {
            return this;
        }
    }
}
