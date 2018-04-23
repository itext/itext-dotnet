/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2018 iText Group NV
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
using System;
using System.Collections.Generic;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>Draws text to a PdfCanvas.</summary>
    /// <remarks>
    /// Draws text to a PdfCanvas.
    /// Currently supported:
    /// - only the default font of PDF
    /// - x, y
    /// </remarks>
    public class TextSvgNodeRenderer : AbstractSvgNodeRenderer {
        protected internal override void DoDraw(SvgDrawContext context) {
            if (this.attributesAndStyles != null && this.attributesAndStyles.ContainsKey(SvgTagConstants.TEXT_CONTENT)
                ) {
                PdfCanvas currentCanvas = context.GetCurrentCanvas();
                Rectangle currentViewPort = context.GetCurrentViewPort();
                currentCanvas.ConcatMatrix(TransformUtils.ParseTransform("matrix(1 0 0 -1 0 " + SvgCssUtils.ConvertFloatToString
                    (currentViewPort.GetHeight()) + ")"));
                String xRawValue = this.attributesAndStyles.Get(SvgTagConstants.X);
                String yRawValue = this.attributesAndStyles.Get(SvgTagConstants.Y);
                String fontSizeRawValue = this.attributesAndStyles.Get(SvgTagConstants.FONT_SIZE);
                IList<String> xValuesList = SvgCssUtils.SplitValueList(xRawValue);
                IList<String> yValuesList = SvgCssUtils.SplitValueList(yRawValue);
                float x = 0f;
                float y = 0f;
                float fontSize = 0f;
                if (fontSizeRawValue != null && !String.IsNullOrEmpty(fontSizeRawValue)) {
                    fontSize = CssUtils.ParseAbsoluteLength(fontSizeRawValue, CssConstants.PT);
                }
                if (!xValuesList.IsEmpty()) {
                    x = float.Parse(xValuesList[0], System.Globalization.CultureInfo.InvariantCulture);
                }
                if (!yValuesList.IsEmpty()) {
                    y = float.Parse(yValuesList[0], System.Globalization.CultureInfo.InvariantCulture);
                }
                currentCanvas.BeginText();
                try {
                    // TODO font resolution RND-883
                    currentCanvas.SetFontAndSize(PdfFontFactory.CreateFont(), fontSize);
                }
                catch (System.IO.IOException e) {
                    throw new SvgProcessingException(SvgLogMessageConstant.FONT_NOT_FOUND, e);
                }
                currentCanvas.MoveText(x, y);
                currentCanvas.SetColor(ColorConstants.BLACK, true);
                currentCanvas.ShowText(this.attributesAndStyles.Get(SvgTagConstants.TEXT_CONTENT));
                currentCanvas.EndText();
            }
        }
    }
}
