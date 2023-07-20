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
                field = PdfFormCreator.CreateTextFormField(GetDocument());
            }
            else {
                PdfWidgetAnnotation annotation = new PdfWidgetAnnotation(GetWidgetRectangle());
                if (null != GetConformanceLevel()) {
                    annotation.SetFlag(PdfAnnotation.PRINT);
                }
                field = PdfFormCreator.CreateTextFormField(annotation, GetDocument());
                SetPageToField(field);
            }
            field.DisableFieldRegeneration();
            field.pdfAConformanceLevel = GetConformanceLevel();
            field.SetMultiline(multiline);
            field.SetFieldName(GetFormFieldName());
            field.SetValue(TEXT_FORM_FIELD_DEFAULT_VALUE);
            field.EnableFieldRegeneration();
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
