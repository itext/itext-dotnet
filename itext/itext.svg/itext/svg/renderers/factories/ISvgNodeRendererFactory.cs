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
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Factories {
    /// <summary>
    /// Interface for the factory used by
    /// <see cref="iText.Svg.Processors.Impl.DefaultSvgProcessor"/>.
    /// </summary>
    /// <remarks>
    /// Interface for the factory used by
    /// <see cref="iText.Svg.Processors.Impl.DefaultSvgProcessor"/>.
    /// Pass along using
    /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>.
    /// </remarks>
    public interface ISvgNodeRendererFactory {
        /// <summary>Create a configured renderer based on the passed Svg tag and set its parent.</summary>
        /// <param name="tag">Representation of the Svg tag, with all style attributes set</param>
        /// <param name="parent">renderer of the parent tag</param>
        /// <returns>Configured ISvgNodeRenderer</returns>
        ISvgNodeRenderer CreateSvgNodeRendererForTag(IElementNode tag, ISvgNodeRenderer parent);

        /// <summary>Checks whether the provided tag is an ignored tag of this factory or not.</summary>
        /// <remarks>Checks whether the provided tag is an ignored tag of this factory or not. If ignored, the factory won't process this IElementNode into an ISvgNodeRenderer.
        ///     </remarks>
        /// <param name="tag">the IElementNode</param>
        /// <returns>true if ignored</returns>
        bool IsTagIgnored(IElementNode tag);
    }
}
