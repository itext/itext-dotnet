using System;
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>Root renderer responsible for applying the initial axis-flipping transform</summary>
    public class PdfRootSvgNodeRenderer : ISvgNodeRenderer {
        internal ISvgNodeRenderer subTreeRoot;

        /// <summary>
        /// Creates a
        /// <see cref="PdfRootSvgNodeRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="subTreeRoot">root of the subtree</param>
        public PdfRootSvgNodeRenderer(ISvgNodeRenderer subTreeRoot) {
            this.subTreeRoot = subTreeRoot;
            subTreeRoot.SetParent(this);
        }

        public virtual void SetParent(ISvgNodeRenderer parent) {
        }

        // TODO RND-986
        public virtual ISvgNodeRenderer GetParent() {
            // TODO RND-986
            return null;
        }

        public virtual void Draw(SvgDrawContext context) {
            //Set viewport and transformation for pdf-context
            context.AddViewPort(this.CalculateViewPort(context));
            PdfCanvas currentCanvas = context.GetCurrentCanvas();
            currentCanvas.ConcatMatrix(this.CalculateTransformation(context));
            currentCanvas.WriteLiteral("% svg root\n");
            subTreeRoot.Draw(context);
        }

        public virtual void SetAttributesAndStyles(IDictionary<String, String> attributesAndStyles) {
        }

        public virtual String GetAttribute(String key) {
            return null;
        }

        public virtual void SetAttribute(String key, String value) {
        }

        internal virtual AffineTransform CalculateTransformation(SvgDrawContext context) {
            Rectangle viewPort = context.GetCurrentViewPort();
            float horizontal = viewPort.GetX();
            float vertical = viewPort.GetY() + viewPort.GetHeight();
            // flip coordinate space vertically and translate on the y axis with the viewport height
            AffineTransform transform = AffineTransform.GetTranslateInstance(0, 0);
            //Identity-transform
            transform.Concatenate(AffineTransform.GetTranslateInstance(horizontal, vertical));
            transform.Concatenate(new AffineTransform(1, 0, 0, -1, 0, 0));
            return transform;
        }

        internal virtual Rectangle CalculateViewPort(SvgDrawContext context) {
            float portX = 0f;
            float portY = 0f;
            float portWidth = 0f;
            float portHeight = 0f;
            PdfStream contentStream = context.GetCurrentCanvas().GetContentStream();
            if (!contentStream.ContainsKey(PdfName.BBox)) {
                throw new SvgProcessingException(SvgLogMessageConstant.ROOT_SVG_NO_BBOX);
            }
            PdfArray bboxArray = contentStream.GetAsArray(PdfName.BBox);
            portX = bboxArray.GetAsNumber(0).FloatValue();
            portY = bboxArray.GetAsNumber(1).FloatValue();
            portWidth = bboxArray.GetAsNumber(2).FloatValue() - portX;
            portHeight = bboxArray.GetAsNumber(3).FloatValue() - portY;
            return new Rectangle(portX, portY, portWidth, portHeight);
        }
    }
}
