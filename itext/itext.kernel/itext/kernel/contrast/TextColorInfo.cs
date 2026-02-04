/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Geom;

namespace iText.Kernel.Contrast {
    /// <summary>Represents rendering information for a text element in contrast analysis.</summary>
    public class TextColorInfo : ColorInfo {
        private readonly String text;

        private readonly String parent;

        private readonly float fontSize;

        /// <summary>
        /// Constructs a new
        /// <see cref="TextColorInfo"/>
        /// with the specified text properties.
        /// </summary>
        /// <param name="text">the text content (typically a single character) being rendered</param>
        /// <param name="parent">the parent text string that this text is part of, providing context</param>
        /// <param name="color">the fill color of the text</param>
        /// <param name="path">the geometric path defining the shape and position of the text element</param>
        /// <param name="fontSize">the font size in points.</param>
        public TextColorInfo(String text, String parent, Color color, Path path, float fontSize)
            : base(color, path) {
            this.text = text;
            this.parent = parent;
            this.fontSize = fontSize;
        }

        /// <summary>Gets the text content being rendered.</summary>
        /// <remarks>
        /// Gets the text content being rendered.
        /// <para />
        /// This typically represents a single character or glyph from the PDF content stream.
        /// </remarks>
        /// <returns>the text content as a String</returns>
        public virtual String GetText() {
            return text;
        }

        /// <summary>Gets the parent text string that this text is part of.</summary>
        /// <remarks>
        /// Gets the parent text string that this text is part of.
        /// <para />
        /// This provides context about the larger text block or string that contains this
        /// individual text element, which can be useful for debugging and analysis.
        /// </remarks>
        /// <returns>the parent text string</returns>
        public virtual String GetParent() {
            return parent;
        }

        /// <summary>Gets the font size of the text in points.</summary>
        /// <returns>the font size in points</returns>
        public virtual float GetFontSize() {
            return fontSize;
        }

        /// <summary>Returns a string representation of this text render information.</summary>
        /// <returns>a string containing the text, parent, color, and path information</returns>
        public override String ToString() {
            return "TextRenderInfo{" + "character=" + text + ", parent=" + parent + ", color=" + GetColor() + ", path="
                 + GetPath() + '}';
        }
    }
}
