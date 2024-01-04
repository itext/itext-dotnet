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
