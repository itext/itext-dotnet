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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace iText.Kernel.Colors.Gradients {
    /// <summary>
    /// Contract for gradient builders that can produce
    /// <see cref="iText.Kernel.Colors.Color"/>
    /// instances.
    /// </summary>
    public interface IGradientBuilder {
        /// <summary>
        /// Adds the new color stop to the end (
        /// <see cref="IGradientBuilder">more info</see>
        /// ).
        /// </summary>
        /// <remarks>
        /// Adds the new color stop to the end (
        /// <see cref="IGradientBuilder">more info</see>
        /// ).
        /// <para />
        /// Note: if the previously added color stop's offset would have greater offset than the added
        /// one, then the new offset would be normalized to be equal to the previous one. (Comparison
        /// made between relative on coordinates vector offsets. If any of them has
        /// the absolute offset, then the absolute value would be converted to relative first.)
        /// </remarks>
        /// <param name="gradientColorStop">the gradient stop color to add</param>
        /// <returns>the current builder instance</returns>
        IGradientBuilder AddStopColor(GradientColorStop gradientColorStop);

        /// <summary>Set the spread method to use for the gradient.</summary>
        /// <param name="gradientSpreadMethod">the gradient spread method to set</param>
        /// <returns>the current builder instance</returns>
        IGradientBuilder SetSpread(GradientSpreadMethod gradientSpreadMethod);

        /// <summary>
        /// Builds the
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// object representing the gradient with specified configuration
        /// that fills the target bounding box.
        /// </summary>
        /// <param name="targetBoundingBox">the bounding box to be filled in current space</param>
        /// <param name="contextTransform">
        /// the transformation from the base coordinates space into
        /// the current space. The
        /// <see langword="null"/>
        /// value is valid and can be used
        /// if there is no transformation from base coordinates to current space
        /// specified, or it is equal to identity transformation.
        /// </param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// for which the linear gradient would be built.
        /// </param>
        /// <returns>
        /// the constructed
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// or
        /// <see langword="null"/>
        /// if no color to be applied
        /// or base gradient vector has been specified
        /// </returns>
        Color BuildColor(Rectangle targetBoundingBox, AffineTransform contextTransform, PdfDocument document);
    }
}
