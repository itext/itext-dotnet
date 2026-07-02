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
using iText.Commons.Datastructures;
using iText.Kernel.Geom;

namespace iText.Kernel.Colors.Gradients {
    /// <summary>
    /// The radial gradient builder implementation with direct target gradient vector
    /// and shading transformation (
    /// <see cref="AbstractRadialGradientBuilder">more info</see>
    /// )
    /// </summary>
    public class RadialGradientBuilder : AbstractRadialGradientBuilder {
        private readonly RadialGradientPoint[] coordinates = new RadialGradientPoint[] { new RadialGradientPoint()
            , new RadialGradientPoint() };

        private AffineTransform transformation = null;

        /// <summary>Constructs the builder instance</summary>
        public RadialGradientBuilder() {
        }

        // empty constructor
        /// <summary>
        /// Set coordinates for gradient vector circles (
        /// <see cref="AbstractRadialGradientBuilder">more info</see>
        /// )
        /// </summary>
        /// <param name="x0">the x coordinate of the vector start circle</param>
        /// <param name="y0">the y coordinate of the vector start circle</param>
        /// <param name="r0">the radius of the vector start circle</param>
        /// <param name="x1">the x coordinate of the vector end circle</param>
        /// <param name="y1">the y coordinate of the vector end circle</param>
        /// <param name="r1">the radius of the vector end circle</param>
        /// <returns>the current builder instance</returns>
        public virtual iText.Kernel.Colors.Gradients.RadialGradientBuilder SetGradientVector(double x0, double y0, 
            double r0, double x1, double y1, double r1) {
            this.coordinates[0].GetCenter().SetLocation(x0, y0);
            this.coordinates[0].SetRadius(r0);
            this.coordinates[1].GetCenter().SetLocation(x1, y1);
            this.coordinates[1].SetRadius(r1);
            return this;
        }

        /// <summary>
        /// Set the radial gradient space transformation which specifies the transformation from
        /// the current coordinates space to gradient vector space.
        /// </summary>
        /// <remarks>
        /// Set the radial gradient space transformation which specifies the transformation from
        /// the current coordinates space to gradient vector space.
        /// <para />
        /// The current space is the one on which radial gradient will be drawn (as a fill or stroke
        /// color for shapes on PDF canvas). This transformation mainly used for ellipse based gradient.
        /// </remarks>
        /// <param name="transformation">
        /// the
        /// <see cref="iText.Kernel.Geom.AffineTransform"/>
        /// representing the transformation to set
        /// </param>
        /// <returns>the current builder instance</returns>
        public virtual iText.Kernel.Colors.Gradients.RadialGradientBuilder SetCurrentSpaceToGradientVectorSpaceTransformation
            (AffineTransform transformation) {
            this.transformation = transformation;
            return this;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override Tuple2<RadialGradientPoint[], AffineTransform> GetGradientVectorWithTransform(
            Rectangle targetBoundingBox, AffineTransform contextTransform) {
            return new Tuple2<RadialGradientPoint[], AffineTransform>(new RadialGradientPoint[] { new RadialGradientPoint
                (this.coordinates[0]), new RadialGradientPoint(this.coordinates[1]) }, this.transformation);
        }
    }
}
