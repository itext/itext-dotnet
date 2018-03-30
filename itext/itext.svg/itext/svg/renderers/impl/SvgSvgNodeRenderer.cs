using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;svg&gt; tag.
    /// </summary>
    public class SvgSvgNodeRenderer : AbstractBranchSvgNodeRenderer {
        private bool outermost = true;

        protected internal override void DoDraw(SvgDrawContext context) {
            // we need to know if we are processing the outermost svg element
            // if this renderer's parent is null than we know we're in the outermost svg tag
            // this is important to set portX, portY, portWidth, and portHeight values
            this.outermost = this.GetParent() == null;
            // TODO RND-877
            context.AddViewPort(this.CalculateViewPort(context));
            PdfCanvas currentCanvas = context.GetCurrentCanvas();
            currentCanvas.ConcatMatrix(this.CalculateTransformation(context));
            base.DoDraw(context);
        }

        /// <summary>Calculate the transformation based on the context.</summary>
        /// <remarks>
        /// Calculate the transformation based on the context. If this renderer is the outermost renderer,
        /// we need to flip on the vertical axis and translate the height of the viewport
        /// </remarks>
        /// <param name="context">the SVG draw context</param>
        /// <returns>the transformation that needs to be applied to this renderer</returns>
        internal virtual AffineTransform CalculateTransformation(SvgDrawContext context) {
            Rectangle viewPort = context.GetCurrentViewPort();
            AffineTransform transform;
            if (outermost) {
                // flip coordinate space vertically and translate on the y axis with the viewport height
                transform = TransformUtils.ParseTransform("matrix(1 0 0 -1 " + viewPort.GetX() + " " + (viewPort.GetY() + 
                    viewPort.GetHeight()));
            }
            else {
                transform = AffineTransform.GetTranslateInstance(viewPort.GetX(), viewPort.GetY());
            }
            return transform;
        }

        /// <summary>Calculate the viewport based on the context.</summary>
        /// <param name="context">the SVG draw context</param>
        /// <returns>the viewport that applies to this renderer</returns>
        internal virtual Rectangle CalculateViewPort(SvgDrawContext context) {
            Rectangle currentViewPort = outermost ? null : context.GetCurrentViewPort();
            float portX = 0f;
            float portY = 0f;
            float portWidth = 0f;
            float portHeight = 0f;
            if (outermost) {
                PdfStream contentStream = context.GetCurrentCanvas().GetContentStream();
                if (!contentStream.ContainsKey(PdfName.BBox)) {
                    throw new SvgProcessingException(SvgLogMessageConstant.ROOT_SVG_NO_BBOX);
                }
                PdfArray bboxArray = contentStream.GetAsArray(PdfName.BBox);
                portX = bboxArray.GetAsNumber(0).FloatValue();
                portY = bboxArray.GetAsNumber(1).FloatValue();
                portWidth = bboxArray.GetAsNumber(2).FloatValue() - portX;
                portHeight = bboxArray.GetAsNumber(3).FloatValue() - portY;
            }
            else {
                // set default values to parent viewport in the case of a nested svg tag
                portX = currentViewPort.GetX();
                portY = currentViewPort.GetY();
                portWidth = currentViewPort.GetWidth();
                // default should be parent portWidth if not outermost
                portHeight = currentViewPort.GetHeight();
            }
            // default should be parent heigth if not outermost
            if (attributesAndStyles != null) {
                if (!outermost) {
                    if (attributesAndStyles.ContainsKey(SvgTagConstants.X)) {
                        portX = CssUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgTagConstants.X));
                    }
                    if (attributesAndStyles.ContainsKey(SvgTagConstants.Y)) {
                        portY = CssUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgTagConstants.Y));
                    }
                    if (attributesAndStyles.ContainsKey(SvgTagConstants.WIDTH)) {
                        portWidth = CssUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgTagConstants.WIDTH));
                    }
                    if (attributesAndStyles.ContainsKey(SvgTagConstants.HEIGHT)) {
                        portHeight = CssUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgTagConstants.HEIGHT));
                    }
                }
            }
            return new Rectangle(portX, portY, portWidth, portHeight);
        }
    }
}
