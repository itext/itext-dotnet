using iText.Kernel.Font;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    public interface ISvgTextNodeRenderer : ISvgNodeRenderer {
        float GetTextContentLength(float parentFontSize, PdfFont font);

        float[] GetRelativeTranslation();

        bool ContainsRelativeMove();

        bool ContainsAbsolutePositionChange();

        float[][] GetAbsolutePositionChanges();
    }
}
