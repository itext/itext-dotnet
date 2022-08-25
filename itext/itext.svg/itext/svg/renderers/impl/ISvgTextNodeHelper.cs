/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
using System;
using iText.Kernel.Geom;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>An interface containing a method to simplify working with SVG text elements.</summary>
    /// <remarks>
    /// An interface containing a method to simplify working with SVG text elements.
    /// Must be removed in update 7.3 as the methods of this interface will be moved to
    /// <see cref="ISvgTextNodeRenderer"/>
    /// </remarks>
    [Obsolete]
    public interface ISvgTextNodeHelper {
        /// <summary>Return the bounding rectangle of the text element.</summary>
        /// <param name="context">
        /// current
        /// <see cref="iText.Svg.Renderers.SvgDrawContext"/>
        /// </param>
        /// <param name="basePoint">end point of previous text element</param>
        /// <returns>
        /// created instance of
        /// <see cref="iText.Svg.Utils.TextRectangle"/>
        /// </returns>
        TextRectangle GetTextRectangle(SvgDrawContext context, Point basePoint);
    }
}
