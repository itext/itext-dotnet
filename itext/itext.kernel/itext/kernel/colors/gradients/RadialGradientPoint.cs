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
using iText.Kernel.Geom;

namespace iText.Kernel.Colors.Gradients {
    /// <summary>Radial gradient vector point represented as circle with center and radius.</summary>
    public class RadialGradientPoint {
        private readonly Point center;

        private double radius;

        /// <summary>Creates an instance with (0,0) center and 0 radius.</summary>
        public RadialGradientPoint()
            : this(new Point(), 0d) {
        }

        /// <summary>Creates an instance with specified center and radius.</summary>
        /// <param name="center">the center of the circle</param>
        /// <param name="radius">the radius of the circle</param>
        public RadialGradientPoint(Point center, double radius) {
            this.center = center;
            this.radius = radius;
        }

        /// <summary>Copy constructor.</summary>
        /// <param name="point">the point to copy</param>
        public RadialGradientPoint(iText.Kernel.Colors.Gradients.RadialGradientPoint point)
            : this(point.center.GetLocation(), point.radius) {
        }

        /// <summary>
        /// Get the center as a
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// instance
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// object representing the circle center
        /// </returns>
        public virtual Point GetCenter() {
            return center;
        }

        /// <summary>Get the radius.</summary>
        /// <returns>the radius</returns>
        public virtual double GetRadius() {
            return radius;
        }

        /// <summary>Set the radius.</summary>
        /// <param name="radius">the radius value</param>
        public virtual void SetRadius(double radius) {
            this.radius = radius;
        }

        /// <summary>Get the X coordinate of the center.</summary>
        /// <returns>the X coordinate</returns>
        public virtual double GetX() {
            return this.center.GetX();
        }

        /// <summary>Get the Y coordinate of the center.</summary>
        /// <returns>the Y coordinate</returns>
        public virtual double GetY() {
            return this.center.GetY();
        }

        /// <summary>Get the distance between this and other point centers.</summary>
        /// <param name="c">the point to which center the distance is needed</param>
        /// <returns>the distance value</returns>
        public virtual double Distance(iText.Kernel.Colors.Gradients.RadialGradientPoint c) {
            return this.center.Distance(c.center);
        }
    }
}
