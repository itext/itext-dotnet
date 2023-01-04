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
