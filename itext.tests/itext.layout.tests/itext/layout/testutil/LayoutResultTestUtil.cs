/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Renderer;

namespace iText.Layout.Testutil {
    public sealed class LayoutResultTestUtil {
        private LayoutResultTestUtil() {
        }

        /// <summary>Gets the layout status from an element.</summary>
        /// <param name="element">the to-check element</param>
        /// <param name="document">document</param>
        /// <param name="area">bounding box</param>
        /// <returns>layout status</returns>
        public static int GetLayoutStatus(IBlockElement element, Document document, Rectangle area) {
            IRenderer renderer = element.CreateRendererSubTree().SetParent(document.GetRenderer());
            LayoutResult result = renderer.Layout(new LayoutContext(new LayoutArea(1, area)));
            return result.GetStatus();
        }

        /// <summary>Gets the layout status from an image.</summary>
        /// <param name="image">the to-check image</param>
        /// <param name="document">document</param>
        /// <param name="area">bounding box</param>
        /// <returns>layout status</returns>
        public static int GetLayoutStatusForImage(Image image, Document document, Rectangle area) {
            IRenderer renderer = image.CreateRendererSubTree().SetParent(document.GetRenderer());
            LayoutResult result = renderer.Layout(new LayoutContext(new LayoutArea(1, area)));
            return result.GetStatus();
        }
    }
}
