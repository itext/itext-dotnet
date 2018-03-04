using System;
using iText.IO.Util;
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>
    /// This class helps to identify whether we are dealing with a renderer that creates
    /// a new "Block formatting context" in terms of CSS.
    /// </summary>
    /// <remarks>
    /// This class helps to identify whether we are dealing with a renderer that creates
    /// a new "Block formatting context" in terms of CSS. Such renderers adhere to
    /// specific rules of floating elements and margins collapse handling.
    /// <p>
    /// See
    /// <a href="https://www.w3.org/TR/CSS21/visuren.html#block-formatting">https://www.w3.org/TR/CSS21/visuren.html#block-formatting</a>
    /// and
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/Guide/CSS/Block_formatting_context">https://developer.mozilla.org/en-US/docs/Web/Guide/CSS/Block_formatting_context</a>
    /// for more info.
    /// </remarks>
    public class BlockFormattingContextUtil {
        public static bool IsRendererCreateBfc(IRenderer renderer) {
            return (renderer is RootRenderer) || (renderer is CellRenderer) || IsInlineBlock(renderer) || FloatingHelper
                .IsRendererFloating(renderer) || IsAbsolutePosition(renderer) || IsFixedPosition(renderer) || AbstractRenderer
                .IsOverflowProperty(OverflowPropertyValue.HIDDEN, renderer, Property.OVERFLOW_X) || AbstractRenderer.IsOverflowProperty
                (OverflowPropertyValue.HIDDEN, renderer, Property.OVERFLOW_Y);
        }

        private static bool IsInlineBlock(IRenderer renderer) {
            return renderer.GetParent() is LineRenderer && (renderer is BlockRenderer || renderer is TableRenderer);
        }

        private static bool IsAbsolutePosition(IRenderer renderer) {
            int? positioning = NumberUtil.AsInteger(renderer.GetProperty<Object>(Property.POSITION));
            return Convert.ToInt32(LayoutPosition.ABSOLUTE).Equals(positioning);
        }

        private static bool IsFixedPosition(IRenderer renderer) {
            int? positioning = NumberUtil.AsInteger(renderer.GetProperty<Object>(Property.POSITION));
            return Convert.ToInt32(LayoutPosition.FIXED).Equals(positioning);
        }
    }
}
