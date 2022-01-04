/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using iText.Kernel.Geom;

namespace iText.Svg.Utils {
    /// <summary>A rectangle adapted for working with text elements.</summary>
    public class TextRectangle : Rectangle {
        /// <summary>The y coordinate of the line on which the text is located.</summary>
        private float textBaseLineYCoordinate;

        /// <summary>Create new instance of text rectangle.</summary>
        /// <param name="x">the x coordinate of lower left point</param>
        /// <param name="y">the y coordinate of lower left point</param>
        /// <param name="width">the width value</param>
        /// <param name="height">the height value</param>
        /// <param name="textBaseLineYCoordinate">the y coordinate of the line on which the text is located.</param>
        public TextRectangle(float x, float y, float width, float height, float textBaseLineYCoordinate)
            : base(x, y, width, height) {
            this.textBaseLineYCoordinate = textBaseLineYCoordinate;
        }

        /// <summary>Return the far right point of the rectangle with y on the baseline.</summary>
        /// <returns>the far right baseline point</returns>
        public virtual Point GetTextBaseLineRightPoint() {
            return new Point(GetRight(), textBaseLineYCoordinate);
        }
    }
}
