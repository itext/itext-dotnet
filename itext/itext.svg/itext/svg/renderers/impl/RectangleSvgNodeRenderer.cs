using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;rect&gt; tag.
    /// </summary>
    public class RectangleSvgNodeRenderer : AbstractSvgNodeRenderer {
        protected internal override void DoDraw(SvgDrawContext context) {
            PdfCanvas canvas = context.GetCurrentCanvas();
            float x = CssUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgTagConstants.X));
            float y = CssUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgTagConstants.Y));
            float width = CssUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgTagConstants.WIDTH));
            float height = CssUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgTagConstants.HEIGHT));
            Rectangle rect = new Rectangle(x, y, width, height);
            canvas.Rectangle(rect);
        }
    }
}
