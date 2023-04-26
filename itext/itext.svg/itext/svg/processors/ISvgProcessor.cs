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
using iText.StyledXmlParser.Node;

namespace iText.Svg.Processors {
    /// <summary>Interface for SVG processors.</summary>
    /// <remarks>
    /// Interface for SVG processors.
    /// Processors take the root
    /// <see cref="iText.StyledXmlParser.Node.INode"/>
    /// that corresponds to a Svg element
    /// and return a
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// that serves as the root for the same SVG
    /// </remarks>
    public interface ISvgProcessor {
        /// <summary>
        /// Process an SVG, returning the root of a renderer-tree and a list
        /// of named objects wrapped in a processor result object
        /// </summary>
        /// <param name="root">Root of the INode representation of the SVG</param>
        /// <param name="converterProps">configuration properties</param>
        /// <returns>
        /// root of the renderer-tree representing the SVG wrapped in {link
        /// <see cref="ISvgProcessorResult"/>
        /// }
        /// </returns>
        ISvgProcessorResult Process(INode root, ISvgConverterProperties converterProps);
    }
}
