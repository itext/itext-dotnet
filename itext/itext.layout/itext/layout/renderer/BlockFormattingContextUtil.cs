/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
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
    /// <para />
    /// </remarks>
    /// <seealso><a href="https://www.w3.org/tr/css21/visuren.html#block-formatting">https://www.w3.org/TR/CSS21/visuren.html#block-formatting</a>
    ///     </seealso>
    /// <seealso><a href="https://developer.mozilla.org/en-us/docs/web/guide/css/block_formatting_context">https://developer.mozilla.org/en-US/docs/Web/Guide/CSS/Block_formatting_context</a>
    ///     </seealso>
    public class BlockFormattingContextUtil {
        /// <summary>Defines whether a renderer creates a new "Block formatting context" in terms of CSS.</summary>
        /// <remarks>
        /// Defines whether a renderer creates a new "Block formatting context" in terms of CSS.
        /// <para />
        /// See
        /// <see cref="BlockFormattingContextUtil"/>
        /// class description for more info.
        /// </remarks>
        /// <param name="renderer">
        /// an
        /// <see cref="IRenderer"/>
        /// to be checked.
        /// </param>
        /// <returns>true if given renderer creates a new "Block formatting context" in terms of CSS, false otherwise.
        ///     </returns>
        public static bool IsRendererCreateBfc(IRenderer renderer) {
            return (renderer is RootRenderer) || (renderer is CellRenderer) || IsInlineBlock(renderer) || renderer.GetParent
                () is FlexContainerRenderer || FloatingHelper.IsRendererFloating(renderer) || IsAbsolutePosition(renderer
                ) || IsFixedPosition(renderer) || IsCaption(renderer) || AbstractRenderer.IsOverflowProperty(OverflowPropertyValue
                .HIDDEN, renderer, Property.OVERFLOW_X) || AbstractRenderer.IsOverflowProperty(OverflowPropertyValue.HIDDEN
                , renderer, Property.OVERFLOW_Y);
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

        private static bool IsCaption(IRenderer renderer) {
            return renderer.GetParent() is TableRenderer && (renderer is DivRenderer);
        }
    }
}
