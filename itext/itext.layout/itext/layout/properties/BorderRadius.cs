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
namespace iText.Layout.Properties {
    /// <summary>Represents a border radius.</summary>
    public class BorderRadius {
        /// <summary>The horizontal semi-major axis of the ellipse to use for the border in that corner.</summary>
        private UnitValue horizontalRadius;

        /// <summary>The vertical semi-major axis of the ellipse to use for the border in that corner.</summary>
        private UnitValue verticalRadius;

        /// <summary>
        /// Creates a
        /// <see cref="BorderRadius">border radius</see>
        /// with given value.
        /// </summary>
        /// <param name="radius">the radius</param>
        public BorderRadius(UnitValue radius) {
            this.horizontalRadius = radius;
            this.verticalRadius = radius;
        }

        /// <summary>
        /// Creates a
        /// <see cref="BorderRadius">border radius</see>
        /// with a given point value.
        /// </summary>
        /// <param name="radius">the radius</param>
        public BorderRadius(float radius) {
            this.horizontalRadius = UnitValue.CreatePointValue(radius);
            this.verticalRadius = this.horizontalRadius;
        }

        /// <summary>
        /// Creates a
        /// <see cref="BorderRadius">border radius</see>
        /// with given horizontal and vertical values.
        /// </summary>
        /// <param name="horizontalRadius">the horizontal radius of the corner</param>
        /// <param name="verticalRadius">the vertical radius of the corner</param>
        public BorderRadius(UnitValue horizontalRadius, UnitValue verticalRadius) {
            this.horizontalRadius = horizontalRadius;
            this.verticalRadius = verticalRadius;
        }

        /// <summary>
        /// Creates a
        /// <see cref="BorderRadius">border radius</see>
        /// with given horizontal and vertical point values.
        /// </summary>
        /// <param name="horizontalRadius">the horizontal radius of the corner</param>
        /// <param name="verticalRadius">the vertical radius of the corner</param>
        public BorderRadius(float horizontalRadius, float verticalRadius) {
            this.horizontalRadius = UnitValue.CreatePointValue(horizontalRadius);
            this.verticalRadius = UnitValue.CreatePointValue(verticalRadius);
        }

        /// <summary>
        /// Gets the horizontal radius of the
        /// <see cref="iText.Layout.Borders.Border">border's</see>
        /// corner.
        /// </summary>
        /// <returns>the horizontal radius</returns>
        public virtual UnitValue GetHorizontalRadius() {
            return horizontalRadius;
        }

        /// <summary>
        /// Gets the vertical radius of the
        /// <see cref="iText.Layout.Borders.Border">border's</see>
        /// corner.
        /// </summary>
        /// <returns>the vertical radius</returns>
        public virtual UnitValue GetVerticalRadius() {
            return verticalRadius;
        }
    }
}
