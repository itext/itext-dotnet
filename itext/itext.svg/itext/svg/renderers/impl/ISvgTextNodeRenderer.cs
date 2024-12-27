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
using System;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    public interface ISvgTextNodeRenderer : ISvgNodeRenderer {
        [Obsolete]
        float GetTextContentLength(float parentFontSize, PdfFont font);

        /// <summary>
        /// This method is deprecated and will be replaced with new signature
        /// <c>getRelativeTranslation(SvgDrawContext)</c>.
        /// </summary>
        /// <remarks>
        /// This method is deprecated and will be replaced with new signature
        /// <c>getRelativeTranslation(SvgDrawContext)</c>.
        /// This is needed because xMove/yMove can contain relative values, so SvgDrawContext is needed to resolve them.
        /// </remarks>
        /// <returns>text relative translation</returns>
        [Obsolete]
        float[] GetRelativeTranslation();

        /// <summary>
        /// This method is deprecated and will be replaced with new signature
        /// <c>containsRelativeMove(SvgDrawContext)</c>.
        /// </summary>
        /// <remarks>
        /// This method is deprecated and will be replaced with new signature
        /// <c>containsRelativeMove(SvgDrawContext)</c>.
        /// This is needed because xMove/yMove can contain relative values, so SvgDrawContext is needed to resolve them.
        /// </remarks>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if there is a relative move,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        [Obsolete]
        bool ContainsRelativeMove();

        bool ContainsAbsolutePositionChange();

        float[][] GetAbsolutePositionChanges();

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
