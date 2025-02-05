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

namespace iText.Forms.Xfdf {
    /// <summary>Represents the BorderStyleAlt element, a child of the link element.</summary>
    /// <remarks>
    /// Represents the BorderStyleAlt element, a child of the link element.
    /// Corresponds to the Border key in the common annotation dictionary.
    /// Content model: border style encoded in the format specified in the border style attributes.
    /// Required attributes: HCornerRadius, VCornerRadius, Width.
    /// Optional attributes: DashPattern.
    /// For more details see paragraph 6.5.3 in Xfdf document specification.
    /// For more details about attributes see paragraph 6.6.19 in Xfdf document specification.
    /// </remarks>
    public class BorderStyleAltObject {
        /// <summary>Number specifying the horizontal corner radius of the rectangular border.</summary>
        /// <remarks>
        /// Number specifying the horizontal corner radius of the rectangular border.
        /// Corresponds to array index 0 in the Border key in the common annotation dictionary.
        /// </remarks>
        private float hCornerRadius;

        /// <summary>Number specifying the vertical corner radius of the rectangular border.</summary>
        /// <remarks>
        /// Number specifying the vertical corner radius of the rectangular border.
        /// Corresponds to array index 1 in the Border key in the common annotation dictionary.
        /// </remarks>
        private float vCornerRadius;

        /// <summary>Number specifying the width of the rectangular border.</summary>
        /// <remarks>
        /// Number specifying the width of the rectangular border.
        /// Corresponds to array index 2 in the Border key in the common annotation dictionary.
        /// </remarks>
        private float width;

        /// <summary>An array of numbers specifying the pattern of dashes and gaps of the border.</summary>
        /// <remarks>
        /// An array of numbers specifying the pattern of dashes and gaps of the border.
        /// Corresponds to array index 3 in the Border key in the common annotation dictionary.
        /// </remarks>
        private float[] dashPattern;

        /// <summary>Encoded border style string.</summary>
        private String content;

        /// <summary>Creates an instance that encapsulates BorderStyleAlt XFDF element data.</summary>
        /// <param name="hCornerRadius">a float value specifying the horizontal corner radius of the rectangular border.
        ///     </param>
        /// <param name="vCornerRadius">a float value specifying the vertical corner radius of the rectangular border.
        ///     </param>
        /// <param name="width">a float value specifying the width of the rectangular border.</param>
        public BorderStyleAltObject(float hCornerRadius, float vCornerRadius, float width) {
            this.hCornerRadius = hCornerRadius;
            this.vCornerRadius = vCornerRadius;
            this.width = width;
        }

        /// <summary>Gets the horizontal corner radius of the rectangular border.</summary>
        /// <remarks>
        /// Gets the horizontal corner radius of the rectangular border.
        /// Corresponds to array index 0 in the Border key in the common annotation dictionary.
        /// </remarks>
        /// <returns>a float value specifying the horizontal corner radius.</returns>
        public virtual float GetHCornerRadius() {
            return hCornerRadius;
        }

        /// <summary>Gets the vertical corner radius of the rectangular border.</summary>
        /// <remarks>
        /// Gets the vertical corner radius of the rectangular border.
        /// Corresponds to array index 1 in the Border key in the common annotation dictionary.
        /// </remarks>
        /// <returns>a float value specifying the vertical corner radius.</returns>
        public virtual float GetVCornerRadius() {
            return vCornerRadius;
        }

        /// <summary>Gets the width of the rectangular border.</summary>
        /// <remarks>
        /// Gets the width of the rectangular border.
        /// Corresponds to array index 2 in the Border key in the common annotation dictionary.
        /// </remarks>
        /// <returns>a float value specifying the width of the border.</returns>
        public virtual float GetWidth() {
            return width;
        }

        /// <summary>Gets the dash pattern of the border.</summary>
        /// <remarks>
        /// Gets the dash pattern of the border.
        /// Corresponds to array index 3 in the Border key in the common annotation dictionary.
        /// </remarks>
        /// <returns>an array of numbers specifying the pattern of dashes and gaps of the border.</returns>
        public virtual float[] GetDashPattern() {
            return dashPattern;
        }

        /// <summary>Sets the dash pattern of the border.</summary>
        /// <remarks>
        /// Sets the dash pattern of the border.
        /// Corresponds to array index 3 in the Border key in the common annotation dictionary.
        /// </remarks>
        /// <param name="dashPattern">an array of numbers specifying the pattern of dashes and gaps of the border.</param>
        /// <returns>
        /// this
        /// <see cref="BorderStyleAltObject"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Xfdf.BorderStyleAltObject SetDashPattern(float[] dashPattern) {
            this.dashPattern = dashPattern;
            return this;
        }

        /// <summary>Gets border style.</summary>
        /// <returns>an encoded border style as string.</returns>
        public virtual String GetContent() {
            return content;
        }

        /// <summary>Sets border style.</summary>
        /// <param name="content">an encoded border style as string.</param>
        /// <returns>
        /// this
        /// <see cref="BorderStyleAltObject"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Xfdf.BorderStyleAltObject SetContent(String content) {
            this.content = content;
            return this;
        }
    }
}
