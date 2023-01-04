/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using System;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Kernel.Geom;

namespace iText.Layout.Layout {
    /// <summary>
    /// Represents the area for content
    /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layouting</see>.
    /// </summary>
    public class LayoutArea {
        /// <summary>The number of page on which the area is located.</summary>
        protected internal int pageNumber;

        /// <summary>The area's bounding box</summary>
        protected internal Rectangle bBox;

        /// <summary>
        /// Creates the area for content
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layouting</see>.
        /// </summary>
        /// <param name="pageNumber">the number of page on which the area is located.</param>
        /// <param name="bBox">the area's bounding box</param>
        public LayoutArea(int pageNumber, Rectangle bBox) {
            this.pageNumber = pageNumber;
            this.bBox = bBox;
        }

        /// <summary>Gets the number of page on which the area is located.</summary>
        /// <returns>page number</returns>
        public virtual int GetPageNumber() {
            return pageNumber;
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Geom.Rectangle">box</see>
        /// which bounds the area.
        /// </summary>
        /// <returns>the bounding box</returns>
        public virtual Rectangle GetBBox() {
            return bBox;
        }

        /// <summary>
        /// Sets the
        /// <see cref="iText.Kernel.Geom.Rectangle">box</see>
        /// which bounds the area.
        /// </summary>
        /// <param name="bbox">the area's bounding box</param>
        public virtual void SetBBox(Rectangle bbox) {
            this.bBox = bbox;
        }

        /// <summary>
        /// Creates a "deep copy" of this LayoutArea, meaning the object returned by this method will be independent
        /// of the object being cloned.
        /// </summary>
        /// <returns>the copied LayoutArea.</returns>
        public virtual iText.Layout.Layout.LayoutArea Clone() {
            iText.Layout.Layout.LayoutArea clone = (iText.Layout.Layout.LayoutArea) MemberwiseClone();
            clone.bBox = bBox.Clone();
            return clone;
        }

        /// <summary><inheritDoc/></summary>
        public override bool Equals(Object obj) {
            if (GetType() != obj.GetType()) {
                return false;
            }
            iText.Layout.Layout.LayoutArea that = (iText.Layout.Layout.LayoutArea)obj;
            return pageNumber == that.pageNumber && bBox.EqualsWithEpsilon(that.bBox);
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            HashCode hashCode = new HashCode();
            hashCode.Append(pageNumber).Append(bBox.GetHashCode());
            return hashCode.GetHashCode();
        }

        /// <summary><inheritDoc/></summary>
        public override String ToString() {
            return MessageFormatUtil.Format("{0}, page {1}", bBox.ToString(), pageNumber);
        }
    }
}
