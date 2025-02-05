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
using System;
using System.Collections.Generic;
using iText.Layout.Font;
using iText.Svg.Renderers;

namespace iText.Svg.Processors {
    /// <summary>Interface for SVG processors results.</summary>
    public interface ISvgProcessorResult {
        /// <summary>Obtains a map of named-objects with their id's as keys and the objects as values</summary>
        /// <returns>
        /// Map of Strings as keys and
        /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
        /// as values
        /// </returns>
        IDictionary<String, ISvgNodeRenderer> GetNamedObjects();

        /// <summary>
        /// Obtains the wrapped
        /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
        /// root renderer.
        /// </summary>
        /// <returns>ISvgNodeRenderer</returns>
        ISvgNodeRenderer GetRootRenderer();

        /// <summary>
        /// Obtains the
        /// <see cref="iText.Layout.Font.FontProvider"/>.
        /// </summary>
        /// <returns>FontProvider</returns>
        FontProvider GetFontProvider();

        /// <summary>Obtains the list of temporary fonts</summary>
        /// <returns>FontSet</returns>
        FontSet GetTempFonts();
    }
}
