using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>Responsible for drawing Images to the canvas.</summary>
    /// <remarks>
    /// Responsible for drawing Images to the canvas.
    /// Referenced SVG images aren't supported yet. TODO RND-984
    /// </remarks>
    public class ImageSvgNodeRenderer : AbstractSvgNodeRenderer {
        protected internal override void DoDraw(SvgDrawContext context) {
            ResourceResolver resourceResolver = context.GetResourceResolver();
            if (resourceResolver != null && this.attributesAndStyles != null) {
                PdfImageXObject xObject = resourceResolver.RetrieveImage(this.attributesAndStyles.Get(SvgConstants.Attributes
                    .XLINK_HREF));
                if (xObject != null) {
                    PdfCanvas currentCanvas = context.GetCurrentCanvas();
                    float x = 0;
                    float y = 0;
                    if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.X)) {
                        x = CssUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgConstants.Attributes.X));
                    }
                    if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.Y)) {
                        y = CssUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgConstants.Attributes.Y));
                    }
                    float width = 0;
                    if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.WIDTH)) {
                        width = CssUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgConstants.Attributes.WIDTH));
                    }
                    float height = 0;
                    if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.HEIGHT)) {
                        height = CssUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgConstants.Attributes.HEIGHT));
                    }
                    if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.PRESERVE_ASPECT_RATIO)) {
                    }
                    // TODO RND-876
                    float v = y + height;
                    currentCanvas.AddXObject(xObject, width, 0, 0, -height, x, v);
                }
            }
        }
    }
}
