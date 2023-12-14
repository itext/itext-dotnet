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
using iText.Kernel.Geom;

namespace iText.Kernel.Colors.Gradients {
    /// <summary>
    /// The linear gradient builder implementation with direct target gradient vector
    /// and shading transformation (
    /// <see cref="AbstractLinearGradientBuilder">more info</see>
    /// )
    /// </summary>
    public class LinearGradientBuilder : AbstractLinearGradientBuilder {
        private readonly Point[] coordinates = new Point[] { new Point(), new Point() };

        private AffineTransform transformation = null;

        /// <summary>Constructs the builder instance</summary>
        public LinearGradientBuilder() {
        }

        /// <summary>
        /// Set coordinates for gradient vector (
        /// <see cref="AbstractLinearGradientBuilder">more info</see>
        /// )
        /// </summary>
        /// <param name="x0">the x coordinate of the vector start</param>
        /// <param name="y0">the y coordinate of the vector start</param>
        /// <param name="x1">the x coordinate of the vector end</param>
        /// <param name="y1">the y coordinate of the vector end</param>
        /// <returns>the current builder instance</returns>
        public virtual iText.Kernel.Colors.Gradients.LinearGradientBuilder SetGradientVector(double x0, double y0, 
            double x1, double y1) {
            this.coordinates[0].SetLocation(x0, y0);
            this.coordinates[1].SetLocation(x1, y1);
            return this;
        }

        /// <summary>
        /// Set the linear gradient space transformation which specifies the transformation from
        /// the current coordinates space to gradient vector space
        /// </summary>
        /// <remarks>
        /// Set the linear gradient space transformation which specifies the transformation from
        /// the current coordinates space to gradient vector space
        /// <para />
        /// The current space is the one on which linear gradient will be drawn (as a fill or stroke
        /// color for shapes on PDF canvas). This transformation mainly used for color lines skewing.
        /// </remarks>
        /// <param name="transformation">
        /// the
        /// <see cref="iText.Kernel.Geom.AffineTransform"/>
        /// representing the transformation to set
        /// </param>
        /// <returns>the current builder instance</returns>
        public virtual iText.Kernel.Colors.Gradients.LinearGradientBuilder SetCurrentSpaceToGradientVectorSpaceTransformation
            (AffineTransform transformation) {
            this.transformation = transformation;
            return this;
        }

        protected internal override Point[] GetGradientVector(Rectangle targetBoundingBox, AffineTransform contextTransform
            ) {
            return new Point[] { this.coordinates[0].GetLocation(), this.coordinates[1].GetLocation() };
        }

        protected internal override AffineTransform GetCurrentSpaceToGradientVectorSpaceTransformation(Rectangle targetBoundingBox
            , AffineTransform contextTransform) {
            return this.transformation;
        }
    }
}
