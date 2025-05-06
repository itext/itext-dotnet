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
using iText.Kernel.Pdf;
using iText.Kernel.Validation;

namespace iText.Kernel.Validation.Context {
    /// <summary>Class which contains context in which annotation was added.</summary>
    public class PdfAnnotationContext : IValidationContext {
        private readonly PdfDictionary annotation;

        /// <summary>
        /// Creates new
        /// <see cref="PdfAnnotationContext"/>
        /// instance.
        /// </summary>
        /// <param name="annotation">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// annotation which was added
        /// </param>
        public PdfAnnotationContext(PdfDictionary annotation) {
            this.annotation = annotation;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual ValidationType GetType() {
            return ValidationType.ANNOTATION;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// annotation instance.
        /// </summary>
        /// <returns>annotation dictionary</returns>
        public virtual PdfDictionary GetAnnotation() {
            return annotation;
        }
    }
}
