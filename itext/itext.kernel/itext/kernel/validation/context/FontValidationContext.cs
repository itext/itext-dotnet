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
using iText.Kernel.Font;
using iText.Kernel.Validation;

namespace iText.Kernel.Validation.Context {
    /// <summary>Class for font validation context.</summary>
    public class FontValidationContext : IValidationContext {
        private readonly String text;

        private readonly PdfFont font;

        /// <summary>
        /// Instantiates a new
        /// <see cref="FontValidationContext"/>
        /// based on text and font.
        /// </summary>
        /// <param name="text">the text</param>
        /// <param name="font">the font</param>
        public FontValidationContext(String text, PdfFont font) {
            this.text = text;
            this.font = font;
        }

        /// <summary>Gets the text.</summary>
        /// <returns>the text</returns>
        public virtual String GetText() {
            return text;
        }

        /// <summary>Gets the font.</summary>
        /// <returns>the font</returns>
        public virtual PdfFont GetFont() {
            return font;
        }

        public virtual ValidationType GetType() {
            return ValidationType.FONT;
        }
    }
}
