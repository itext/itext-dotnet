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
using System;
using System.Collections.Generic;
using iText.Kernel.Geom;

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

        /// <summary>Get a modifiable copy of the style and attribute map</summary>
        /// <returns>copy of the attributes and styles-map</returns>
        IDictionary<String, String> GetAttributeMapCopy();

        /// <summary>Creates a deep copy of this renderer, including it's subtree of children</summary>
        /// <returns>deep copy of this renderer</returns>
        ISvgNodeRenderer CreateDeepCopy();

        /// <summary>Calculates the current object bounding box.</summary>
        /// <param name="context">
        /// the current context, for instance it contains current viewport and available
        /// font data
        /// </param>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// representing the current object's bounding box, or null
        /// if bounding box is undefined
        /// </returns>
        Rectangle GetObjectBoundingBox(SvgDrawContext context);
    }
}
