using System;
using System.Collections.Generic;

namespace iText.Svg.Renderers {
    /// <summary>
    /// Interface for SvgNodeRenderer, the renderer draws the SVG to its Pdf-canvas
    /// passed in
    /// <see cref="iText.Layout.Renderer.DrawContext"/>
    /// , applying styling
    /// (CSS and attributes).
    /// </summary>
    public interface ISvgNodeRenderer {
        /// <summary>Sets the parent of this renderer.</summary>
        /// <remarks>
        /// Sets the parent of this renderer. The parent may be the source of
        /// inherited properties and default values.
        /// </remarks>
        /// <param name="parent">the parent renderer</param>
        void SetParent(ISvgNodeRenderer parent);

        /// <summary>Gets the parent of this renderer.</summary>
        /// <remarks>
        /// Gets the parent of this renderer. The parent may be the source of
        /// inherited properties and default values.
        /// </remarks>
        /// <returns>the parent renderer; null in case of a root node</returns>
        ISvgNodeRenderer GetParent();

        /// <summary>Draws this element to a canvas-like object maintained in the context.</summary>
        /// <param name="context">
        /// the object that knows the place to draw this element and
        /// maintains its state
        /// </param>
        void Draw(SvgDrawContext context);

        /// <summary>Adds a renderer as the last element of the list of children.</summary>
        /// <param name="child">any renderer</param>
        void AddChild(ISvgNodeRenderer child);

        /// <summary>Gets all child renderers of this object.</summary>
        /// <returns>the list of child renderers (in the order that they were added)</returns>
        IList<ISvgNodeRenderer> GetChildren();

        void SetAttributesAndStyles(IDictionary<String, String> attributesAndStyles);
    }
}
