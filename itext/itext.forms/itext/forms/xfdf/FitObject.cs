/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Forms.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Forms.Xfdf {
    /// <summary>Represent Fit, a child of the Dest element.</summary>
    /// <remarks>
    /// Represent Fit, a child of the Dest element.
    /// Content model: none.
    /// Attributes: depends of type of Fit (FitH, FitB, FitV etc.).
    /// For more details see paragraphs 6.5.13-6.5.19, 6.6.23 in Xfdf specification.
    /// </remarks>
    public class FitObject {
        /// <summary>Represents the page displayed by current Fit element.</summary>
        /// <remarks>
        /// Represents the page displayed by current Fit element.
        /// Attribute of Fit, FitB, FitBH, FitBV, FitH, FitR, FitV, XYZ elements.
        /// </remarks>
        private readonly PdfObject page;

        /// <summary>Vertical coordinate positioned at the top edge of the window.</summary>
        private float top;

        /// <summary>Vertical coordinate positioned at the bottom edge of the window.</summary>
        private float bottom;

        /// <summary>Horizontal coordinate positioned at the left edge of the window.</summary>
        private float left;

        /// <summary>Horizontal coordinate positioned at the right edge of the window.</summary>
        private float right;

        /// <summary>Corresponds to the zoom object in the destination syntax.</summary>
        /// <remarks>
        /// Corresponds to the zoom object in the destination syntax.
        /// Attribute of XYZ object.
        /// </remarks>
        private float zoom;

        /// <summary>
        /// Creates an instance of
        /// <see cref="FitObject"/>.
        /// </summary>
        /// <param name="page">the page displayed by current Fit element</param>
        public FitObject(PdfObject page) {
            if (page == null) {
                throw new XfdfException(XfdfException.PAGE_IS_MISSING);
            }
            this.page = page;
        }

        /// <summary>Gets the PdfObject representing the page displayed by current Fit element.</summary>
        /// <remarks>
        /// Gets the PdfObject representing the page displayed by current Fit element.
        /// Attribute of Fit, FitB, FitBH, FitBV, FitH, FitR, FitV, XYZ elements.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfObject">page</see>
        /// of the current Fit element.
        /// </returns>
        public virtual PdfObject GetPage() {
            return page;
        }

        /// <summary>Gets a float vertical coordinate positioned at the top edge of the window.</summary>
        /// <returns>top vertical coordinate.</returns>
        public virtual float GetTop() {
            return top;
        }

        /// <summary>Sets a float vertical coordinate positioned at the top edge of the window.</summary>
        /// <param name="top">vertical coordinate value</param>
        /// <returns>
        /// current
        /// <see cref="FitObject">fit object</see>.
        /// </returns>
        public virtual iText.Forms.Xfdf.FitObject SetTop(float top) {
            this.top = top;
            return this;
        }

        /// <summary>Gets a float horizontal coordinate positioned at the left edge of the window.</summary>
        /// <returns>left horizontal coordinate.</returns>
        public virtual float GetLeft() {
            return left;
        }

        /// <summary>Sets a float horizontal coordinate positioned at the left edge of the window.</summary>
        /// <param name="left">horizontal coordinate value</param>
        /// <returns>
        /// current
        /// <see cref="FitObject">fit object</see>.
        /// </returns>
        public virtual iText.Forms.Xfdf.FitObject SetLeft(float left) {
            this.left = left;
            return this;
        }

        /// <summary>Gets a float vertical coordinate positioned at the bottom edge of the window.</summary>
        /// <returns>bottom vertical coordinate.</returns>
        public virtual float GetBottom() {
            return bottom;
        }

        /// <summary>Sets a float vertical coordinate positioned at the bottom edge of the window.</summary>
        /// <param name="bottom">vertical coordinate value</param>
        /// <returns>
        /// current
        /// <see cref="FitObject">fit object</see>.
        /// </returns>
        public virtual iText.Forms.Xfdf.FitObject SetBottom(float bottom) {
            this.bottom = bottom;
            return this;
        }

        /// <summary>Gets a float horizontal coordinate positioned at the right edge of the window.</summary>
        /// <returns>right horizontal coordinate.</returns>
        public virtual float GetRight() {
            return right;
        }

        /// <summary>Sets a float horizontal coordinate positioned at the right edge of the window.</summary>
        /// <param name="right">horizontal coordinate</param>
        /// <returns>
        /// current
        /// <see cref="FitObject">fit object</see>.
        /// </returns>
        public virtual iText.Forms.Xfdf.FitObject SetRight(float right) {
            this.right = right;
            return this;
        }

        /// <summary>Gets a float representing the zoom ratio.</summary>
        /// <remarks>
        /// Gets a float representing the zoom ratio.
        /// Attribute of XYZ object.
        /// </remarks>
        /// <returns>zoom ratio value.</returns>
        public virtual float GetZoom() {
            return zoom;
        }

        /// <summary>Sets a float representing the zoom ratio.</summary>
        /// <remarks>
        /// Sets a float representing the zoom ratio.
        /// Attribute of XYZ object.
        /// </remarks>
        /// <param name="zoom">ratio value</param>
        /// <returns>
        /// current
        /// <see cref="FitObject">fit object</see>.
        /// </returns>
        public virtual iText.Forms.Xfdf.FitObject SetZoom(float zoom) {
            this.zoom = zoom;
            return this;
        }
    }
}
