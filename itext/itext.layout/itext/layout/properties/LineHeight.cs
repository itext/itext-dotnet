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
namespace iText.Layout.Properties {
    /// <summary>
    /// A property corresponding to the css line-height property and used to
    /// set the height of a line box in the HTML mode.
    /// </summary>
    /// <remarks>
    /// A property corresponding to the css line-height property and used to
    /// set the height of a line box in the HTML mode. On block-level elements,
    /// it specifies the minimum height of line boxes within the element.
    /// On non-replaced inline elements, it specifies the height that is used to calculate line box height.
    /// </remarks>
    public class LineHeight {
        private const int FIXED = 1;

        private const int MULTIPLIED = 2;

        private const int NORMAL = 4;

        private int type;

        private float value;

        private LineHeight(int type, float value) {
            this.type = type;
            this.value = value;
        }

        /// <summary>Returns the line height value.</summary>
        /// <remarks>
        /// Returns the line height value.
        /// The meaning of the returned value depends on the type of line height.
        /// </remarks>
        /// <returns>
        /// the
        /// <see cref="LineHeight"/>
        /// value.
        /// </returns>
        public virtual float GetValue() {
            return value;
        }

        /// <summary>
        /// Creates a
        /// <see cref="LineHeight"/>
        /// with a fixed value.
        /// </summary>
        /// <param name="value">value to set</param>
        /// <returns>
        /// created
        /// <see cref="LineHeight"/>
        /// object
        /// </returns>
        public static iText.Layout.Properties.LineHeight CreateFixedValue(float value) {
            return new iText.Layout.Properties.LineHeight(FIXED, value);
        }

        /// <summary>
        /// Creates a
        /// <see cref="LineHeight"/>
        /// with multiplied value.
        /// </summary>
        /// <remarks>
        /// Creates a
        /// <see cref="LineHeight"/>
        /// with multiplied value.
        /// This value must be multiplied by the element's font size.
        /// </remarks>
        /// <param name="value">value to set</param>
        /// <returns>
        /// created
        /// <see cref="LineHeight"/>
        /// object
        /// </returns>
        public static iText.Layout.Properties.LineHeight CreateMultipliedValue(float value) {
            return new iText.Layout.Properties.LineHeight(MULTIPLIED, value);
        }

        /// <summary>
        /// Creates a normal
        /// <see cref="LineHeight"/>.
        /// </summary>
        /// <returns>
        /// created
        /// <see cref="LineHeight"/>
        /// object
        /// </returns>
        public static iText.Layout.Properties.LineHeight CreateNormalValue() {
            return new iText.Layout.Properties.LineHeight(NORMAL, 0);
        }

        /// <summary>
        /// Check if the
        /// <see cref="LineHeight"/>
        /// contains fixed value.
        /// </summary>
        /// <returns>
        /// true if
        /// <see cref="LineHeight"/>
        /// contains fixed value.
        /// </returns>
        public virtual bool IsFixedValue() {
            return type == FIXED;
        }

        /// <summary>
        /// Check if the
        /// <see cref="LineHeight"/>
        /// contains multiplied value.
        /// </summary>
        /// <returns>
        /// true if
        /// <see cref="LineHeight"/>
        /// contains multiplied value.
        /// </returns>
        public virtual bool IsMultipliedValue() {
            return type == MULTIPLIED;
        }

        /// <summary>
        /// Check if the
        /// <see cref="LineHeight"/>
        /// contains normal value.
        /// </summary>
        /// <returns>
        /// true if
        /// <see cref="LineHeight"/>
        /// is normal.
        /// </returns>
        public virtual bool IsNormalValue() {
            return type == NORMAL;
        }
    }
}
