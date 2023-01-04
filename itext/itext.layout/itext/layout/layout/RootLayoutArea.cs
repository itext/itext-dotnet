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
using iText.Kernel.Geom;

namespace iText.Layout.Layout {
    /// <summary>Represents the root layout area.</summary>
    public class RootLayoutArea : LayoutArea {
        /// <summary>Indicates whether the area already has some placed content or not.</summary>
        protected internal bool emptyArea = true;

        /// <summary>Creates the root layout area.</summary>
        /// <param name="pageNumber">the value number of page</param>
        /// <param name="bBox">the bounding box</param>
        public RootLayoutArea(int pageNumber, Rectangle bBox)
            : base(pageNumber, bBox) {
        }

        /// <summary>Indicates whether the area already has some placed content or not.</summary>
        /// <returns>whether the area is empty or not</returns>
        public virtual bool IsEmptyArea() {
            return emptyArea;
        }

        /// <summary>Defines whether the area already has some placed content or not.</summary>
        /// <param name="emptyArea">indicates whether the area already has some placed content or not.</param>
        public virtual void SetEmptyArea(bool emptyArea) {
            this.emptyArea = emptyArea;
        }

        /// <summary>
        /// Creates a "deep copy" of this RootLayoutArea, meaning the object returned by this method will be independent
        /// of the object being cloned.
        /// </summary>
        /// <remarks>
        /// Creates a "deep copy" of this RootLayoutArea, meaning the object returned by this method will be independent
        /// of the object being cloned.
        /// Note that although the return type of this method is
        /// <see cref="LayoutArea"/>
        /// ,
        /// the actual type of the returned object is
        /// <see cref="RootLayoutArea"/>.
        /// </remarks>
        /// <returns>the copied RootLayoutArea.</returns>
        public override LayoutArea Clone() {
            iText.Layout.Layout.RootLayoutArea area = (iText.Layout.Layout.RootLayoutArea)base.Clone();
            area.SetEmptyArea(emptyArea);
            return area;
        }
    }
}
