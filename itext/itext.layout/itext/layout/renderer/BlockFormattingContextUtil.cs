/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
