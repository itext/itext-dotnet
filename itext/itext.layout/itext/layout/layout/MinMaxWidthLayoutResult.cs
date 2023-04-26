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
using iText.Layout.Minmaxwidth;
using iText.Layout.Renderer;

namespace iText.Layout.Layout {
    /// <summary>
    /// Represents the result of content
    /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layouting</see>.
    /// </summary>
    public class MinMaxWidthLayoutResult : LayoutResult {
        /// <summary>
        /// The
        /// <see cref="iText.Layout.Minmaxwidth.MinMaxWidth"/>
        /// value of min and max width.
        /// </summary>
        protected internal MinMaxWidth minMaxWidth;

        /// <summary>Creates min and max width.</summary>
        /// <param name="status">the status which indicates the content</param>
        /// <param name="occupiedArea">the area occupied by the content</param>
        /// <param name="splitRenderer">the renderer to draw the splitted part of the content</param>
        /// <param name="overflowRenderer">the renderer to draw the overflowed part of the content</param>
        public MinMaxWidthLayoutResult(int status, LayoutArea occupiedArea, IRenderer splitRenderer, IRenderer overflowRenderer
            )
            : base(status, occupiedArea, splitRenderer, overflowRenderer) {
            minMaxWidth = new MinMaxWidth();
        }

        /// <summary>Creates min and max width.</summary>
        /// <param name="status">the status which indicates the content</param>
        /// <param name="occupiedArea">the area occupied by the content</param>
        /// <param name="splitRenderer">the renderer to draw the splitted part of the content</param>
        /// <param name="overflowRenderer">the renderer to draw the overflowed part of the content</param>
        /// <param name="cause">
        /// the first renderer to produce
        /// <see cref="LayoutResult.NOTHING"/>
        /// </param>
        public MinMaxWidthLayoutResult(int status, LayoutArea occupiedArea, IRenderer splitRenderer, IRenderer overflowRenderer
            , IRenderer cause)
            : base(status, occupiedArea, splitRenderer, overflowRenderer, cause) {
            minMaxWidth = new MinMaxWidth();
        }

        /// <summary>Gets min and max width.</summary>
        /// <returns>min and max width</returns>
        public virtual MinMaxWidth GetMinMaxWidth() {
            return minMaxWidth;
        }

        /// <summary>Sets min and max width.</summary>
        /// <param name="minMaxWidth">min and max width</param>
        /// <returns>min and max width</returns>
        public virtual iText.Layout.Layout.MinMaxWidthLayoutResult SetMinMaxWidth(MinMaxWidth minMaxWidth) {
            this.minMaxWidth = minMaxWidth;
            return this;
        }
    }
}
