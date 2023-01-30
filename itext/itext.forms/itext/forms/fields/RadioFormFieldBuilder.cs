/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
                    annotation.SetAppearanceState(new PdfName(PdfFormAnnotation.OFF_STATE_VALUE));
                }
                radio = new PdfButtonFormField(annotation, GetDocument());
            }
            radio.pdfAConformanceLevel = GetConformanceLevel();
            if (GetWidgetRectangle() != null) {
                radio.GetFirstFormAnnotation().DrawRadioAppearance(GetWidgetRectangle().GetWidth(), GetWidgetRectangle().GetHeight
                    (), appearanceName);
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
