using System;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas;
using iText.Svg;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;line&gt; tag.
    /// </summary>
    public class LineSvgNodeRenderer : AbstractSvgNodeRenderer {
        private float x1;

        private float x2;

        private float y1;

        private float y2;

        public LineSvgNodeRenderer() {
        }

        public LineSvgNodeRenderer(float x1, float y1, float x2, float y2) {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }

        protected internal override void DoDraw(SvgDrawContext context) {
            PdfCanvas canvas = context.GetCurrentCanvas();
            canvas.MoveTo(GetAttribute(attributesAndStyles, SvgTagConstants.X1), GetAttribute(attributesAndStyles, SvgTagConstants
                .Y1)).LineTo(GetAttribute(attributesAndStyles, SvgTagConstants.X2), GetAttribute(attributesAndStyles, 
                SvgTagConstants.Y2));
        }

        public virtual float GetX1() {
            return x1;
        }

        public virtual float GetX2() {
            return x2;
        }

        public virtual float GetY1() {
            return y1;
        }

        public virtual float GetY2() {
            return y2;
        }

        private float GetAttribute(IDictionary<String, String> attributes, String key) {
            String value = attributes.Get(key);
            if (value != null && !String.IsNullOrEmpty(value)) {
                return float.Parse(attributes.Get(key), System.Globalization.CultureInfo.InvariantCulture);
            }
            return 0;
        }
    }
}
