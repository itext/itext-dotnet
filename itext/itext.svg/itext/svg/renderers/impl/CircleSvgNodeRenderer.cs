using iText.StyledXmlParser.Css.Util;
using iText.Svg;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;circle&gt; tag.
    /// </summary>
    public class CircleSvgNodeRenderer : EllipseSvgNodeRenderer {
        protected internal override bool SetParameters() {
            cx = 0;
            cy = 0;
            if (GetAttribute(SvgAttributeConstants.CX_ATTRIBUTE) != null) {
                cx = CssUtils.ParseAbsoluteLength(GetAttribute(SvgAttributeConstants.CX_ATTRIBUTE));
            }
            if (GetAttribute(SvgAttributeConstants.CY_ATTRIBUTE) != null) {
                cy = CssUtils.ParseAbsoluteLength(GetAttribute(SvgAttributeConstants.CY_ATTRIBUTE));
            }
            if (GetAttribute(SvgAttributeConstants.R_ATTRIBUTE) != null && CssUtils.ParseAbsoluteLength(GetAttribute(SvgAttributeConstants
                .R_ATTRIBUTE)) > 0) {
                rx = CssUtils.ParseAbsoluteLength(GetAttribute(SvgAttributeConstants.R_ATTRIBUTE));
                ry = rx;
            }
            else {
                return false;
            }
            //No drawing if rx is absent
            return true;
        }
    }
}
