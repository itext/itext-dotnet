using System;
using iText.Kernel.Colors;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the gradient &lt;stop&gt; tag.
    /// </summary>
    public class StopSvgNodeRenderer : NoDrawOperationSvgNodeRenderer {
        /// <summary>Evaluates the stop color offset value</summary>
        /// <returns>the stop color offset value in [0, 1] range</returns>
        public virtual double GetOffset() {
            double? offset = null;
            String offsetAttribute = GetAttribute(SvgConstants.Attributes.OFFSET);
            if (CssUtils.IsPercentageValue(offsetAttribute)) {
                offset = (double)CssUtils.ParseRelativeValue(offsetAttribute, 1);
            }
            else {
                if (CssUtils.IsNumericValue(offsetAttribute)) {
                    offset = CssUtils.ParseDouble(offsetAttribute);
                }
            }
            double result = offset != null ? offset.Value : 0d;
            return result > 1d ? 1d : result > 0d ? result : 0d;
        }

        /// <summary>Evaluates the rgba array of the specified stop color</summary>
        /// <returns>
        /// the array of 4 floats which contains the rgba value corresponding
        /// to the specified stop color
        /// </returns>
        public virtual float[] GetStopColor() {
            float[] color = null;
            String colorValue = GetAttribute(SvgConstants.Tags.STOP_COLOR);
            if (colorValue != null) {
                color = WebColors.GetRGBAColor(colorValue);
            }
            if (color == null) {
                color = WebColors.GetRGBAColor("black");
            }
            return color;
        }

        /// <summary>Evaluates the stop opacity of the specified stop color</summary>
        /// <returns>the stop opacity value specified in the stop color</returns>
        public virtual float GetStopOpacity() {
            float? result = null;
            String opacityValue = GetAttribute(SvgConstants.Tags.STOP_OPACITY);
            if (opacityValue != null && !SvgConstants.Values.NONE.EqualsIgnoreCase(opacityValue)) {
                result = CssUtils.ParseFloat(opacityValue);
            }
            return result != null ? result.Value : 1f;
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            StopSvgNodeRenderer copy = new StopSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            return copy;
        }

        protected internal override void DoDraw(SvgDrawContext context) {
            throw new NotSupportedException(SvgLogMessageConstant.DRAW_NO_DRAW);
        }
    }
}
