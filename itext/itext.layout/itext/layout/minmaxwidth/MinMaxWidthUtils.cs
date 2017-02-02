using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Minmaxwidth {
    public class MinMaxWidthUtils {
        private const float eps = 0.01f;

        private const float max = 32760f;

        public static float GetEps() {
            return eps;
        }

        public static float GetMax() {
            return max;
        }

        public static MinMaxWidth CountDefaultMinMaxWidth(IRenderer renderer, float availableWidth) {
            LayoutResult result = renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(availableWidth, AbstractRenderer
                .INF))));
            return result.GetStatus() == LayoutResult.NOTHING ? new MinMaxWidth(0, availableWidth) : new MinMaxWidth(0
                , availableWidth, 0, result.GetOccupiedArea().GetBBox().GetWidth());
        }

        public static float GetBorderWidth(IPropertyContainer element) {
            Border border = element.GetProperty<Border>(Property.BORDER);
            Border rightBorder = element.GetProperty<Border>(Property.BORDER_RIGHT);
            Border leftBorder = element.GetProperty<Border>(Property.BORDER_LEFT);
            if (!element.HasOwnProperty(Property.BORDER_RIGHT)) {
                rightBorder = border;
            }
            if (!element.HasOwnProperty(Property.BORDER_LEFT)) {
                leftBorder = border;
            }
            float rightBorderWidth = rightBorder != null ? rightBorder.GetWidth() : 0;
            float leftBorderWidth = leftBorder != null ? leftBorder.GetWidth() : 0;
            return rightBorderWidth + leftBorderWidth;
        }

        public static float GetMarginsWidth(IPropertyContainer element) {
            float? rightMargin = element.GetProperty<float?>(Property.MARGIN_RIGHT);
            float? leftMargin = element.GetProperty<float?>(Property.MARGIN_LEFT);
            float rightMarginWidth = rightMargin != null ? (float)rightMargin : 0;
            float leftMarginWidth = leftMargin != null ? (float)leftMargin : 0;
            return rightMarginWidth + leftMarginWidth;
        }

        public static float GetPaddingWidth(IPropertyContainer element) {
            float? rightPadding = element.GetProperty<float?>(Property.PADDING_RIGHT);
            float? leftPadding = element.GetProperty<float?>(Property.PADDING_LEFT);
            float rightPaddingWidth = rightPadding != null ? (float)rightPadding : 0;
            float leftPaddingWidth = leftPadding != null ? (float)leftPadding : 0;
            return rightPaddingWidth + leftPaddingWidth;
        }
    }
}
