/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Layout.Renderer;

namespace iText.Layout.Layout {
    /// <summary>
    /// Represents the result of a line
    /// <see cref="iText.Layout.Renderer.LineRenderer.Layout(LayoutContext)">layouting</see>.
    /// </summary>
    public class LineLayoutResult : MinMaxWidthLayoutResult {
        /// <summary>Indicates whether split was forced by new line symbol or not.</summary>
        protected internal bool splitForcedByNewline;

        private IList<IRenderer> floatsOverflowedToNextPage;

        /// <summary>
        /// Creates the
        /// <see cref="LayoutResult"/>
        /// result of
        /// <see cref="iText.Layout.Renderer.LineRenderer.Layout(LayoutContext)">layouting</see>
        /// }.
        /// </summary>
        /// <remarks>
        /// Creates the
        /// <see cref="LayoutResult"/>
        /// result of
        /// <see cref="iText.Layout.Renderer.LineRenderer.Layout(LayoutContext)">layouting</see>
        /// }.
        /// The
        /// <see cref="LayoutResult.causeOfNothing"/>
        /// will be set as null.
        /// </remarks>
        /// <param name="status">
        /// the status of
        /// <see cref="iText.Layout.Renderer.LineRenderer.Layout(LayoutContext)"/>
        /// </param>
        /// <param name="occupiedArea">the area occupied by the content</param>
        /// <param name="splitRenderer">the renderer to draw the splitted part of the content</param>
        /// <param name="overflowRenderer">the renderer to draw the overflowed part of the content</param>
        public LineLayoutResult(int status, LayoutArea occupiedArea, IRenderer splitRenderer, IRenderer overflowRenderer
            )
            : base(status, occupiedArea, splitRenderer, overflowRenderer) {
        }

        /// <summary>
        /// Creates the
        /// <see cref="LayoutResult"/>
        /// result of
        /// <see cref="iText.Layout.Renderer.LineRenderer.Layout(LayoutContext)">layouting</see>
        /// }.
        /// </summary>
        /// <param name="status">
        /// the status of
        /// <see cref="iText.Layout.Renderer.LineRenderer.Layout(LayoutContext)"/>
        /// </param>
        /// <param name="occupiedArea">the area occupied by the content</param>
        /// <param name="splitRenderer">the renderer to draw the splitted part of the content</param>
        /// <param name="overflowRenderer">the renderer to draw the overflowed part of the content</param>
        /// <param name="cause">
        /// the first renderer to produce
        /// <see cref="LayoutResult.NOTHING"/>
        /// </param>
        public LineLayoutResult(int status, LayoutArea occupiedArea, IRenderer splitRenderer, IRenderer overflowRenderer
            , IRenderer cause)
            : base(status, occupiedArea, splitRenderer, overflowRenderer, cause) {
        }

        /// <summary>Indicates whether split was forced by new line symbol in rendered text.</summary>
        /// <remarks>
        /// Indicates whether split was forced by new line symbol in rendered text.
        /// The value will be set as true if, for example,
        /// the rendered text of one of the child renderers contains '\n' symbol.
        /// </remarks>
        /// <returns>whether split was forced by new line or not</returns>
        public virtual bool IsSplitForcedByNewline() {
            return splitForcedByNewline;
        }

        /// <summary>Sets a flat that split was forced by new line symbol in rendered text.</summary>
        /// <param name="isSplitForcedByNewline">indicates that split was forced by new line symbol in rendered text.</param>
        /// <returns>
        /// 
        /// <see cref="LineLayoutResult">this layout result</see>
        /// the setting was applied on.
        /// </returns>
        public virtual iText.Layout.Layout.LineLayoutResult SetSplitForcedByNewline(bool isSplitForcedByNewline) {
            this.splitForcedByNewline = isSplitForcedByNewline;
            return this;
        }

        /// <summary>Gets the list of floats overflowed to next page.</summary>
        /// <returns>list of floats overflowed to next page</returns>
        public virtual IList<IRenderer> GetFloatsOverflowedToNextPage() {
            return floatsOverflowedToNextPage;
        }

        /// <summary>Sets the list of floats overflowed to next page.</summary>
        /// <param name="floatsOverflowedToNextPage">the floats overflowed to next page</param>
        public virtual void SetFloatsOverflowedToNextPage(IList<IRenderer> floatsOverflowedToNextPage) {
            this.floatsOverflowedToNextPage = floatsOverflowedToNextPage;
        }
    }
}
