/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
