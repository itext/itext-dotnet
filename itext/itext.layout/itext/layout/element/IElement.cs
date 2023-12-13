/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Layout;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>
    /// This class represents a layout element, i.e. a piece of content that will
    /// take up 'physical' space on a canvas or document.
    /// </summary>
    /// <remarks>
    /// This class represents a layout element, i.e. a piece of content that will
    /// take up 'physical' space on a canvas or document. Its presence and positioning
    /// may influence the position of other
    /// <see cref="IElement"/>
    /// s on the layout surface.
    /// </remarks>
    public interface IElement : IPropertyContainer {
        /// <summary>
        /// Overrides the
        /// <see cref="iText.Layout.Renderer.IRenderer"/>
        /// instance which will be returned by the next call to the
        /// <see cref="GetRenderer()"/>.
        /// </summary>
        /// <param name="renderer">the renderer instance</param>
        void SetNextRenderer(IRenderer renderer);

        /// <summary>Gets a renderer for this element.</summary>
        /// <remarks>
        /// Gets a renderer for this element. Note that this method can be called more than once.
        /// By default each element should define its own renderer, but the renderer can be overridden by
        /// <see cref="SetNextRenderer(iText.Layout.Renderer.IRenderer)"/>
        /// method call.
        /// </remarks>
        /// <returns>a renderer for this element</returns>
        IRenderer GetRenderer();

        /// <summary>Creates a renderer subtree with root in the current element.</summary>
        /// <remarks>
        /// Creates a renderer subtree with root in the current element.
        /// Compared to
        /// <see cref="GetRenderer()"/>
        /// , the renderer returned by this method should contain all the child
        /// renderers for children of the current element.
        /// </remarks>
        /// <returns>a renderer subtree for this element</returns>
        IRenderer CreateRendererSubTree();
    }
}
