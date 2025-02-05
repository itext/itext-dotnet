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

namespace iText.Forms.Fields {
    /// <summary>Builder for form field.</summary>
    /// <typeparam name="T">specific form field builder which extends this class.</typeparam>
    public abstract class FormFieldBuilder<T>
        where T : iText.Forms.Fields.FormFieldBuilder<T> {
        /// <summary>Document to be used for form field creation.</summary>
        private readonly PdfDocument document;

        /// <summary>Name of the form field.</summary>
        private readonly String formFieldName;

        /// <summary>Conformance of the form field.</summary>
        private PdfConformance conformance = null;

        /// <summary>
        /// Creates builder for
        /// <see cref="PdfFormField"/>
        /// creation.
        /// </summary>
        /// <param name="document">document to be used for form field creation</param>
        /// <param name="formFieldName">name of the form field</param>
        protected internal FormFieldBuilder(PdfDocument document, String formFieldName) {
            this.document = document;
            this.formFieldName = formFieldName;
            if (document != null) {
                this.conformance = document.GetConformance();
            }
        }

        /// <summary>Gets document to be used for form field creation.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance
        /// </returns>
        public virtual PdfDocument GetDocument() {
            return document;
        }

        /// <summary>Gets name of the form field.</summary>
        /// <returns>name to be used for form field creation</returns>
        public virtual String GetFormFieldName() {
            return formFieldName;
        }

        /// <summary>Gets conformance for form field creation.</summary>
        /// <returns>
        /// instance of
        /// <see cref="iText.Kernel.Pdf.PdfConformance"/>
        /// to be used for form field creation
        /// </returns>
        public virtual PdfConformance GetConformance() {
            return conformance;
        }

        /// <summary>Sets conformance for form field creation.</summary>
        /// <param name="conformance">
        /// Instance of
        /// <see cref="iText.Kernel.Pdf.PdfConformance"/>
        /// to be used for form field creation.
        /// </param>
        /// <returns>this builder</returns>
        public virtual T SetConformance(PdfConformance conformance) {
            this.conformance = conformance;
            return GetThis();
        }

        /// <summary>Returns this builder object.</summary>
        /// <remarks>Returns this builder object. Required for superclass methods.</remarks>
        /// <returns>this builder</returns>
        protected internal abstract T GetThis();
    }
}
