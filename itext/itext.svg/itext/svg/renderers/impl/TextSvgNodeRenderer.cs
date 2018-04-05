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
