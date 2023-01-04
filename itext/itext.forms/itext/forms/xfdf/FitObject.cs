/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
        private PdfObject page;

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
        /// of the current Fit element
        /// </returns>
        public virtual PdfObject GetPage() {
            return page;
        }

        /// <summary>Gets a float vertical coordinate positioned at the top edge of the window.</summary>
        /// <returns>top vertical coordinate</returns>
        public virtual float GetTop() {
            return top;
        }

        /// <summary>Sets a float vertical coordinate positioned at the top edge of the window.</summary>
        /// <param name="top">vertical coordinate value</param>
        /// <returns>
        /// current
        /// <see cref="FitObject">fit object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.FitObject SetTop(float top) {
            this.top = top;
            return this;
        }

        /// <summary>Gets a float horizontal coordinate positioned at the left edge of the window.</summary>
        /// <returns>left horizontal coordinate</returns>
        public virtual float GetLeft() {
            return left;
        }

        /// <summary>Sets a float horizontal coordinate positioned at the left edge of the window.</summary>
        /// <param name="left">horizontal coordinate value</param>
        /// <returns>
        /// current
        /// <see cref="FitObject">fit object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.FitObject SetLeft(float left) {
            this.left = left;
            return this;
        }

        /// <summary>Gets a float vertical coordinate positioned at the bottom edge of the window.</summary>
        /// <returns>bottom vertical coordinate</returns>
        public virtual float GetBottom() {
            return bottom;
        }

        /// <summary>Sets a float vertical coordinate positioned at the bottom edge of the window.</summary>
        /// <param name="bottom">vertical coordinate value</param>
        /// <returns>
        /// current
        /// <see cref="FitObject">fit object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.FitObject SetBottom(float bottom) {
            this.bottom = bottom;
            return this;
        }

        /// <summary>Gets a float horizontal coordinate positioned at the right edge of the window.</summary>
        /// <returns>right horizontal coordinate</returns>
        public virtual float GetRight() {
            return right;
        }

        /// <summary>Sets a float horizontal coordinate positioned at the right edge of the window.</summary>
        /// <param name="right">horizontal coordinate</param>
        /// <returns>
        /// current
        /// <see cref="FitObject">fit object</see>
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
        /// <returns>zoom ratio value</returns>
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
        /// <see cref="FitObject">fit object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.FitObject SetZoom(float zoom) {
            this.zoom = zoom;
            return this;
        }
    }
}
