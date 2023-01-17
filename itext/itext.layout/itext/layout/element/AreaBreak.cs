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
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>
    /// A layout object that terminates the current content area and creates a new
    /// one.
    /// </summary>
    /// <remarks>
    /// A layout object that terminates the current content area and creates a new
    /// one. If no
    /// <see cref="iText.Kernel.Geom.PageSize"/>
    /// is given, the new content area will have the same
    /// size as the current one.
    /// </remarks>
    public class AreaBreak : AbstractElement<iText.Layout.Element.AreaBreak> {
        protected internal PageSize pageSize;

        /// <summary>Creates an AreaBreak.</summary>
        /// <remarks>
        /// Creates an AreaBreak. The new content area will have the same size as the
        /// current one.
        /// </remarks>
        public AreaBreak()
            : this(AreaBreakType.NEXT_AREA) {
        }

        /// <summary>Creates an AreaBreak that terminates a specified area type.</summary>
        /// <param name="areaBreakType">
        /// an
        /// <see cref="iText.Layout.Properties.AreaBreakType?">area break type</see>
        /// </param>
        public AreaBreak(AreaBreakType? areaBreakType) {
            SetProperty(Property.AREA_BREAK_TYPE, areaBreakType);
        }

        /// <summary>Creates an AreaBreak.</summary>
        /// <remarks>
        /// Creates an AreaBreak. The new content area will have the specified page
        /// size.
        /// </remarks>
        /// <param name="pageSize">the size of the new content area</param>
        public AreaBreak(PageSize pageSize)
            : this(AreaBreakType.NEXT_PAGE) {
            this.pageSize = pageSize;
        }

        /// <summary>Gets the page size.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Geom.PageSize">page size</see>
        /// of the next content area.
        /// </returns>
        public virtual PageSize GetPageSize() {
            return pageSize;
        }

        /// <summary>Sets the page size.</summary>
        /// <param name="pageSize">
        /// the new
        /// <see cref="iText.Kernel.Geom.PageSize">page size</see>
        /// of the next content area.
        /// </param>
        public virtual void SetPageSize(PageSize pageSize) {
            this.pageSize = pageSize;
        }

        /// <summary>Gets the type of area that this AreaBreak will terminate.</summary>
        /// <returns>
        /// the current
        /// <see cref="iText.Layout.Properties.AreaBreakType?">area break type</see>
        /// </returns>
        public virtual AreaBreakType? GetAreaType() {
            return this.GetProperty<AreaBreakType?>(Property.AREA_BREAK_TYPE);
        }

        protected internal override IRenderer MakeNewRenderer() {
            return new AreaBreakRenderer(this);
        }
    }
}
