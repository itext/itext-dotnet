using System;
using iText.Kernel.Geom;
using iText.Layout.Borders;
using iText.Layout.Element;
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

        public static float ToEffectiveWidth(BlockElement b, float fullWidth) {
            if (b is Table) {
                return fullWidth + ((Table)b).GetNumberOfColumns() * eps;
            }
            else {
                return fullWidth - GetBorderWidth(b) - GetMarginsWidth(b) - GetPaddingWidth(b) + eps;
            }
        }

        public static float[] ToEffectiveTableColumnWidth(float[] tableColumnWidth) {
            float[] result = tableColumnWidth.Clone();
            for (int i = 0; i < result.Length; ++i) {
                result[i] += eps;
            }
            return result;
        }

        //heuristic method
        public static MinMaxWidth CountRotationMinMaxWidth(MinMaxWidth minMaxWidth, BlockRenderer renderer) {
            float? rotation = renderer.GetPropertyAsFloat(Property.ROTATION_ANGLE);
            if (rotation != null && renderer.GetModelElement() is BlockElement) {
                float angle = rotation;
                bool restoreRendererProperty = renderer.HasOwnProperty(Property.ROTATION_ANGLE);
                renderer.SetProperty(Property.ROTATION_ANGLE, System.Convert.ToSingle(0));
                float width = ToEffectiveWidth((BlockElement)renderer.GetModelElement(), minMaxWidth.GetAvailableWidth());
                LayoutResult result = renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(width, AbstractRenderer
                    .INF))));
                if (restoreRendererProperty) {
                    renderer.SetProperty(Property.ROTATION_ANGLE, rotation);
                }
                else {
                    renderer.DeleteOwnProperty(Property.ROTATION_ANGLE);
                }
                if (result.GetOccupiedArea() != null) {
                    double a = result.GetOccupiedArea().GetBBox().GetWidth();
                    double b = result.GetOccupiedArea().GetBBox().GetHeight();
                    return new MinMaxWidth(0, minMaxWidth.GetAvailableWidth(), (float)Math.Sqrt(2 * a * b), (float)Math.Sqrt(a
                         * a + b * b));
                }
            }
            return minMaxWidth;
        }

        public static MinMaxWidth CountDefaultMinMaxWidth(IRenderer renderer, float availableWidth) {
            LayoutResult result = renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(availableWidth, AbstractRenderer
                .INF))));
            return result.GetStatus() == LayoutResult.NOTHING ? new MinMaxWidth(0, availableWidth) : new MinMaxWidth(0
                , availableWidth, 0, result.GetOccupiedArea().GetBBox().GetWidth());
        }

        private static float GetBorderWidth(IElement element) {
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

        private static float GetMarginsWidth(IElement element) {
            float? rightMargin = element.GetProperty<float?>(Property.MARGIN_RIGHT);
            float? leftMargin = element.GetProperty<float?>(Property.MARGIN_LEFT);
            float rightMarginWidth = rightMargin != null ? (float)rightMargin : 0;
            float leftMarginWidth = leftMargin != null ? (float)leftMargin : 0;
            return rightMarginWidth + leftMarginWidth;
        }

        private static float GetPaddingWidth(IElement element) {
            float? rightPadding = element.GetProperty<float?>(Property.PADDING_RIGHT);
            float? leftPadding = element.GetProperty<float?>(Property.PADDING_LEFT);
            float rightPaddingWidth = rightPadding != null ? (float)rightPadding : 0;
            float leftPaddingWidth = leftPadding != null ? (float)leftPadding : 0;
            return rightPaddingWidth + leftPaddingWidth;
        }
    }
}
