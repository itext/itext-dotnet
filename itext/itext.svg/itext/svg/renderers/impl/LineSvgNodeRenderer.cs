using System;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;line&gt; tag.
    /// </summary>
    public class LineSvgNodeRenderer : AbstractSvgNodeRenderer {
        public LineSvgNodeRenderer() {
        }

        protected internal override void DoDraw(SvgDrawContext context) {
            PdfCanvas canvas = context.GetCurrentCanvas();
            try {
                if (attributesAndStyles.Count > 0) {
                    float x1 = 0f;
                    float y1 = 0f;
                    float x2 = 0f;
                    float y2 = 0f;
                    if (attributesAndStyles.ContainsKey(SvgTagConstants.X1)) {
                        x1 = GetAttribute(attributesAndStyles, SvgTagConstants.X1);
                    }
                    if (attributesAndStyles.ContainsKey(SvgTagConstants.Y1)) {
                        y1 = GetAttribute(attributesAndStyles, SvgTagConstants.Y1);
                    }
                    if (attributesAndStyles.ContainsKey(SvgTagConstants.X2)) {
                        x2 = GetAttribute(attributesAndStyles, SvgTagConstants.X2);
                    }
                    if (attributesAndStyles.ContainsKey(SvgTagConstants.Y2)) {
                        y2 = GetAttribute(attributesAndStyles, SvgTagConstants.Y2);
                    }
                    canvas.MoveTo(x1, y1).LineTo(x2, y2);
                }
            }
            catch (FormatException e) {
                throw new SvgProcessingException(SvgLogMessageConstant.FLOAT_PARSING_NAN, e);
            }
        }

        protected internal override bool CanElementFill() {
            return false;
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
