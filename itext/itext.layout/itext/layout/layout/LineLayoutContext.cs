/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Layout.Margincollapse;

namespace iText.Layout.Layout {
    /// <summary>
    /// Represents the context for content of a line
    /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layouting</see>.
    /// </summary>
    public class LineLayoutContext : LayoutContext {
        private bool floatOverflowedToNextPageWithNothing = false;

        private float textIndent;

        /// <summary>Creates the context for content of a line.</summary>
        /// <param name="area">for the content to be placed on</param>
        /// <param name="marginsCollapseInfo">the info about margins collapsing</param>
        /// <param name="floatedRendererAreas">
        /// list of
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// objects
        /// </param>
        /// <param name="clippedHeight">indicates whether the height is clipped or not</param>
        public LineLayoutContext(LayoutArea area, MarginsCollapseInfo marginsCollapseInfo, IList<Rectangle> floatedRendererAreas
            , bool clippedHeight)
            : base(area, marginsCollapseInfo, floatedRendererAreas, clippedHeight) {
        }

        /// <summary>Creates the context for content of a line.</summary>
        /// <param name="layoutContext">the context for content layouting</param>
        public LineLayoutContext(LayoutContext layoutContext)
            : base(layoutContext.area, layoutContext.marginsCollapseInfo, layoutContext.floatRendererAreas, layoutContext
                .clippedHeight) {
        }

        /// <summary>
        /// Specifies whether some floating element within the same paragraph has already completely overflowed to the next
        /// page.
        /// </summary>
        /// <returns>true if floating element has already overflowed to the next page, false otherwise.</returns>
        public virtual bool IsFloatOverflowedToNextPageWithNothing() {
            return floatOverflowedToNextPageWithNothing;
        }

        /// <summary>
        /// Changes the value of property specified by
        /// <see cref="IsFloatOverflowedToNextPageWithNothing()"/>.
        /// </summary>
        /// <param name="floatOverflowedToNextPageWithNothing">true if some floating element already completely overflowed.
        ///     </param>
        /// <returns>
        /// this
        /// <see cref="LineLayoutContext"/>
        /// instance.
        /// </returns>
        public virtual iText.Layout.Layout.LineLayoutContext SetFloatOverflowedToNextPageWithNothing(bool floatOverflowedToNextPageWithNothing
            ) {
            this.floatOverflowedToNextPageWithNothing = floatOverflowedToNextPageWithNothing;
            return this;
        }

        /// <summary>Gets the indent of text in the beginning of the current line.</summary>
        /// <returns>the indent of text in this line.</returns>
        public virtual float GetTextIndent() {
            return textIndent;
        }

        /// <summary>Sets the indent of text in the beginning of the current line.</summary>
        /// <param name="textIndent">the indent of text in this line.</param>
        /// <returns>
        /// this
        /// <see cref="LineLayoutContext"/>
        /// instance.
        /// </returns>
        public virtual iText.Layout.Layout.LineLayoutContext SetTextIndent(float textIndent) {
            this.textIndent = textIndent;
            return this;
        }
    }
}
