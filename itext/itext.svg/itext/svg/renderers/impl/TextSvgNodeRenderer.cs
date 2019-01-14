/*
This file is part of the iText (R) project.
Copyright (c) 1998-2019 iText Group NV
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
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Font;
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
            if (this.attributesAndStyles != null && this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.TEXT_CONTENT
                )) {
                PdfCanvas currentCanvas = context.GetCurrentCanvas();
                String xRawValue = this.attributesAndStyles.Get(SvgConstants.Attributes.X);
                String yRawValue = this.attributesAndStyles.Get(SvgConstants.Attributes.Y);
                String fontSizeRawValue = this.attributesAndStyles.Get(SvgConstants.Attributes.FONT_SIZE);
                IList<String> xValuesList = SvgCssUtils.SplitValueList(xRawValue);
                IList<String> yValuesList = SvgCssUtils.SplitValueList(yRawValue);
                float x = 0f;
                float y = 0f;
                float fontSize = 0f;
                if (fontSizeRawValue != null && !String.IsNullOrEmpty(fontSizeRawValue)) {
                    fontSize = CssUtils.ParseAbsoluteLength(fontSizeRawValue, CommonCssConstants.PT);
                }
                if (!xValuesList.IsEmpty()) {
                    x = CssUtils.ParseAbsoluteLength(xValuesList[0]);
                }
                if (!yValuesList.IsEmpty()) {
                    y = CssUtils.ParseAbsoluteLength(yValuesList[0]);
                }
                currentCanvas.BeginText();
                FontProvider provider = context.GetFontProvider();
                FontSet tempFonts = context.GetTempFonts();
                PdfFont font = null;
                if (!provider.GetFontSet().IsEmpty() || (tempFonts != null && !tempFonts.IsEmpty())) {
                    String fontFamily = this.attributesAndStyles.Get(SvgConstants.Attributes.FONT_FAMILY);
                    String fontWeight = this.attributesAndStyles.Get(SvgConstants.Attributes.FONT_WEIGHT);
                    String fontStyle = this.attributesAndStyles.Get(SvgConstants.Attributes.FONT_STYLE);
                    fontFamily = fontFamily != null ? fontFamily.Trim() : "";
                    FontInfo fontInfo = ResolveFontName(fontFamily, fontWeight, fontStyle, provider, tempFonts);
                    font = provider.GetPdfFont(fontInfo, tempFonts);
                }
                if (font == null) {
                    try {
                        // TODO (DEVSIX-2057)
                        // TODO each call of createFont() create a new instance of PdfFont.
                        // TODO FontProvider shall be used instead.
                        font = PdfFontFactory.CreateFont();
                    }
                    catch (System.IO.IOException e) {
                        throw new SvgProcessingException(SvgLogMessageConstant.FONT_NOT_FOUND, e);
                    }
                }
                currentCanvas.SetFontAndSize(font, fontSize);
                //Current transformation matrix results in the character glyphs being mirrored, correct with inverse tf
                currentCanvas.SetTextMatrix(1, 0, 0, -1, x, y);
                currentCanvas.SetColor(ColorConstants.BLACK, true);
                currentCanvas.ShowText(this.attributesAndStyles.Get(SvgConstants.Attributes.TEXT_CONTENT));
                currentCanvas.EndText();
            }
        }

        private FontInfo ResolveFontName(String fontFamily, String fontWeight, String fontStyle, FontProvider provider
            , FontSet tempFonts) {
            bool isBold = fontWeight != null && fontWeight.EqualsIgnoreCase(SvgConstants.Attributes.BOLD);
            bool isItalic = fontStyle != null && fontStyle.EqualsIgnoreCase(SvgConstants.Attributes.ITALIC);
            FontCharacteristics fontCharacteristics = new FontCharacteristics();
            IList<String> stringArrayList = new List<String>();
            stringArrayList.Add(fontFamily);
            fontCharacteristics.SetBoldFlag(isBold);
            fontCharacteristics.SetItalicFlag(isItalic);
            return provider.GetFontSelector(stringArrayList, fontCharacteristics, tempFonts).BestMatch();
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            TextSvgNodeRenderer copy = new TextSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            return copy;
        }
    }
}
