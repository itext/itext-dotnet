using System;
using iText.Forms.Fields;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Renderer;

namespace iText.Forms.Util {
    /// <summary>Utility class for font size calculations.</summary>
    public sealed class FontSizeUtil {
        private FontSizeUtil() {
        }

        //utility class
        /// <summary>Calculates the font size that will fit the text in the given rectangle.</summary>
        /// <param name="paragraph">the paragraph to be fitted</param>
        /// <param name="rect">the rectangle to fit the text in</param>
        /// <param name="parentRenderer">the parent renderer</param>
        /// <returns>the font size that will fit the text in the given rectangle</returns>
        public static float ApproximateFontSizeToFitMultiLine(Paragraph paragraph, Rectangle rect, IRenderer parentRenderer
            ) {
            IRenderer renderer = paragraph.CreateRendererSubTree().SetParent(parentRenderer);
            LayoutContext layoutContext = new LayoutContext(new LayoutArea(1, rect));
            float lFontSize = AbstractPdfFormField.MIN_FONT_SIZE;
            float rFontSize = AbstractPdfFormField.DEFAULT_FONT_SIZE;
            paragraph.SetFontSize(AbstractPdfFormField.DEFAULT_FONT_SIZE);
            if (renderer.Layout(layoutContext).GetStatus() != LayoutResult.FULL) {
                int numberOfIterations = 6;
                for (int i = 0; i < numberOfIterations; i++) {
                    float mFontSize = (lFontSize + rFontSize) / 2;
                    paragraph.SetFontSize(mFontSize);
                    LayoutResult result = renderer.Layout(layoutContext);
                    if (result.GetStatus() == LayoutResult.FULL) {
                        lFontSize = mFontSize;
                    }
                    else {
                        rFontSize = mFontSize;
                    }
                }
            }
            else {
                lFontSize = AbstractPdfFormField.DEFAULT_FONT_SIZE;
            }
            return lFontSize;
        }

        /// <summary>Calculates the font size that will fit the text in the given rectangle.</summary>
        /// <param name="localFont">the font to be used</param>
        /// <param name="bBox">the bounding box of the field</param>
        /// <param name="value">the value of the field</param>
        /// <param name="minValue">the minimum font size</param>
        /// <param name="borderWidth">the border width of the field</param>
        /// <returns>the font size that will fit the text in the given rectangle</returns>
        public static float ApproximateFontSizeToFitSingleLine(PdfFont localFont, Rectangle bBox, String value, float
             minValue, float borderWidth) {
            // For text field that value shall be min 4, for checkbox there is no min value.
            float fs;
            float height = bBox.GetHeight() - borderWidth * 2;
            int[] fontBbox = localFont.GetFontProgram().GetFontMetrics().GetBbox();
            fs = FontProgram.ConvertGlyphSpaceToTextSpace(height / (fontBbox[2] - fontBbox[1]));
            float baseWidth = localFont.GetWidth(value, 1);
            if (baseWidth != 0) {
                float availableWidth = Math.Max(bBox.GetWidth() - borderWidth * 2, 0);
                // This constant is taken based on what was the resultant padding in previous version
                // of this algorithm in case border width was zero.
                float absMaxPadding = 4F;
                // relative value is quite big in order to preserve visible padding on small field sizes.
                // This constant is taken arbitrary, based on visual similarity to Acrobat behaviour.
                float relativePaddingForSmallSizes = 0.15F;
                // with current constants, if availableWidth is less than ~26 points, padding will be made relative
                if (availableWidth * relativePaddingForSmallSizes < absMaxPadding) {
                    availableWidth -= availableWidth * relativePaddingForSmallSizes * 2;
                }
                else {
                    availableWidth -= absMaxPadding * 2;
                }
                fs = Math.Min(fs, availableWidth / baseWidth);
            }
            return Math.Max(fs, minValue);
        }
    }
}
