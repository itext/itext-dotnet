/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Collections.Generic;
using iText.Layout;
using iText.Layout.Layout;

namespace iText.Layout.Renderer {
    /// <summary>
    /// A renderer object is responsible for drawing a corresponding layout object on
    /// a document or canvas.
    /// </summary>
    /// <remarks>
    /// A renderer object is responsible for drawing a corresponding layout object on
    /// a document or canvas. Every layout object has a renderer, by default one of
    /// the corresponding type, e.g. you can ask an
    /// <see cref="iText.Layout.Element.Image"/>
    /// for its
    /// <see cref="ImageRenderer"/>.
    /// Renderers are designed to be extensible, and custom implementations can be
    /// seeded to layout objects (or their custom subclasses) at runtime.
    /// </remarks>
    public interface IRenderer : IPropertyContainer {
        /// <summary>Adds a child to the current renderer</summary>
        /// <param name="renderer">a child to be added</param>
        void AddChild(IRenderer renderer);

        /// <summary>
        /// This method simulates positioning of the renderer, including all of its children, and returns
        /// the
        /// <see cref="iText.Layout.Layout.LayoutResult"/>
        /// , representing the layout result, including occupied area, status, i.e.
        /// if there was enough place to fit the renderer subtree, etc.
        /// </summary>
        /// <remarks>
        /// This method simulates positioning of the renderer, including all of its children, and returns
        /// the
        /// <see cref="iText.Layout.Layout.LayoutResult"/>
        /// , representing the layout result, including occupied area, status, i.e.
        /// if there was enough place to fit the renderer subtree, etc.
        /// <see cref="iText.Layout.Layout.LayoutResult"/>
        /// can be extended to return custom layout results for custom elements, e.g.
        /// <see cref="TextRenderer"/>
        /// uses
        /// <see cref="iText.Layout.Layout.TextLayoutResult"/>
        /// as its result.
        /// This method can be called standalone to learn how much area the renderer subtree needs, or can be called
        /// before
        /// <see cref="Draw(DrawContext)"/>
        /// , to prepare the renderer to be flushed to the output stream.
        /// </remarks>
        /// <param name="layoutContext">the description of layout area and any other additional information</param>
        /// <returns>result of the layout process</returns>
        LayoutResult Layout(LayoutContext layoutContext);

        /// <summary>
        /// Flushes the renderer subtree contents, i.e. draws itself on canvas,
        /// adds necessary objects to the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// etc.
        /// </summary>
        /// <param name="drawContext">
        /// contains the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to which the renderer subtree if flushed,
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// on which the renderer subtree is drawn and other additional parameters
        /// needed to perform drawing
        /// </param>
        void Draw(DrawContext drawContext);

        /// <summary>
        /// Gets the resultant occupied area after the last call to the
        /// <see cref="Layout(iText.Layout.Layout.LayoutContext)"/>
        /// method.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Layout.Layout.LayoutArea"/>
        /// instance
        /// </returns>
        LayoutArea GetOccupiedArea();

        /// <summary>Gets a property from this entity or one of its hierarchical parents.</summary>
        /// <remarks>
        /// Gets a property from this entity or one of its hierarchical parents.
        /// If the property is not found,
        /// <paramref name="defaultValue"/>
        /// will be returned.
        /// </remarks>
        /// <typeparam name="T1">the return type associated with the property</typeparam>
        /// <param name="property">the property to be retrieved</param>
        /// <param name="defaultValue">a fallback value</param>
        /// <returns>the value of the given property</returns>
        T1 GetProperty<T1>(int property, T1 defaultValue);

        /// <summary>
        /// Explicitly sets this object as the child of another
        /// <see cref="IRenderer"/>
        /// in
        /// the renderer hierarchy.
        /// </summary>
        /// <remarks>
        /// Explicitly sets this object as the child of another
        /// <see cref="IRenderer"/>
        /// in
        /// the renderer hierarchy. Some implementations also use this method
        /// internally to create a consistent hierarchy tree.
        /// </remarks>
        /// <param name="parent">the object to place higher in the renderer hierarchy</param>
        /// <returns>by default, this object</returns>
        IRenderer SetParent(IRenderer parent);

        /// <summary>
        /// Gets the parent
        /// <see cref="IRenderer"/>.
        /// </summary>
        /// <returns>
        /// direct parent
        /// <see cref="IRenderer">renderer</see>
        /// of this instance
        /// </returns>
        IRenderer GetParent();

        /// <summary>Gets the model element associated with this renderer.</summary>
        /// <returns>
        /// the model element, as a
        /// <see cref="iText.Layout.IPropertyContainer">container of properties</see>
        /// </returns>
        IPropertyContainer GetModelElement();

        /// <summary>
        /// Gets the child
        /// <see cref="IRenderer"/>
        /// s.
        /// </summary>
        /// <returns>
        /// a list of direct child
        /// <see cref="IRenderer">renderers</see>
        /// of this instance
        /// </returns>
        IList<IRenderer> GetChildRenderers();

        /// <summary>
        /// Indicates whether this renderer is flushed or not, i.e. if
        /// <see cref="Draw(DrawContext)"/>
        /// has already
        /// been called.
        /// </summary>
        /// <returns>whether the renderer has been flushed</returns>
        bool IsFlushed();

        /// <summary>Moves the renderer subtree by the specified offset.</summary>
        /// <remarks>Moves the renderer subtree by the specified offset. This method affects occupied area of the renderer.
        ///     </remarks>
        /// <param name="dx">the x-axis offset in points. Positive value will move the renderer subtree to the right.</param>
        /// <param name="dy">the y-axis offset in points. Positive value will move the renderer subtree to the top.</param>
        void Move(float dx, float dy);

        /// <summary>
        /// Gets a new instance of this class to be used as a next renderer, after this renderer is used, if
        /// <see cref="Layout(iText.Layout.Layout.LayoutContext)"/>
        /// is called more than once.
        /// </summary>
        /// <returns>new renderer instance</returns>
        IRenderer GetNextRenderer();
    }
}
