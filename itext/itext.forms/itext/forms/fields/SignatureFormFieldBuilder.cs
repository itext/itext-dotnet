/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
    /// <summary>Builder for signature form field.</summary>
    public class SignatureFormFieldBuilder : TerminalFormFieldBuilder<iText.Forms.Fields.SignatureFormFieldBuilder
        > {
        /// <summary>
        /// Creates builder for
        /// <see cref="PdfSignatureFormField"/>
        /// creation.
        /// </summary>
        /// <param name="document">document to be used for form field creation</param>
        /// <param name="formFieldName">name of the form field</param>
        public SignatureFormFieldBuilder(PdfDocument document, String formFieldName)
            : base(document, formFieldName) {
        }

        /// <summary>Creates signature form field based on provided parameters.</summary>
        /// <returns>
        /// new
        /// <see cref="PdfSignatureFormField"/>
        /// instance.
        /// </returns>
        public virtual PdfSignatureFormField CreateSignature() {
            PdfSignatureFormField signatureFormField;
            if (GetWidgetRectangle() == null) {
                signatureFormField = PdfFormCreator.CreateSignatureFormField(GetDocument());
            }
            else {
                PdfWidgetAnnotation annotation = new PdfWidgetAnnotation(GetWidgetRectangle());
                if (GetConformance() != null && GetConformance().IsPdfAOrUa()) {
                    annotation.SetFlag(PdfAnnotation.PRINT);
                }
                signatureFormField = PdfFormCreator.CreateSignatureFormField(annotation, GetDocument());
                SetPageToField(signatureFormField);
            }
            // we can't use setFont() here, because the signature values can only be created one time on first
            // appearance generation, so we avoid the generation call until the moment we have all the necessary data
            if (GetFont() != null) {
                signatureFormField.font = GetFont();
            }
            signatureFormField.pdfConformance = GetConformance();
            signatureFormField.SetFieldName(GetFormFieldName());
            return signatureFormField;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override iText.Forms.Fields.SignatureFormFieldBuilder GetThis() {
            return this;
        }
    }
}
