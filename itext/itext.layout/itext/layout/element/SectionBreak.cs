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
using iText.Kernel.Geom;
using iText.Layout.Properties.Margins;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>A layout object that terminates the current page content if any and starts the new page.</summary>
    /// <remarks>
    /// A layout object that terminates the current page content if any and starts the new page.
    /// <para />
    /// If no
    /// <see cref="iText.Kernel.Geom.PageSize"/>
    /// and
    /// <see cref="iText.Layout.Properties.Margins.PageMarginBoxes"/>
    /// are given,
    /// the new content section will have default page size and page margins.
    /// <para />
    /// Specified (or default if not specified)
    /// <see cref="iText.Kernel.Geom.PageSize"/>
    /// and
    /// <see cref="iText.Layout.Properties.Margins.PageMarginBoxes"/>
    /// will be applied for all next pages until it'll be overridden by other
    /// <see cref="SectionBreak"/>
    /// or
    /// <see cref="AreaBreak"/>
    /// elements.
    /// </remarks>
    public class SectionBreak : AbstractElement<iText.Layout.Element.SectionBreak> {
        private PageSize pageSize;

        private PageMarginBoxes pageMarginBoxes;

        private bool breakPage;

        /// <summary>
        /// Creates new
        /// <see cref="SectionBreak"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates new
        /// <see cref="SectionBreak"/>
        /// instance.
        /// <para />
        /// The new content section will have default page size and page margins.
        /// </remarks>
        public SectionBreak() {
        }

        // Default constructor.
        /// <summary>
        /// Creates new
        /// <see cref="SectionBreak"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates new
        /// <see cref="SectionBreak"/>
        /// instance.
        /// <para />
        /// The new content section will have the specified page size and default page margins.
        /// </remarks>
        /// <param name="pageSize">
        /// 
        /// <see cref="iText.Kernel.Geom.PageSize"/>
        /// page size of the new content section
        /// </param>
        public SectionBreak(PageSize pageSize) {
            this.pageSize = pageSize;
        }

        /// <summary>
        /// Creates new
        /// <see cref="SectionBreak"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates new
        /// <see cref="SectionBreak"/>
        /// instance.
        /// <para />
        /// The new content section will have the specified page margins and default page size.
        /// </remarks>
        /// <param name="pageMarginBoxes">
        /// 
        /// <see cref="iText.Layout.Properties.Margins.PageMarginBoxes"/>
        /// page margins of the new content section
        /// </param>
        public SectionBreak(PageMarginBoxes pageMarginBoxes) {
            this.pageMarginBoxes = pageMarginBoxes;
        }

        /// <summary>
        /// Creates new
        /// <see cref="SectionBreak"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates new
        /// <see cref="SectionBreak"/>
        /// instance.
        /// <para />
        /// The new content section will have the specified page size and page margins.
        /// </remarks>
        /// <param name="pageSize">
        /// 
        /// <see cref="iText.Kernel.Geom.PageSize"/>
        /// page size of the new content section
        /// </param>
        /// <param name="pageMarginBoxes">
        /// 
        /// <see cref="iText.Layout.Properties.Margins.PageMarginBoxes"/>
        /// page margins of the new content section
        /// </param>
        public SectionBreak(PageSize pageSize, PageMarginBoxes pageMarginBoxes) {
            this.pageSize = pageSize;
            this.pageMarginBoxes = pageMarginBoxes;
        }

        /// <summary>Gets the page size.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Geom.PageSize">page size</see>
        /// of the next content section
        /// </returns>
        public virtual PageSize GetPageSize() {
            return pageSize;
        }

        /// <summary>Sets the page size.</summary>
        /// <param name="pageSize">
        /// the new
        /// <see cref="iText.Kernel.Geom.PageSize">page size</see>
        /// of the next content section
        /// </param>
        /// <returns>this same instance</returns>
        public virtual iText.Layout.Element.SectionBreak SetPageSize(PageSize pageSize) {
            this.pageSize = pageSize;
            return this;
        }

        /// <summary>Gets the page margins.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.Layout.Properties.Margins.PageMarginBoxes">page margins</see>
        /// of the next content section
        /// </returns>
        public virtual PageMarginBoxes GetPageMargins() {
            return pageMarginBoxes;
        }

        /// <summary>Sets the page margins.</summary>
        /// <param name="pageMarginBoxes">
        /// the
        /// <see cref="iText.Layout.Properties.Margins.PageMarginBoxes">page margins</see>
        /// of the next content section
        /// </param>
        /// <returns>this same instance</returns>
        public virtual iText.Layout.Element.SectionBreak SetPageMargins(PageMarginBoxes pageMarginBoxes) {
            this.pageMarginBoxes = pageMarginBoxes;
            return this;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Checks whether this
        /// <see cref="SectionBreak"/>
        /// should add page break.
        /// </summary>
        /// <remarks>
        /// Checks whether this
        /// <see cref="SectionBreak"/>
        /// should add page break.
        /// <para />
        /// Page won't break in case SectionBreak is added to the empty page with the same page size
        /// or if page margins and page size were not changed. So
        /// <c>breakPage</c>
        /// field also checks
        /// whether SectionBreak changes page margins or page size and is not the 1st element on the page.
        /// </remarks>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if page break is expected,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        internal virtual bool BreakPage() {
            return breakPage;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Defines whether this
        /// <see cref="SectionBreak"/>
        /// should add page break.
        /// </summary>
        /// <remarks>
        /// Defines whether this
        /// <see cref="SectionBreak"/>
        /// should add page break.
        /// Controlled by
        /// <see cref="iText.Layout.Renderer.SectionBreakRenderer.Layout(iText.Layout.Layout.LayoutContext)"/>.
        /// <para />
        /// Page shouldn't break in case SectionBreak is added to the empty page with the same page size
        /// or if page margins and page size were not changed. So
        /// <paramref name="breakPage"/>
        /// field also checks
        /// whether SectionBreak changes page margins or page size and is not the 1st element on the page.
        /// </remarks>
        /// <param name="breakPage">
        /// 
        /// <see langword="true"/>
        /// if page break is expected,
        /// <see langword="false"/>
        /// otherwise
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="SectionBreak"/>
        /// instance
        /// </returns>
        internal virtual iText.Layout.Element.SectionBreak BreakPage(bool breakPage) {
            this.breakPage = breakPage;
            return this;
        }
//\endcond

        protected internal override IRenderer MakeNewRenderer() {
            return new SectionBreakRenderer(this);
        }
    }
}
