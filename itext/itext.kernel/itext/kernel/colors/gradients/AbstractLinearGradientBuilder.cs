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
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Colorspace.Shading;
using iText.Kernel.Pdf.Function;

namespace iText.Kernel.Colors.Gradients {
    /// <summary>Base class for linear gradient builders implementations.</summary>
    /// <remarks>
    /// Base class for linear gradient builders implementations.
    /// <para />
    /// Color transitions for linear gradients are defined by a series of color stops along a gradient
    /// vector. A gradient normal defines how the colors in a vector are painted to the surface. For
    /// a linear gradient, a normal is a line perpendicular to the vector.
    /// <para />
    /// Contains the main logic that works with stop colors and creation of the resulted pdf color object.
    /// </remarks>
    public abstract class AbstractLinearGradientBuilder : AbstractGradientBuilder<Point> {
        /// <summary>The epsilon value used for data creation</summary>
        [System.ObsoleteAttribute(@"use AbstractGradientBuilder{T}.IsZero(double) instead for zero comparisons")]
        protected internal const double ZERO_EPSILON = 1E-10;

        /// <summary>
        /// Adds the new color stop to the end (
        /// <see cref="AbstractLinearGradientBuilder">more info</see>
        /// ).
        /// </summary>
        /// <remarks>
        /// Adds the new color stop to the end (
        /// <see cref="AbstractLinearGradientBuilder">more info</see>
        /// ).
        /// <para />
        /// Note: if the previously added color stop's offset would have grater offset than the added
        /// one, then the new offset would be normalized to be equal to the previous one. (Comparison
        /// made between relative on coordinates vector offsets. If any of them has
        /// the absolute offset, then the absolute value would converted to relative first.)
        /// </remarks>
        /// <param name="gradientColorStop">the gradient stop color to add</param>
        /// <returns>the current builder instance</returns>
        [System.ObsoleteAttribute(@"use AbstractGradientBuilder{T}.AddStopColor(GradientColorStop) instead")]
        public virtual AbstractLinearGradientBuilder AddColorStop(GradientColorStop gradientColorStop) {
            base.AddStopColor(gradientColorStop);
            return this;
        }

        /// <summary>Set the spread method to use for the gradient</summary>
        /// <param name="gradientSpreadMethod">the gradient spread method to set</param>
        /// <returns>the current builder instance</returns>
        [System.ObsoleteAttribute(@"use AbstractGradientBuilder{T}.SetSpread(GradientSpreadMethod) instead")]
        public virtual AbstractLinearGradientBuilder SetSpreadMethod(GradientSpreadMethod gradientSpreadMethod) {
            base.SetSpread(gradientSpreadMethod);
            return this;
        }

        /// <summary>Get the copy of current color stops list.</summary>
        /// <remarks>Get the copy of current color stops list. Note that the stop colors are not copied here</remarks>
        /// <returns>the copy of current stop colors list</returns>
        [System.ObsoleteAttribute(@"use AbstractGradientBuilder{T}.GetStopColors() instead")]
        public virtual IList<GradientColorStop> GetColorStops() {
            return base.GetStopColors();
        }

        /// <summary>Get the current spread method</summary>
        /// <returns>the current spread method</returns>
        [System.ObsoleteAttribute(@"use AbstractGradientBuilder{T}.GetSpread() instead")]
        public virtual GradientSpreadMethod GetSpreadMethod() {
            return base.GetSpread();
        }

        /// <summary>Returns the base gradient vector in gradient vector space.</summary>
        /// <remarks>
        /// Returns the base gradient vector in gradient vector space. This vector would be set
        /// as shading coordinates vector and its length would be used to translate all color stops
        /// absolute offsets into the relatives.
        /// </remarks>
        /// <param name="targetBoundingBox">the rectangle to be covered by constructed color in current space</param>
        /// <param name="contextTransform">the current canvas transformation</param>
        /// <returns>the array of exactly two elements specifying the gradient coordinates vector</returns>
        [System.ObsoleteAttribute(@"use AbstractGradientBuilder{T}.GetGradientVectorWithTransform(iText.Kernel.Geom.Rectangle, iText.Kernel.Geom.AffineTransform)"
            )]
        protected internal abstract Point[] GetGradientVector(Rectangle targetBoundingBox, AffineTransform contextTransform
            );

