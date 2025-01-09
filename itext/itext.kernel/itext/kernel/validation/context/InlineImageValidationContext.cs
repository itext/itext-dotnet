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
    /// <summary>Class for inline image validation context.</summary>
    public class InlineImageValidationContext : IValidationContext {
        private readonly PdfStream image;

        private readonly PdfDictionary currentColorSpaces;

        /// <summary>
        /// Instantiates a new
        /// <see cref="InlineImageValidationContext"/>
        /// based on image and resources.
        /// </summary>
        /// <param name="image">the image</param>
        /// <param name="resources">the resources which are used to extract color space of the image</param>
        public InlineImageValidationContext(PdfStream image, PdfResources resources) {
            this.image = image;
            currentColorSpaces = resources == null ? null : resources.GetPdfObject().GetAsDictionary(PdfName.ColorSpace
                );
        }

        /// <summary>Gets the image.</summary>
        /// <returns>the image</returns>
        public virtual PdfStream GetImage() {
            return image;
        }

        /// <summary>Gets the current color space.</summary>
        /// <returns>the color space</returns>
        public virtual PdfDictionary GetCurrentColorSpaces() {
            return currentColorSpaces;
        }

        public virtual ValidationType GetType() {
            return ValidationType.INLINE_IMAGE;
        }
    }
}
