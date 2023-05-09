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
            PdfFormField field = PdfFormCreator.CreateFormField(GetDocument());
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
