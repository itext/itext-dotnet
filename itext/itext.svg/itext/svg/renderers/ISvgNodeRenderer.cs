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
