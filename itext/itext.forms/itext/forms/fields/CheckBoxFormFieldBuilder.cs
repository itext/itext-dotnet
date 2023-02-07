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
                annotation.SetAppearanceState(new PdfName(PdfFormAnnotation.OFF_STATE_VALUE));
                if (GetConformanceLevel() != null) {
                    annotation.SetFlag(PdfAnnotation.PRINT);
                }
                check = new PdfButtonFormField(annotation, GetDocument());
            }
            check.pdfAConformanceLevel = GetConformanceLevel();
            check.SetCheckType(checkType);
            check.SetFieldName(GetFormFieldName());
            check.Put(PdfName.V, new PdfName(PdfFormAnnotation.OFF_STATE_VALUE));
            if (GetWidgetRectangle() != null) {
                if (GetConformanceLevel() == null) {
                    check.GetFirstFormAnnotation().DrawCheckAppearance(GetWidgetRectangle().GetWidth(), GetWidgetRectangle().GetHeight
                        (), PdfFormAnnotation.ON_STATE_VALUE);
                }
                else {
                    check.GetFirstFormAnnotation().DrawPdfA2CheckAppearance(GetWidgetRectangle().GetWidth(), GetWidgetRectangle
                        ().GetHeight(), PdfFormAnnotation.ON_STATE_VALUE, checkType);
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
