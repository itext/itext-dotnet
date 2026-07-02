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
namespace iText.Layout.Element {
    /// <summary>
    /// This file is a helper class for
    /// <see cref="SectionBreak"/>
    /// for internal usage only.
    /// </summary>
    /// <remarks>
    /// This file is a helper class for
    /// <see cref="SectionBreak"/>
    /// for internal usage only.
    /// Be aware that its API and functionality may be changed in the future.
    /// </remarks>
    public sealed class SectionBreakUtil {
        /// <summary>
        /// Checks whether provided
        /// <see cref="SectionBreak"/>
        /// should add page break.
        /// </summary>
        /// <remarks>
        /// Checks whether provided
        /// <see cref="SectionBreak"/>
        /// should add page break.
        /// <para />
        /// Page won't break in case SectionBreak is added to the empty page with the same page size
        /// or if page margins and page size were not changed. So
        /// <c>breakPage</c>
        /// field also checks
        /// whether SectionBreak changes page margins or page size and is not the 1st element on the page.
        /// </remarks>
        /// <param name="sectionBreak">
        /// 
        /// <see cref="SectionBreak"/>
        /// to check
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if page break is expected,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public static bool BreakPage(SectionBreak sectionBreak) {
            return sectionBreak.BreakPage();
        }

        /// <summary>
        /// Defines whether provided
        /// <see cref="SectionBreak"/>
        /// should add page break.
        /// </summary>
        /// <remarks>
        /// Defines whether provided
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
        /// <param name="sectionBreak">
        /// 
        /// <see cref="SectionBreak"/>
        /// to check
        /// </param>
        /// <param name="breakPage">
        /// 
        /// <see langword="true"/>
        /// if page break is expected,
        /// <see langword="false"/>
        /// otherwise
        /// </param>
        public static void BreakPage(SectionBreak sectionBreak, bool breakPage) {
            sectionBreak.BreakPage(breakPage);
        }

        private SectionBreakUtil() {
        }
        // Private constructor will prevent the instantiation of this class directly.
    }
}