        /// <summary>
        /// Returns the current space to gradient vector space transformations that should be applied
        /// to the shading color.
        /// </summary>
        /// <remarks>
        /// Returns the current space to gradient vector space transformations that should be applied
        /// to the shading color. The transformation should be invertible as the current target
        /// bounding box coordinates should be transformed into the resulted shading space coordinates.
        /// </remarks>
        /// <param name="targetBoundingBox">the rectangle to be covered by constructed color in current space</param>
        /// <param name="contextTransform">the current canvas transformation</param>
        /// <returns>
        /// the additional transformation to be concatenated to the current for resulted shading
        /// or
        /// <see langword="null"/>
        /// if no additional transformation is specified
        /// </returns>
        [System.ObsoleteAttribute(@"use AbstractGradientBuilder{T}.GetGradientVectorWithTransform(iText.Kernel.Geom.Rectangle, iText.Kernel.Geom.AffineTransform)"
            )]
        protected internal virtual AffineTransform GetCurrentSpaceToGradientVectorSpaceTransformation(Rectangle targetBoundingBox
            , AffineTransform contextTransform) {
            return null;
        }

        /// <summary>Evaluates the minimal domain that covers the box with vector normals.</summary>
        /// <remarks>
        /// Evaluates the minimal domain that covers the box with vector normals.
        /// The domain corresponding to the initial vector is [0, 1].
        /// </remarks>
        /// <param name="coords">
        /// the array of exactly two elements that describe
        /// the base vector (corresponding to [0,1] domain, that need to be adjusted
        /// to cover the box
        /// </param>
        /// <param name="toCover">the box that needs to be covered</param>
        /// <returns>
        /// the array of two elements in ascending order specifying the calculated covering
        /// domain
        /// </returns>
        protected internal static double[] EvaluateCoveringDomain(Point[] coords, Rectangle toCover) {
            // TODO: DEVSIX-8808 move the implementation directly into the place where we call this method
            if (toCover == null) {
                return new double[] { 0d, 1d };
            }
            double scale = 1d / (coords[0].Distance(coords[1]));
            AffineTransform transform = GetToIntervalTransform(coords[0], coords[1], scale);
            Point[] rectanglePoints = toCover.ToPointsArray();
            double minX = transform.Transform(rectanglePoints[0], null).GetX();
            double maxX = minX;
            for (int i = 1; i < rectanglePoints.Length; ++i) {
                double currentX = transform.Transform(rectanglePoints[i], null).GetX();
                minX = Math.Min(minX, currentX);
                maxX = Math.Max(maxX, currentX);
            }
            return new double[] { minX, maxX };
        }

        /// <summary>Expand the base vector to cover the new domain</summary>
        /// <param name="newDomain">
        /// the array of exactly two elements that specifies the domain
        /// that should be covered by the created vector
        /// </param>
        /// <param name="baseVector">
        /// the array of exactly two elements that specifies the base vector
        /// which corresponds to [0, 1] domain
        /// </param>
        /// <returns>the array of two</returns>
        protected internal static Point[] CreateCoordinatesForNewDomain(double[] newDomain, Point[] baseVector) {
            // TODO: DEVSIX-8808 move the implementation directly into the place where we call this method
            double xDiff = baseVector[1].GetX() - baseVector[0].GetX();
            double yDiff = baseVector[1].GetY() - baseVector[0].GetY();
            Point[] targetCoords = new Point[] { baseVector[0].GetLocation(), baseVector[1].GetLocation() };
            targetCoords[0].Move(xDiff * newDomain[0], yDiff * newDomain[0]);
            targetCoords[1].Move(xDiff * (newDomain[1] - 1), yDiff * (newDomain[1] - 1));
            return targetCoords;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override double[] ComputeCoveringDomain(Point[] coords, Rectangle toCover) {
            return EvaluateCoveringDomain(coords, toCover);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override Point[] CreateCoordsForNewDomain(double[] newDomain, Point[] baseVector) {
            return CreateCoordinatesForNewDomain(newDomain, baseVector);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override Point[] CreateCoveringCoordinates(Rectangle targetBoundingBox) {
            return new Point[] { new Point(targetBoundingBox.GetLeft(), targetBoundingBox.GetBottom()), new Point(targetBoundingBox
                .GetRight(), targetBoundingBox.GetBottom()) };
        }

        /// <summary><inheritDoc/></summary>
        protected internal override double GetBaseVectorLength(Point[] coordinates) {
            return coordinates[1].Distance(coordinates[0]);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override PdfArray CreateCoordsDictEntry(Point[] coordinates) {
            System.Diagnostics.Debug.Assert(coordinates != null && coordinates.Length == 2);
            return new PdfArray(new double[] { coordinates[0].GetX(), coordinates[0].GetY(), coordinates[1].GetX(), coordinates
                [1].GetY() });
        }

        /// <summary><inheritDoc/></summary>
        protected internal override AbstractPdfShading CreatePdfShading(PdfColorSpace colorSpace, PdfArray coordinates
            , PdfArray coordinatesDomain, IPdfFunction stopsFunction) {
            return new PdfAxialShading(colorSpace, coordinates, coordinatesDomain, stopsFunction);
        }
    }
}
