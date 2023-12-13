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

namespace iText.Svg.Renderers.Factories {
    /// <summary>
    /// Interface that will provide a mapping from SVG tag names to Renderers that
    /// will be able to draw them.
    /// </summary>
    /// <remarks>
    /// Interface that will provide a mapping from SVG tag names to Renderers that
    /// will be able to draw them. It's used in
    /// <see cref="DefaultSvgNodeRendererFactory"/>
    /// to allow customizability in client code, and dependency injection in tests.
    /// </remarks>
    [System.ObsoleteAttribute(@"The interface will be removed in 7.2, while its implementation (DefaultSvgNodeRendererMapper ) will be used as our internal class. Users should override ISvgNodeRendererFactory (or at least DefaultSvgNodeRendererFactory ) and should not deal with the mapping class as it's more of an implementation detail."
        )]
    public interface ISvgNodeRendererMapper {
        /// <summary>Gets the map from tag names to Renderer classes.</summary>
        /// <returns>
        /// a
        /// <see cref="System.Collections.IDictionary{K, V}"/>
        /// with Strings as keys and {link @ISvgNodeRenderer}
        /// implementations as values
        /// </returns>
        IDictionary<String, Type> GetMapping();

        /// <summary>Get the list of tags that do not map to any Renderer and should be ignored</summary>
        /// <returns>a collection of ignored tags</returns>
        ICollection<String> GetIgnoredTags();
    }
}
