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

namespace iText.Svg.Renderers.Path {
    /// <summary>
    /// Interface that will provide a mapping from path element-data  instruction names to
    /// <see cref="IPathShape"/>.
    /// </summary>
    public interface IPathShapeMapper {
        /// <summary>Provides a mapping of Path-data instructions' names to path shape classes.</summary>
        /// <returns>
        /// a
        /// <see cref="System.Collections.IDictionary{K, V}"/>
        /// with Strings as keys and
        /// <see cref="IPathShape"/>
        /// implementations as values
        /// </returns>
        IDictionary<String, IPathShape> GetMapping();

        /// <summary>
        /// Provides a mapping of SVG path element's path-data instruction name to the appropriate number of arguments
        /// for a path command, based on this passed SVG path data instruction tag.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="System.Collections.IDictionary{K, V}"/>
        /// with Strings as keys and Integers as values
        /// </returns>
        IDictionary<String, int?> GetArgumentCount();
    }
}
