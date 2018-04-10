using System;
using System.Collections.Generic;
using iText.Kernel.Colors;
using iText.Kernel.Font;
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
                    x = CssUtils.ParseAbsoluteLength(xValuesList[0]);
                }
                if (!yValuesList.IsEmpty()) {
                    y = CssUtils.ParseAbsoluteLength(yValuesList[0]);
                }
                currentCanvas.BeginText();
                try {
                    // TODO font resolution RND-883
                    currentCanvas.SetFontAndSize(PdfFontFactory.CreateFont(), fontSize);
                }
                catch (System.IO.IOException e) {
                    throw new SvgProcessingException(SvgLogMessageConstant.FONT_NOT_FOUND, e);
                }
                //Current transformation matrix results in the character glyphs being mirrored, correct with inverse tf
                currentCanvas.SaveState();
                currentCanvas.SetTextMatrix(1, 0, 0, -1, x, y);
                currentCanvas.SetColor(ColorConstants.BLACK, true);
                currentCanvas.ShowText(this.attributesAndStyles.Get(SvgTagConstants.TEXT_CONTENT));
                currentCanvas.RestoreState();
                currentCanvas.EndText();
            }
        }
    }
}
