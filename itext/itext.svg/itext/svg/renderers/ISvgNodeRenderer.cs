using System;
using System.Collections.Generic;

namespace iText.Svg.Renderers {
    /// <summary>
    /// Interface for SvgNodeRenderer, the renderer draws the SVG to its Pdf-canvas
    /// passed in
    /// <see cref="SvgDrawContext"/>
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

        /// <summary>
        /// Sets the map of XML node attributes and CSS style properties that this
        /// renderer needs.
        /// </summary>
        /// <param name="attributesAndStyles">the mapping from key names to values</param>
        void SetAttributesAndStyles(IDictionary<String, String> attributesAndStyles);

        /// <summary>Retrieves the property value for a given key name.</summary>
        /// <param name="key">the name of the property to search for</param>
        /// <returns>
        /// the value for this key, or
        /// <see langword="null"/>
        /// </returns>
        String GetAttribute(String key);

        /// <summary>Sets a property key and value pairs for a given attribute</summary>
        /// <param name="key">the name of the attribute</param>
        /// <param name="value">the value of the attribute</param>
        void SetAttribute(String key, String value);
    }
}
